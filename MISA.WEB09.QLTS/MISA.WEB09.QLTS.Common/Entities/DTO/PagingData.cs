using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.Common.Entities
{
    /// <summary>
    /// Dữ liệu cho phân trang
    /// </summary>
    public class PagingData<T>
    {
        #region Propety

        /// <summary>
        /// Danh sách các bản ghi hiển thị
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Tổng số bản ghi hiển thị
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// Tổng số lượng
        /// </summary>
        public long? TotalQuantity { get; set; } = 0;

        /// <summary>
        /// Tổng số nguyên giá
        /// </summary>
        public double? TotalCost { get; set; } = 0;

        /// <summary>
        /// Tổng số hao mòn lũy kế
        /// </summary>
        public double? TotalDepreciation { get; set; } = 0;

        /// <summary>
        /// Tổng số còn lại
        /// </summary>
        public double? TotalRemain { get; set; } = 0;

        #endregion

        #region Constructor

        public PagingData() { }

        public PagingData(IEnumerable<T> data, long totalCount, long? totalQuantity, double? totalCost, double? totalDepreciation, double? totalRemain)
        {
            Data = data;
            TotalCount = totalCount;
            TotalQuantity = totalQuantity;
            TotalCost = totalCost;
            TotalDepreciation = totalDepreciation;
            TotalRemain = totalRemain;
        }

        #endregion
    }
}
