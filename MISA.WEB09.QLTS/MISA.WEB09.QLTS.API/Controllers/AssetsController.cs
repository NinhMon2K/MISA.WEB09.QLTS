using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB09.QLTS.BL;
using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.Common.Enums;
using MISA.WEB09.QLTS.Common.Resources;

namespace MISA.WEB09.QLTS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AssetsController : BasesController<Asset>
    {
      
        #region Field

        private IAssetBL _assetBL;

        #endregion

        #region Constructor
       
        public AssetsController(IAssetBL assetBL) : base(assetBL)
        {
            _assetBL = assetBL;
        }

        #endregion
        /// <summary>
        /// Sinh mã tài sản tiếp theo
        /// </summary>
        /// <returns>Mã tài sản tiếp theo</returns>
        /// Cretaed by: NNNINH (09/11/2022)
        [HttpGet("nextCode")]
        public IActionResult NextAssetCode()
        {
            try
            {
                string nextAssetCode = _assetBL.NextAssetCode();

                // Xử lý dữ liệu trả về
                if (nextAssetCode != "")
                {
                    return StatusCode(StatusCodes.Status200OK, new NextCode()
                    {
                        Code = nextAssetCode,
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                        ErrorCode.UpdateFailed,
                        Resource.DevMsg_UpdateFailed,
                        Resource.UserMsg_UpdateFailed,
                        Resource.MoreInfo_UpdateFailed,
                        HttpContext.TraceIdentifier));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                    ErrorCode.Exception,
                    Resource.DevMsg_Exception,
                    Resource.UserMsg_Exception,
                    Resource.MoreInfo_Exception,
                    HttpContext.TraceIdentifier));
            }
        }
    }
}
