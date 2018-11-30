using filter.business;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace filter.ui.web.Controllers
{
    public class HomeController : WebSiteController
    {
        private SystemBiz systemBiz;
        public HomeController()
        {
            systemBiz = new SystemBiz();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (CurrentAccount > 0)
            {
                return RedirectToAction("Manager");
            }
            return View();
        }

        public ActionResult Manager()
        {
            if (CurrentAccount <= 0)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<ActionResult> Login(LoginRequest request)
        //{
        //    var result = await merchantBiz.UserLogin(request.Account, request.Password);
        //    if (result.Result)
        //    {
        //        var auth = BuildAuth("" + result.Data);
        //        Response.Headers.Add(HttpConstant.Header_Auth, auth);
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// 获取用户信息
        ///// </summary>
        ///// <returns></returns>
        //public async Task<ActionResult> UserInfo()
        //{
        //    var result = await merchantBiz.GetUser(CurrentAccount);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// 修改密码
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<ActionResult> UpdateUserPwd(UpdateUserPwdRequest request)
        //{
        //    var result = await merchantBiz.UpdatePwd(CurrentAccount, request.OriginPassword, request.NewPassword);
        //    return Json(result);
        //}

        ///// <summary>
        ///// 注销
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<ResultBase> Logout()
        //{
        //    Response.Headers.Add(HttpConstant.Header_Auth, "clear");
        //    return new ResultBase() { Result = true };
        //}

        public async Task<ActionResult> GetMenus()
        {
            var result = await systemBiz.GetMerchantMenus(CurrentAccount);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}