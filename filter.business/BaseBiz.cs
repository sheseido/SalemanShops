using filter.framework.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.business
{
    public class BaseBiz
    {
        protected LogHelper mLogHelper;

        public BaseBiz()
        {
            this.mLogHelper = new LogHelper();
        }
    }
}
