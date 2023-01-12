using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB09.QLTS.BL;
using MISA.WEB09.QLTS.Common.Entities;

namespace MISA.WEB09.QLTS.API.Controllers
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VoucherDetailsController : BasesController<VoucherDetail>
    {
        #region Constructor
        public VoucherDetailsController(IBaseBL<VoucherDetail> baseBL) : base(baseBL)
        {
        } 
        #endregion
    }
}
