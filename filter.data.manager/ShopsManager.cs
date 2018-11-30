using filter.data.entity;
using filter.data.model;
using filter.data.model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.manager
{
    public class ShopsManager : DapperDataAccess
    {
        /// <summary>
        /// 分页获取店铺数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页数据量</param>
        /// <param name="conditions">条件</param>
        /// <returns></returns>
        public async Task<PagedData<ShopsModel>> GetPagedShops(int page, int size, Dictionary<string, string> conditions)
        {
            string colums = @" 
    shop.Id
    ,shop.Name
    ,shop.Contact
    ,shop.Mobile
    ,shop.SalemanId
    ,shop.CreatedAt
    ,saleman.Name as SalemanName ";
            StringBuilder query = new StringBuilder();
            query.Append(@" 
shopinfo shop
        JOIN
    salemaninfo saleman ON shop.SalemanId = saleman.Id
WHERE
    saleman.IsDelete = 0
        AND shop.IsDelete = 0 ");

            string key = "";
            if (conditions.ContainsKey("Key"))
            {
                key = conditions["Key"];
                if (!string.IsNullOrEmpty(key))
                    query.Append(" AND (shop.Name LIKE @Key) ");
            }

            string saleman = "";
            if (conditions.ContainsKey("SalemanName"))
            {
                saleman = conditions["SalemanName"];
                if (!string.IsNullOrEmpty(saleman))
                    query.Append(" AND (saleman.Name LIKE @SalemanName) ");
            }

            var result = await PagedFindAllAsync<ShopsModel>(query.ToString(), colums, page, size, new { Key = $"%{key}%", SalemanName = $"%{saleman}%" }, " Id desc ");
            return result;
        }

        

        /// <summary>
        /// 通过业务员姓名查找
        /// </summary>
        /// <param name="salemanName"></param>
        /// <returns></returns>
        public async Task<List<ShopEntity>> FindBySalemanId(int salemanId)
        {
            string sql = $" SELECT * FROM shopinfo WHERE IsDelete = 0 AND SalemanId={salemanId}";
            var data = await FindAllAsync<ShopEntity>(sql, null);
            return data.ToList();
        }
    }
}
