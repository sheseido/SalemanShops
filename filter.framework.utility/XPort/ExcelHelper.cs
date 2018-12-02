using filter.framework.utility.XPort;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace filter.framework.utility.Xport
{
    public static class ExcelHelper
    {
        public static List<T> ReadExcelNoIndex<T>(string filePath, string sheetName)
            where T : new()
        {
            List<T> result = null;

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath + " not exists.");
            }

            List<ExcelColumnMapping> mappings = new List<ExcelColumnMapping>();

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<ExcelColumnNameAttribute>();
                if (attr != null)
                {
                    mappings.Add(new ExcelColumnMapping() { Property = property, ColumnIndex = mappings.Count, ColumnName = (attr as ExcelColumnNameAttribute).ColumnName });
                }
            }

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var workbook = WorkbookFactory.Create(stream);

                var sheet = workbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet == null)
                {
                    throw new FormatException("sheet not exists");
                }

                result = new List<T>();
                IRow row = null;
                ICell cell = null;
                T data = default(T);
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    data = new T();
                    try
                    {
                        for (int j = 0; j < mappings.Count; j++)
                        {
                            ExcelColumnMapping map = mappings[j];
                            cell = row.GetCell(map.ColumnIndex);
                            if (cell != null)
                            {
                                if (cell.CellType == CellType.Numeric)
                                {
                                    if (map.Property.PropertyType == typeof(double))
                                        map.Property.SetValue(data, cell.NumericCellValue);
                                    else if (cell.DateCellValue != null)
                                        map.Property.SetValue(data, cell.DateCellValue.ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(cell.StringCellValue))
                                    {
                                        cell.SetCellType(CellType.String);
                                        if (map.ColumnName == "联系电话" || map.ColumnName == "订单编号"
                                            || map.ColumnName == "联系手机" || map.ColumnName == "手机")
                                            map.Property.SetValue(data, cell.StringCellValue.Replace("\"", "").Replace("=", "").Replace("'", ""));
                                        else
                                            map.Property.SetValue(data, cell.StringCellValue);
                                    }
                                }
                            }
                        }
                        result.Add(data);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"第{i}行,{ex.Message}");
                    }
                }
            }

            return result;
        }

        public static List<T> ReadExcel<T>(string filePath, string sheetName)
            where T : new()
        {
            List<T> result = null;

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath + " not exists.");
            }

            List<ExcelColumnMapping> mappings = new List<ExcelColumnMapping>();

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<ExcelColumnNameAttribute>();
                var indexAttr = property.GetCustomAttribute<ExcelColumnIndexAttribute>();
                if (attr != null)
                {
                    mappings.Add(new ExcelColumnMapping() { Property = property, ColumnIndex = (indexAttr as ExcelColumnIndexAttribute).ColumnIndex, ColumnName = (attr as ExcelColumnNameAttribute).ColumnName });
                }
            }

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var workbook = WorkbookFactory.Create(stream);

                var sheet = workbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet == null)
                {
                    throw new FormatException("sheet not exists");
                }

                result = new List<T>();
                IRow row = null;
                ICell cell = null;
                T data = default(T);

                //读取列头
                var excelColumnHeads= sheet.GetRow(0);
                foreach (var item in excelColumnHeads.Cells)
                {
                    cell = item;
                    if (cell != null)
                    {
                        cell.SetCellType(CellType.String);
                        var value = cell.StringCellValue;
                        var mapping = mappings.FirstOrDefault(m => m.ColumnName == value.Trim());
                        if (mapping != null)
                            mapping.ColumnIndex = excelColumnHeads.Cells.IndexOf(item);
                    }
                }
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    data = new T();
                    try
                    {
                        for (int j = 0; j < mappings.Count; j++)
                        {
                            ExcelColumnMapping map = mappings[j];
                            cell = row.GetCell(map.ColumnIndex);
                            if (cell != null)
                            {
                                cell.SetCellType(CellType.String);
                                var value = cell.StringCellValue;
                                if (map.ColumnName == "联系电话" || map.ColumnName == "订单编号"
                                    || map.ColumnName == "联系手机" || map.ColumnName == "手机")
                                    value =value.Replace("\"", "").Replace("=", "").Replace("'", "");
                                map.Property.SetValue(data, value);
                            }
                        }
                        result.Add(data);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return result;
        }

        public static void WriteExcel<T>(MemoryStream stream, string sheetName, List<T> data)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName ?? "数据");

            List<ExcelColumnMapping> mappings = new List<ExcelColumnMapping>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<ExcelColumnNameAttribute>();
                if (attr != null)
                {
                    mappings.Add(new ExcelColumnMapping() { Property = property, ColumnIndex = mappings.Count, ColumnName = (attr as ExcelColumnNameAttribute).ColumnName });
                }
            }

            IRow row = sheet.CreateRow(0);
            foreach (var mapping in mappings)
            {
                row.CreateCell(mapping.ColumnIndex).SetCellValue(mapping.ColumnName);
            }

            for (int i = 0; i < data.Count; i++)
            {
                row = sheet.CreateRow(i + 1);
                foreach (var mapping in mappings)
                {
                    var value = mapping.Property.GetValue(data[i]);
                    if (value != null)
                    {
                        row.CreateCell(mapping.ColumnIndex).SetCellValue(value.ToString());
                    }
                }
            }

            for (int i = 0; i < mappings.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            workbook.Write(stream);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
