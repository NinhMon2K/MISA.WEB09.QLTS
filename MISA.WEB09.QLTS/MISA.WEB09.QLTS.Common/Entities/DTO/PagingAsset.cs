using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.Common.Entities
{
    public class PagingAsset
    {
        public string? keyword { get; set; }
        public IList<string>? listDepartment { get; set; }
        public IList<string>? listCategory { get; set; }
        public int? limit { get; set; }
        public int? page { get; set; }

        public PagingAsset()
        {
        }

        public PagingAsset(string? keyword, IList<string>? listDepartment, IList<string>? listCategory, int? limit, int? page)
        {
            this.keyword = keyword;
            this.listDepartment = listDepartment;
            this.listCategory = listCategory;
            this.limit = limit;
            this.page = page;
        }
    }
}
