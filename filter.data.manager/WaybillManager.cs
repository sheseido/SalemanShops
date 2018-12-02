using filter.data.model;
using filter.data.model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.manager
{
    public class WaybillManager : DapperDataAccess
    {
        /// <summary>
        /// 分页获取运单数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页数据量</param>
        /// <param name="conditions">条件</param>
        /// <returns></returns>
        public async Task<PagedData<WaybillModel>> GetPagedWaybills(int page, int size, Dictionary<string, string> conditions)
        {
            string colums = @" 
    waybill.Id,
    waybill.Code,
    waybill.Time,
    shop.Name AS ShopName,
    saleman.Name AS SalemanName,
    waybill.Province,
    waybill.City,
    waybill.Weight,
    waybill.CreatedAt ";
            StringBuilder query = new StringBuilder();
            query.Append(@" 
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
                if (!string.IsNullOrEmpty(key))
                    query.Append(" AND waybill.Code LIKE @Key ");
            }

            string shopname = "";
            if (conditions.ContainsKey("ShopName"))
            {
                shopname = conditions["ShopName"];
                if (!string.IsNullOrEmpty(shopname))
                    query.Append(" AND shop.Name LIKE @ShopName ");
            }

            string saleman = "";
            if (conditions.ContainsKey("SalemanName"))
            {
                saleman = conditions["SalemanName"];
                if (!string.IsNullOrEmpty(saleman))
                    query.Append(" AND saleman.Name LIKE @SalemanName ");
            }

            string time = "";
            if (conditions.ContainsKey("beginDate"))
            {
                time = conditions["beginDate"].Split('T')[0];
                if (!string.IsNullOrEmpty(time))
                    query.Append(" AND waybill.Time = @Time ");
            }

            var result = await PagedFindAllAsync<WaybillModel>(query.ToString(), colums, page, size, new { Key = $"%{key}%", ShopName = $"%{shopname}%", SalemanName = $"%{saleman}%", Time = $"{time}" }, " Id desc ");
            return result;
        }
    }
}
