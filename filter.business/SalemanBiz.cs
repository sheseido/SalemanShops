using filter.data.entity;
using filter.data.manager;
using filter.data.model;
using filter.data.model.Dto;
using filter.framework.utility;
using filter.framework.utility.Xport;
using filter.framework.utility.XPort;
using filter.framework.web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.business
{
    public class SalemanBiz
    {
        private SalemanManager salemanManager;

        public SalemanBiz()
        {
            salemanManager = new SalemanManager();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultBase<SaleManModel>> GetSalemanById(int id)
        {
            var data = await salemanManager.FindByIdAsync<SalemanEntity>(id);
            if (data == null)
                return ResultBase<SaleManModel>.Fail(Enum_ResultBaseCode.DataNotFoundError);

            return ResultBase<SaleManModel>.Sucess(data.Convert<SaleManModel>());
        }

        /// <summary>
        /// 分页获取所有业务员数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public async Task<ResultBase<PagedData<SaleManModel>>> GetPagedSalemans(int page,int size,Dictionary<string,string> conditions)
        {
            var data = await salemanManager.GetPagedSalemans(page, size, conditions);
            var result = new PagedData<SaleManModel>();
            if (data != null && data.Items?.Count() > 0)
            {
                result.PageCount = data.PageCount;
                result.TotalCount = data.TotalCount;
                List<SaleManModel> models = new List<SaleManModel>();
                foreach (var item in data.Items)
                {
                    var model = item.Convert<SaleManModel>();
                    model.CreatedAt = ConvertHelper.ConvertDtToString(item.CreatedAt);

                    //获取所属店铺
                    model.ShopsCount = await salemanManager.FindShopsCount(item.Id);

                    models.Add(model);
                }
                result.Items = models;
            }
            return ResultBase<PagedData<SaleManModel>>.Sucess(result);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultBase> Save(SaveSalemanModel request)
        {
            if (request == null)
                return ResultBase.Fail(Enum_ResultBaseCode.ParamError);
            else if (string.IsNullOrEmpty(request.Name))
                return ResultBase.Fail(Enum_ResultBaseCode.ParamLackError);

            if (request.Id == 0)
            {
                SalemanEntity entity = request.Convert<SalemanEntity>();
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = 0;

                await salemanManager.InsertAsync(entity);
            }
            else
            {
                var entity = salemanManager.FindById<SalemanEntity>(request.Id);
                if (entity == null)
                    return ResultBase.Fail(Enum_ResultBaseCode.DataNotFoundError);

                entity.Name = request.Name;
                entity.Mobile = request.Mobile;
                entity.SettlementPrice = request.SettlementPrice;
                entity.UpdatedAt = DateTime.Now;
                entity.UpdatedBy = 0;
                await salemanManager.UpdateAsync(entity);
            }
            return ResultBase.Sucess();
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<ResultBase> Delete(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return ResultBase.Fail("参数缺失");
            var idsString = ids.Split(',');
            var idsList = idsString.Select(id => Convert.ToInt32(id)).ToList();

            await salemanManager.LogicDeleteAsync<SalemanEntity>(idsList);
            return ResultBase.Sucess();
        }

        /// <summary>
        /// 获取业务员所有店铺导出数据
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public ResultBase<MemoryStream> ExportSalemanAllShops(int salemanId,out string salemanName)
        {
            salemanName = "";
            List<ExportShopsModel> data = salemanManager.GetSalemanAllShops(salemanId).GetAwaiter().GetResult();
            if (data != null && data.Count > 0)
                salemanName = data.First().SalemanName;

            MemoryStream stream = new NPOIMemoryStream(false);
            ExcelHelper.WriteExcel(stream, "业务员店铺", data);
            return ResultBase<MemoryStream>.Sucess(stream);
        }

        /// <summary>
        /// 获取业务员所有运单导出数据
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public ResultBase<MemoryStream> ExportSalemanAllWaybills(Dictionary<string, string> conditions)
        {
            List<WaybillExportWithPriceModel> data = salemanManager.GetSalemanAllWaybills(conditions).GetAwaiter().GetResult();

            MemoryStream stream = new NPOIMemoryStream(false);
            ExcelHelper.WriteExcel(stream, "业务员运单", data);
            return ResultBase<MemoryStream>.Sucess(stream);
        }
    }
}
