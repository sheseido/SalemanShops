using filter.data.entity;
using filter.data.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.manager
{
    public class SalemanManager : DapperDataAccess
    {
        /// <summary>
        /// 获取业务员列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页数据量</param>
        /// <param name="conditions">条件</param>
        /// <returns></returns>
        public async Task<PagedData<SalemanEntity>> GetPagedSalemans(int page, int size, Dictionary<string, string> conditions)
        {
            string colums = @" * ";
            StringBuilder query = new StringBuilder();
            query.Append(@" salemaninfo WHERE IsDelete = 0 ");

            string key = "";
            if (conditions.ContainsKey("Key"))
            {
                key = conditions["Key"];
                query.Append(" AND (Name LIKE @Key) ");
            }

            var result = await PagedFindAllAsync<SalemanEntity>(query.ToString(), colums, page, size, new { Key = $"%{key}%" }, " Id desc ");
            return result;
        }

        /// <summary>
        /// 通过姓名查找业务员
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<SalemanEntity> FindByName(string name)
        {
            string sql= $"SELECT * FROM salemaninfo WHERE Name = '{name}'";
            var data= await FindBySqlAsync<SalemanEntity>(sql, null);
            return data;
        }

        /// <summary>
        /// 获取所属店铺数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> FindShopsCount(int id)
        {
            string sql = $@"
SELECT 
    COUNT(1)
FROM
    shopinfo
WHERE
    IsDelete = 0 AND SalemanId = {id}";

            return await FindBySqlAsync<int>(sql);
        }

        /// <summary>
        /// 获取业务员所有店铺导出数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页数据量</param>
        /// <param name="conditions">条件</param>
        /// <returns></returns>
        public async Task<List<ExportShopsModel>> GetSalemanAllShops(int salemanId)
        {
            string sql = $@"
SELECT 
    shop.Name,
    shop.Contact,
    shop.Mobile,
    saleman.Name AS SalemanName
FROM
    shopinfo shop
        JOIN
    salemaninfo saleman ON shop.SalemanId = saleman.Id
WHERE
    saleman.IsDelete = 0
        AND shop.IsDelete = 0
        AND shop.SalemanId ={salemanId}; ";

            var result = await FindAllAsync<ExportShopsModel>(sql);
            return result.ToList();
        }

        /// <summary>
        /// 获取业务员所有运单导出数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页数据量</param>
        /// <param name="conditions">条件</param>
        /// <returns></returns>
        public async Task<List<WaybillExportWithPriceModel>> GetSalemanAllWaybills(Dictionary<string, string> conditions)
        {
            StringBuilder query = new StringBuilder($@"
SELECT 
    waybill.Code,
    waybill.Time,
    shop.Name AS ShopName,
    saleman.Name AS SalemanName,
    waybill.Province,
    waybill.City,
    waybill.Weight,
    saleman.SettlementPrice,
    waybill.CreatedAt
FROM
    waybillinfo waybill
        JOIN
    shopinfo shop ON waybill.ShopId = shop.Id
        JOIN
    salemaninfo saleman ON waybill.SalemanId = saleman.Id
WHERE
    waybill.IsDelete = 0 ");

            string key = "";
            if (conditions.ContainsKey("Key"))
            {
                key = conditions["Key"];
                if (!string.IsNullOrEmpty(key) && key != "undefined")
                    query.Append(" AND waybill.Code LIKE @Key ");
            }

            string shopname = "";
            if (conditions.ContainsKey("ShopName"))
            {
                shopname = conditions["ShopName"];
                if (!string.IsNullOrEmpty(shopname) && shopname != "undefined")
                    query.Append(" AND shop.Name LIKE @ShopName ");
            }

            string saleman = "";
            if (conditions.ContainsKey("SalemanName"))
            {
                saleman = conditions["SalemanName"];
                if (!string.IsNullOrEmpty(saleman) && saleman != "undefined")
                    query.Append(" AND saleman.Name LIKE @SalemanName ");
            }

            string time = "";
            if (conditions.ContainsKey("beginDate"))
            {
                time = conditions["beginDate"].Split('T')[0];
                if (!string.IsNullOrEmpty(time) && time != "undefined")
                    query.Append(" AND waybill.Time = @Time ");
            }

            query.Append(" ORDER BY waybill.Id DESC; ");

            var result = await FindAllAsync<WaybillExportWithPriceModel>(query.ToString(), new { Key = $"%{key}%", ShopName = $"%{shopname}%", SalemanName = $"%{saleman}%", Time = $"{time}" });
            return result.ToList();
        }
    }
}
