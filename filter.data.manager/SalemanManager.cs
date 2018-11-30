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
    }
}
