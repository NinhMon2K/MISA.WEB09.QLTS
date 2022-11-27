using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.Common.Entities
{
    /// <summary>
    /// Paging tài sản
    /// </summary>
    public class PagingAsset
    {
        /// <summary>
        /// Giá trị tìm kiếm theo mã tài sản hoặc loại tài sản
        /// </summary>
        public string? keyword { get; set; }

        /// <summary>
        /// Mảng giá trị id bộ phận sử dụng
        /// </summary>
        public IList<string>? listDepartment { get; set; }

        /// <summary>
        /// Mảng giá trị id bộ loại tài sản
        /// </summary>
        public IList<string>? listCategory { get; set; }

        /// <summary>
        /// Số bản ghi hiện lên trên 1 trang
        /// </summary>
        public int? limit { get; set; }

        /// <summary>
        /// Trang đứng hiện tại
        /// </summary>
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
