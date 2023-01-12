using MISA.WEB09.QLTS.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.Common.Entities
{
    /// <summary>
    /// Chi tiết chứng từ
    /// </summary>
    public class VoucherDetail : BaseEntity
    {
        /// <summary>
        /// Id chi tiết chứng từ
        /// </summary>
        [PrimaryKey]
        public Guid? voucher_detail_id { get; set; }

        /// <summary>
        /// Mã chứng từ
        /// </summary>
        [PrimaryKey]
        public Guid? voucher_id { get; set; }

        /// <summary>
        /// Id tài sản
        /// </summary>
        [PrimaryKey]
        public Guid fixed_asset_id { get; set; }

        public int? flag { get; set; }

    }
}
