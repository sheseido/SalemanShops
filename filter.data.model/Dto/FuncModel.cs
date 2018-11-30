using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.model.Dto
{
    public class FuncModel
    {
        public string code { get; set; }

        public string name { get; set; }

        public List<FuncItemResponse> items;
    }

    public class FuncItemResponse
    {
        public string parent { get; set; }
        public string title { get; set; }

        public string name { get; set; }
    }
}
