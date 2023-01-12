using MISA.WEB09.QLTS.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.Common.Entities
{
    public class BackAddVoucherDetaill 
    {
        public Voucher Voucher { get; set; }

       public IList<Asset>? listAssetDetail { get; set; }


        public BackAddVoucherDetaill()
        {
        }

        public BackAddVoucherDetaill(Voucher voucher, IList<Asset>? listAssetDetail)
        {
            Voucher = voucher;
            this.listAssetDetail = listAssetDetail;
        }
    }
}
