using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badge.Web.Models.Shared
{
    public class PaginationViewModel<T>
        where T : class
    {
        public List<T> Data { get; set; } = new List<T>();
        public int Count { get; set; }
        public int countotale { get; set; }
        public int Skip { get; set; }
        public int CountBadge { get; set; }
    }
}
