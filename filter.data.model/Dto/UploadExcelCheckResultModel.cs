using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.model.Dto
{
    public class UploadExcelCheckResultModel
    {
        public string FileName { get; set; }
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public bool IsTemplateFile { get; set; }
    }
}
