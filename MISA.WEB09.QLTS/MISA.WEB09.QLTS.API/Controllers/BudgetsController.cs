using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB09.QLTS.BL;
using MISA.WEB09.QLTS.Common.Entities;

namespace MISA.WEB09.QLTS.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BudgetsController : BasesController<Budget>
    {
        #region Constructor 
        public BudgetsController(IBaseBL<Budget> baseBL) : base(baseBL)
        {
        } 
        #endregion
    }
}
