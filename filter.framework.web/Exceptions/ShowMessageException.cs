using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.framework.web
{
    public class ShowMessageException : Exception
    {
        public ShowMessageException(string message) : base(message)
        { }
    }
}
