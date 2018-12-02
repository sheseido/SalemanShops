using filter.business;
using filter.data.model.Dto;
using filter.framework.utility;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace filter.ui.web.Controllers
{
    public class SalemanController : WebSiteController
    {
        private SalemanBiz salemanBiz;

        public SalemanController()
        {
            salemanBiz = new SalemanBiz();
        }

        /// <summary>
        /// 分页获取所有业务员数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetPagedSalemans(int page, int size)
        {
            var result = await salemanBiz.GetPagedSalemans(page, size, RequestDictionary);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Save(SaveSalemanModel request)
        {
            var result = await salemanBiz.Save(request);
            return Json(result);
        }

        /// <summary>
        /// 业务员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Info(int id)
        {
            var result = await salemanBiz.GetSalemanById(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string ids)
        {
            var result = await salemanBiz.Delete(ids);
            return Json(result);
        }

        /// <summary>
        /// 导出业务员所有店铺
        /// </summary>
        /// <returns></returns>
        public FileResult ExportSalemanAllShops(int id)
        {
            var result = salemanBiz.ExportSalemanAllShops(id, out string salemanName);
            return File(result.Data, "application/vnd.ms-excel", $"{salemanName}-{ConvertHelper.ConvertDtToString(DateTime.Now)}-店铺数据.xlsx");
        }

        /// <summary>
        /// 导出业务员所有运单
        /// </summary>
        /// <returns></returns>
        public FileResult ExportSalemanAllWaybill()
        {
            var result = salemanBiz.ExportSalemanAllWaybills(RequestDictionary);
            var saleman = "";
            if (RequestDictionary.ContainsKey("SalemanName")&& RequestDictionary["SalemanName"] != "undefined")
            {
                saleman = RequestDictionary["SalemanName"];
            }
            return File(result.Data, "application/vnd.ms-excel", $"{saleman}-运单数据.xlsx");
        }
    }
}