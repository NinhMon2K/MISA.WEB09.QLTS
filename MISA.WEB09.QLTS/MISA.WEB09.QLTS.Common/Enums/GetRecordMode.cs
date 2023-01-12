using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.Common.Enums
{
    /// <summary>
    /// Danh sách các kiểu lấy bản ghi
    /// </summary>
    public enum GetRecordMode
    {
        /// <summary>
        /// Lấy các bản ghi không được chọn
        /// </summary>
        NotSelected = 0,

        /// <summary>
        /// Lấy các bản ghi được chọn
        /// </summary>
        Selected = 1,

        /// <summary>
        /// Lấy các bản ghi không được chọn và không ghi tăng
        /// </summary>
        NotSelectedNotIncrement = 2,
    }
}
