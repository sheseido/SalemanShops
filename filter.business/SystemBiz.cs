using filter.data.model.Dto;
using filter.framework.web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.business
{
    public class SystemBiz
    {
        public async Task<ResultBase<List<FuncModel>>> GetMerchantMenus(int userId)
        {
            var data= new List<FuncModel>();
            data.Add(new FuncModel()
            {
                code = "home",
                name = "首页",
                items = new List<FuncItemResponse>() {
                       new FuncItemResponse(){
                            name="salesman",
                            title="业务员管理",
                             parent="home",
                       },
                       new FuncItemResponse(){
                            name="shops",
                            title="店铺管理",
                             parent="home",
                       },
                       new FuncItemResponse(){
                            name="waybill",
                            title="运单管理",
                             parent="home",
                       },
                   }
            });
            return ResultBase<List<FuncModel>>.Sucess(data);
        }
    }
}
