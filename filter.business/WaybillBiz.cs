using filter.data.entity;
using filter.data.manager;
using filter.data.model;
using filter.data.model.Dto;
using filter.framework.utility;
using filter.framework.utility.Xport;
using filter.framework.web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace filter.business
{
    public class WaybillBiz : BaseBiz
    {
        private WaybillManager waybillManager;
        private ShopsManager shopsManager;
        private SalemanManager salemanManager;

        public WaybillBiz()
        {
            waybillManager = new WaybillManager();
            shopsManager = new ShopsManager();
            salemanManager = new SalemanManager();
        }

        /// <summary>
        /// 分页获取运单数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public async Task<ResultBase<PagedData<WaybillModel>>> GetPagedWaybills(int page, int size, Dictionary<string, string> conditions)
        {
            var data = await waybillManager.GetPagedWaybills(page, size, conditions);
            return ResultBase<PagedData<WaybillModel>>.Sucess(data);
        }

        /// <summary>
        /// 导入店铺
        /// </summary>
        /// <param name="request"></param>
        /// <param name="files"></param>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public async Task<ResultBase> ImportWaybills(HttpFileCollectionBase files, int manager)
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

            var filePath = $"/Upload/Waybill/";
            var saveFilePath = HttpContext.Current.Server.MapPath($"~{filePath}");
            if (!Directory.Exists(saveFilePath))
            {
                Directory.CreateDirectory(saveFilePath);
            }

            string path = saveFilePath + file.FileName;//获取存储的目标地址
            file.SaveAs(path);

            try
            {
                //读取excel
                var data = ExcelHelper.ReadExcelNoIndex<WaybillExportModel>(path, "Sheet1");

                if (data == null || data.Count == 0)
                    return ResultBase.Fail("没有有效数据");
                else if (data.Count > 60000)
                    return ResultBase.Fail("excel表格行数过大,请分批导入");

                mLogHelper.Info($"开始执行导入快递单号,总量:{data.Count}");

                //检查数据
                var index = 1;
                foreach (var item in data)
                {
                    index++;
                    if (string.IsNullOrEmpty(item.ShopName))
                        return ResultBase.Fail($"第{index}行,运单号{item.Code}, 店铺名为空");
                    //查找店铺
                    var shop = await shopsManager.FindByName(item.ShopName);
                    if (shop == null)
                        return ResultBase.Fail($"第{index}行,运单号{item.Code}, 店铺{item.ShopName}不存在,请先添加店铺");
                    else if (shop.SalemanId == 0)
                        return ResultBase.Fail($"第{index}行,运单号{item.Code},店铺{item.ShopName}业务员数据错误");
                }

                var successedCount = 0;
                var exitsCount = 0;

                //插入数据
                List<int> taskIds = new List<int>();
                foreach (var item in data)
                {
                    //查找店铺
                    var shop = await shopsManager.FindByName(item.ShopName);

                    //相同运单号跳过
                    var waybill = waybillManager.FindByCode(item.Code);
                    if (waybill != null)
                    {
                        exitsCount++;
                        continue;
                    }

                    WaybillEntity waybillEntity = new WaybillEntity()
                    {
                        Code = item.Code,
                        ShopId = shop.Id,
                        SalemanId = shop.SalemanId,
                        Time = item.Time,
                        Province = item.Province,
                        City = item.City,
                        Weight = item.Weight,
                        CreatedAt = DateTime.Now,
                        CreatedBy = manager,
                    };
                    await waybillManager.InsertAsync(waybillEntity);
                    successedCount++;
                }


                mLogHelper.Info($"导入执行完毕");
                return ResultBase.Sucess($"总数居:{data.Count},成功执行:{successedCount},存在相同运单数:{exitsCount}");
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("for key 'uk_waybillinfo_Code'") > -1)
                    return ResultBase.Fail($"存在相同运单号,{ex.Message}");
                else
                    return ResultBase.Fail(ex.Message);
            }
        }
    }
}
