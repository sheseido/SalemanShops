using filter.framework.web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace filter.ui.web
{
    public class WebSiteController : BaseController
    {
        public virtual int CurrentAccount
        {
            get
            {
                //var userId = 0;
                var userId = 1;

                //try
                //{
                //    var cookie = HttpContext.Request.Cookies["eock"];//ExpressOrderCookie
                //    var value = Server.UrlDecode(cookie.Value);
                //    var auth = AuthHelper.Get(value);
                //    userId = int.Parse(auth);
                //}
                //catch
                //{
                //    //userId = 0;
                //    userId = 1;
                //}

                return userId;
            }
        }
    }
}