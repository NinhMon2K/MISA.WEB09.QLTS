﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

    public class BasesController<T> : ControllerBase
    {
        #region Field

        private IBaseBL<T> _baseBL;

        #endregion

        #region Constructor

        public BasesController(IBaseBL<T> baseBL)
        {
            _baseBL = baseBL;
        }

        #endregion


        #region Method

        /// <summary>
        /// Lấy danh sách toàn bộ bản ghi
        /// </summary>
        /// <returns>Danh sách toàn bộ bản ghi</returns>
        /// Cretaed by: NNNINH (09/11/2022)
        [HttpGet("GetAll")]
        public IActionResult GetAllRecords()
        {
            try
            {
                var records = _baseBL.GetAllRecords();
                return StatusCode(StatusCodes.Status200OK, records);
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
        [HttpGet("{recordId}")]
        public IActionResult GetRecordById([FromRoute] Guid recordId)
        {
            try
            {
                T record = _baseBL.GetRecordById(recordId);
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
        #endregion

        #region API Insert
        /// <summary>
        /// Thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Đối tượng cần thêm mới</param>
        /// <returns>ID đối tượng vừa thêm mới</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        [HttpPost]
        public IActionResult InsertRecord([FromBody] T record)
        {
            try
            {
                if (record != null)
                {
                    var result = _baseBL.InsertRecord(record);

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
        #endregion

        #region API Update
        /// <summary>
        /// Cập nhật 1 bản ghi
        /// </summary>
        /// <param name="recordId">ID bản ghi cần cập nhật</param>
        /// <param name="record">Đối tượng cần cập nhật theo</param>
        /// <returns>Đối tượng sau khi cập nhật</returns>
        /// Cretaed by:  NNNINH (11/11/2022)
        [HttpPut("{recordId}")]
        public IActionResult UpdateRecord([FromRoute] Guid recordId, [FromBody] T record)
        {
            try
            {
                var result = _baseBL.UpdateRecord(recordId, record);

                if (result.Success)
                {
                    return StatusCode(StatusCodes.Status200OK, result.Data);
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
       
        #region API Delete
        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">ID bản ghi cần xóa</param>
        /// <returns>ID bản ghi vừa xóa</returns>
        /// Cretaed by: NNNINH (11/11/2022)
        [HttpDelete("{recordId}")]
        public IActionResult DeleteRecord(Guid recordId)
        {
            try
            {
                var result = _baseBL.DeleteRecord(recordId);

                if (result != Guid.Empty)
                {
                    return StatusCode(StatusCodes.Status200OK, result);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                        ErrorCode.Exception,
                        Resource.DevMsg_DeleteFailed,
                        Resource.UserMsg_DeleteFailed,
                        result,
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
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="recordIdList">Danh sách ID các bản ghi cần xóa</param>
        /// <returns>Danh sách ID các bản ghi đã xóa</returns>
        /// Cretaed by: NNNINH (12/11/2022)
        [HttpPost("batch-delete")]
        public IActionResult DeleteMultiRecords([FromBody] List<string> recordIdList)
        {
            try
            {
                var numberOfAffectedRows  = _baseBL.DeleteMultiRecords(recordIdList);

                if (numberOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, numberOfAffectedRows);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                       ErrorCode.Exception,
                        Resource.DevMsg_DeleteFailed,
                        Resource.UserMsg_DeleteFailed,
                        recordIdList,
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


        #region API Excel
        /// <summary>
        /// Xuất file excel danh sách bản ghi
        /// </summary>
        /// <returns>File excel danh sách bản ghi</returns>
        /// Author: NNNINH (26/11/2022)
        [HttpPost("export")]
        public IActionResult ExportExcel([FromBody] IEnumerable<T> record)
        {
            var stream = _baseBL.ExportExcel(record);
            string excelName = $"{"danhsachtaisan"}_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        #endregion


        #region API NextCode
        /// <summary>
        /// Sinh mã tài sản tiếp theo
        /// </summary>
        /// <returns>Mã tài sản tiếp theo</returns>
        /// Cretaed by: NNNINH (09/11/2022)
        [HttpGet("NextCode")]
        public IActionResult NextAssetCode()
        {
            try
            {
                string nextAssetCode = _baseBL.NextAssetCode();

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
        #endregion

    }

}

