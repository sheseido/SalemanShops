using filter.business;
using filter.data.model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace filter.ui.web.Controllers
{
    public class ShopsController : WebSiteController
    {
        private ShopsBiz shopsBiz;

        public ShopsController()
        {
            shopsBiz = new ShopsBiz();
        }

        /// <summary>
        /// 分页获取店铺列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetPagedShops(int page, int size)
        {
            var result = await shopsBiz.GetPagedShops(page, size, RequestDictionary);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Save(SaveShopsModel request)
        {
            var result = await shopsBiz.Save(request);
            return Json(result);
        }

        /// <summary>
        /// 导入店铺
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ImportShops()
        {
            var result = await shopsBiz.ImportShops(Request.Files, CurrentAccount);
            return Json(result);
        }

        /// <summary>
        /// 店铺信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Info(int id)
        {
            var result = await shopsBiz.GetShopInfoById(id);
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
            var result = await shopsBiz.Delete(ids);
            return Json(result);
        }

        
    }
}