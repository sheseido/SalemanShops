using filter.business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace filter.ui.web.Controllers
{
    public class WaybillController : WebSiteController
    {
        private WaybillBiz waybillBiz;

        public WaybillController()
        {
            waybillBiz = new WaybillBiz();
        }

        /// <summary>
        /// 分页获取运单列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetPagedWaybills(int page, int size)
        {
            var result = await waybillBiz.GetPagedWaybills(page, size, RequestDictionary);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导入运单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Import()
        {
            var result = await waybillBiz.ImportWaybills(Request.Files, CurrentAccount);
            return Json(result);
        }
    }
}