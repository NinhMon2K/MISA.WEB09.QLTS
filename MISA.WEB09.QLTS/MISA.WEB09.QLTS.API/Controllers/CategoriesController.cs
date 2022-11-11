using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB09.QLTS.BL;
using MISA.WEB09.QLTS.Common.Entities;

namespace MISA.WEB09.QLTS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoriesController : BasesController<Category>
    {
        public CategoriesController(IBaseBL<Category> baseBL) : base(baseBL)
        {
        }
    }
}
