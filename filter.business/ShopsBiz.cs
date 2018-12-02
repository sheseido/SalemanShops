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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace filter.business
{
    public class ShopsBiz:BaseBiz
    {
        private ShopsManager shopsManager;
        private SalemanManager salemanManager;

        public ShopsBiz()
        {
            shopsManager = new ShopsManager();
            salemanManager = new SalemanManager();
        }

        /// <summary>
        /// 分页获取店铺数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public async Task<ResultBase<PagedData<ShopsModel>>> GetPagedShops(int page, int size, Dictionary<string, string> conditions)
        {
            var data = await shopsManager.GetPagedShops(page, size, conditions);
            return ResultBase<PagedData<ShopsModel>>.Sucess(data);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultBase> Save(SaveShopsModel request)
        {
            if (request == null)
                return ResultBase.Fail(Enum_ResultBaseCode.ParamError);
            else if (string.IsNullOrEmpty(request.Name))
                return ResultBase.Fail(Enum_ResultBaseCode.ParamLackError);
            else if(string.IsNullOrEmpty(request.SalemanName))
                return ResultBase.Fail(Enum_ResultBaseCode.ParamLackError);

            //校验业务员
            var saleman = await salemanManager.FindByName(request.SalemanName);
            if (saleman == null)
                return ResultBase.Fail("业务员不存在,请检查");

            if (request.Id == 0)
            {
                ShopEntity entity = request.Convert<ShopEntity>();
                entity.SalemanId = saleman.Id;
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = 0;

                await shopsManager.InsertAsync(entity);
            }
            else
            {
                var entity = shopsManager.FindById<ShopEntity>(request.Id);
                if (entity == null)
                    return ResultBase.Fail(Enum_ResultBaseCode.DataNotFoundError);

                entity.Name = request.Name;
                entity.Contact = request.Contact;
                entity.Mobile = request.Mobile;
                entity.SalemanId = saleman.Id;
                entity.UpdatedAt = DateTime.Now;
                entity.UpdatedBy = 0;
                await salemanManager.UpdateAsync(entity);
            }
            return ResultBase.Sucess();
        }

        /// <summary>
        /// 导入店铺
        /// </summary>
        /// <param name="request"></param>
        /// <param name="files"></param>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public async Task<ResultBase> ImportShops(HttpFileCollectionBase files, int manager)
        {
            if (files == null || files.Count == 0)
                return ResultBase<UploadExcelCheckResultModel>.Fail("请上传文件");

            var file = files[0];
            if (file.ContentLength == 0)
                return ResultBase<UploadExcelCheckResultModel>.Fail("文件内容为空");

            string fileName = file.FileName;//取得文件名字
            var fileExt = fileName.Substring(fileName.LastIndexOf('.') + 1);
            string[] msExcelFiles = { "xlsx", "xls" };
            if (!msExcelFiles.Any(m => m == fileExt))
                return ResultBase<UploadExcelCheckResultModel>.Fail("只支持EXCEL文件");

            var filePath = $"/Upload/Shops/";
            var saveFilePath = HttpContext.Current.Server.MapPath($"~{filePath}");
            if (!Directory.Exists(saveFilePath))
            {
                Directory.CreateDirectory(saveFilePath);
            }

            //改名称
            var newFileName = ConvertHelper.ConvertDtToUnixTimeSpan(DateTime.Now) + "." + fileExt;
            string path = saveFilePath + newFileName;//获取存储的目标地址
            file.SaveAs(path);

            //读取excel
            var data = ExcelHelper.ReadExcelNoIndex<ExportShopsModel>(path, "店铺上传");

            if (data == null || data.Count == 0)
                return ResultBase.Fail("没有有效数据");

            mLogHelper.Info($"开始执行导入快递单号,总量:{data.Count}");

            //分组批量插入数据
            var groups = data.GroupBy(m => m.SalemanName);
            List<int> taskIds = new List<int>();
            foreach (var group in groups)
            {
                var saleman =await salemanManager.FindByName(group.Key);
                if (saleman == null)
                    continue;

                var items = group.ToList();
                List<ShopEntity> entities = new List<ShopEntity>();
                var total = 0;
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.Name) || string.IsNullOrEmpty(item.SalemanName))
                        continue;

                    var entity = item.Convert<ShopEntity>();
                    entity.SalemanId = saleman.Id;
                    entity.CreatedAt = DateTime.Now;
                    entity.CreatedBy = manager;
                    entities.Add(entity);
                    total += 1;
                }

                var trans = DapperDataAccess.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    await shopsManager.InsertBatchAsync(entities, trans);

                    DapperDataAccess.Commit(trans);
                }
                catch (ShowMessageException ex)
                {
                    DapperDataAccess.Rollback(trans);
                    return ResultBase.Fail(ex.Message);
                }
                catch (Exception ex)
                {
                    DapperDataAccess.Rollback(trans);
                    return ResultBase.Fail(ex.Message);
                }
            }

            mLogHelper.Info($"导入执行完毕");
            return ResultBase.Sucess();
        }

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultBase<ShopsModel>> GetShopInfoById(int id)
        {
            var shops = shopsManager.FindById<ShopEntity>(id);
            if (shops == null)
                return ResultBase<ShopsModel>.Fail(Enum_ResultBaseCode.DataNotFoundError);

            var saleman = salemanManager.FindById<SalemanEntity>(shops.SalemanId);
            if(saleman==null)
                return ResultBase<ShopsModel>.Fail("业务员数据错误");

            var result = shops.Convert<ShopsModel>();
            result.SalemanName = saleman.Name;

            return ResultBase<ShopsModel>.Sucess(result);
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

            await shopsManager.LogicDeleteAsync<ShopEntity>(idsList);
            return ResultBase.Sucess();
        }

        
    }
}
