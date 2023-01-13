using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB09.QLTS.BL;
using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.Common.Enums;
using MISA.WEB09.QLTS.Common.Resources;

namespace MISA.WEB09.QLTS.API.Controllers
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VouchersController : BasesController<Voucher>
    {
        #region Field

        private IVoucherBL _voucherBL;

        #endregion

        #region Constructor
        public VouchersController(IVoucherBL voucherBL) : base(voucherBL)
        {
            _voucherBL = voucherBL;
        }
        #endregion

        #region Method
        /// <summary>
        /// Lấy danh sách các chứng từ có chọn lọc
        /// </summary>
        /// <param name="keyword">Từ để tìm kiếm theo số chứng từ và nội dung</param>
        /// <param name="limit">Số chứng từ muốn lấy</param>
        /// <param name="page">Số trang bắt đầu lấy</param>
        /// <returns>Danh sách các chứng từ sau khi chọn lọc và các giá trị khác</returns>
        /// Created by: NNNINH (27/12/2022)
        [HttpGet("filters")]
        public IActionResult FilterVouchers([FromQuery] string? keyword, [FromQuery] int limit, [FromQuery] int page)
        {
            try
            {
                var filterResponse = _voucherBL.FilterVouchers(keyword, limit, page);

                return StatusCode(StatusCodes.Status200OK, filterResponse);
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


        /// <summary>
        /// Lấy chi tiết chứng từ
        /// </summary>
        /// <param name="voucherId">Id chứng từ</param>
        /// <param name="limit">Số bản ghi muốn lấy</param>
        /// <param name="page">Số trang bắt đầu lấy</param>
        /// <returns>Danh sách các tài sản theo chứng từ</returns>
        /// Created by:  NNNINH (01/01/2023)
        [HttpGet("detail/{voucherId}")]
        public IActionResult GetVoucherDetail([FromQuery] string? keyword, [FromRoute] Guid voucherId, [FromQuery] int limit, [FromQuery] int page)
        {
            try
            {
                var filterResponse = _voucherBL.GetVoucherDetail(keyword, voucherId, limit, page);
                return StatusCode(StatusCodes.Status200OK, filterResponse);
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




        /// <summary>
        /// Lấy 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID của bản ghi cần lấy</param>
        /// <returns>Bản ghi có ID được truyền vào</returns>
        /// Created by: NNNINH (09/11/2022)
        [HttpGet("{voucherId}/{assetId}")]
        public IActionResult GetVoucherDetailById([FromRoute] Guid voucherId,[FromRoute] Guid assetId)
        {
            try
            {
                VoucherDetail record = _voucherBL.GetVoucherDetailById(voucherId,assetId);
                if (record != null)
                {
                    return StatusCode(StatusCodes.Status200OK, record);
                }

                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                        ErrorCode.SelectFailed,
                        Resource.DevMsg_SelectFailed,
                        Resource.UserMsg_SelectFailed,
                        Resource.MoreInfo_SelectFailed,
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


        /// <summary>
        /// Thêm mới chứng từ kèm detail
        /// <param name="backAddVoucherDetaill">Đối tượng chứng từ kèm danh sách tài sản cần chứng từ</param>
        /// <returns>Mã chứng từ vừa thêm</returns>
        /// Cretaed by: NNNINH (06/01/2023)
        [HttpPost("addVouchers")]
        public IActionResult BackAddVoucherDetail([FromBody] BackAddVoucherDetaill backAddVoucherDetaill)
        {
            try
            {
                if (backAddVoucherDetaill != null)
                {
                    var result = _voucherBL.BackAddVoucherDetail(backAddVoucherDetaill);

                    if (result.Success)
                    {
                        return StatusCode(StatusCodes.Status201Created, result.Data);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                            ErrorCode.Exception,
                            Resource.DevMsg_ValidateFailed,
                            Resource.UserMsg_ValidateFailed,
                            result.Data,
                            HttpContext.TraceIdentifier));
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                    ErrorCode.Exception,
                    Resource.DevMsg_Exception,
                    Resource.UserMsg_Exception,
                    Resource.MoreInfo_Exception,
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


        /// <summary>
        /// Cập nhật chứng từ kèm detail 
        /// </summary>
        /// <param name="voucherId">ID chứng từ đang sửa</param>
        /// <param name="backAddVoucherDetaill">Đối tượng chứng từ kèm danh sách tài sản cần chứng từ</param>
        /// <returns>Mã chứng từ vừa sửa</returns>
        /// Cretaed by: NNNINH (06/01/2023)
        [HttpPost("detail/batch-update")]
        public IActionResult UpadateVoucherDetail(Guid voucherId, BackAddVoucherDetaill backAddVoucherDetaill)
        {
            try
            {

                var result = _voucherBL.UpadateVoucherDetail(voucherId, backAddVoucherDetaill);

                if (result.Success)
                {
                    return StatusCode(StatusCodes.Status200OK, result.Data);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                        ErrorCode.Exception,
                        Resource.MoreInfo_UpdateFailed,
                        Resource.MoreInfo_UpdateFailed,
                        backAddVoucherDetaill,
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

        #endregion
    }
}
