using filter.framework.utility.XPort;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace filter.framework.utility.Xport
{
    public static class CsvHelper
    {
        /// <summary>
        /// 写入CSV文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="fileName">文件全名</param>
        /// <returns>是否写入成功</returns>
        public static Boolean SaveCSV(DataTable dt, string fullFileName)
        {
            Boolean r = false;
            FileStream fs = new FileStream(fullFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();

            r = true;
            return r;  
        }

        /// <summary>
        /// 打开CSV 文件
        /// </summary>
        /// <param name="fileName">文件全名</param>
        /// <returns>DataTable</returns>
        public static List<T> ReadCsv<T>(string fullFileName) where T : new()
        {
            return ReadCsv<T>(fullFileName, 0, 0, 0, 0, true);
        }

        /// <summary>
        /// 打开CSV 文件
        /// </summary>
        /// <param name="fileName">文件全名</param>
        /// <param name="firstRow">开始行</param>
        /// <param name="firstColumn">开始列</param>
        /// <param name="getRows">获取多少行</param>
        /// <param name="getColumns">获取多少列</param>
        /// <param name="haveTitleRow">是有标题行</param>
        /// <returns>DataTable</returns>
        public static List<T> ReadCsv<T>(string filePath, Int16 firstRow = 0, Int16 firstColumn = 0, Int16 getRows = 0, Int16 getColumns = 0, bool haveTitleRow = true)
             where T : new()
        {
            List<T> result = new List<T>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath + " not exists.");
            }

            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            try
            {
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
                
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine;
                //标示列数
                int columnCount = 0;
                //是否已建立了表的字段
                bool bCreateTableColumns = false;
                //第几行
                int iRow = 1;

                //去除无用行
                if (firstRow > 0)
                {
                    for (int i = 1; i < firstRow; i++)
                    {
                        sr.ReadLine();
                    }
                }

                // { ",", ".", "!", "?", ";", ":", " " };
                string[] separators = { "," };
                //逐行读取CSV中的数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    strLine = strLine.Trim();
                    aryLine = strLine.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
                    columnCount = aryLine.Length;
                    if (columnCount == 0)
                        continue;

                    if (bCreateTableColumns == false)
                    {
                        bCreateTableColumns = true;
                        var columnList = aryLine.ToList();
                        foreach (var item in columnList)
                        {
                            var mapping = mappings.FirstOrDefault(m => m.ColumnName == item.Replace("\"", "").Trim());
                            if (mapping != null)
                                mapping.ColumnIndex = columnList.IndexOf(item);
                        }
                        continue;
                    }

                    T data = default(T);
                    data = new T();
                    try
                    {
                        for (int j = 0; j < mappings.Count; j++)
                        {
                            ExcelColumnMapping map = mappings[j];
                            var value = aryLine[map.ColumnIndex].Replace("\"", "").Replace("=", "").Replace("'", "");
                            if (value.Length > 49)
                                value = value.Substring(0, 49);
                            map.Property.SetValue(data, value);
                        }
                        result.Add(data);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    iRow = iRow + 1;
                    if (getRows > 0)
                    {
                        if (iRow > getRows)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
                fs.Close();
            }
            return result;
        }
    }
}
