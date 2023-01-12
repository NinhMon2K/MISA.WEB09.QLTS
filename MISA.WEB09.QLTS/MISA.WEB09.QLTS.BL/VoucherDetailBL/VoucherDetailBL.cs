using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.BL
{
    public class VoucherDetailBL : BaseBL<VoucherDetail>, IVoucherDetailBL
    {
        public VoucherDetailBL(IBaseDL<VoucherDetail> baseDL) : base(baseDL)
        {
        }
    }
}
