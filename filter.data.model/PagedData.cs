using System.Collections.Generic;
using System.Linq;

namespace filter.data.model
{
    public class PagedData<T>
    {
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public IEnumerable<T> Items { get; set; }

        public PagedData()
        {
            TotalCount = 0;
            PageCount = 0;
            Items = null;
        }

        public PagedData(long count, int pagesize, IEnumerable<T> items)
        {
            TotalCount = count;
            Items = items;
            if (items != null && items.Count() > 0)
            {
                PageCount = count / pagesize;
                if (count % pagesize > 0)
                {
                    PageCount += 1;
                }
            }
        }
        public PagedData(int pageCount, IEnumerable<T> items)
        {
            this.PageCount = pageCount;
            this.Items = items;
        }
    }
}
