using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.framework.utility.XPort
{
    public class NPOIMemoryStream : MemoryStream
    {
        public bool AllowClose { get; set; }

        public NPOIMemoryStream()
        {
            AllowClose = true;
        }

        public NPOIMemoryStream(bool allowClose)
        {
            AllowClose = allowClose;
        }

        public override void Close()
        {
            if (AllowClose)
            {
                base.Close();
            }
        }
    }
}
