using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.Common.Enums;
using MISA.WEB09.QLTS.Common.Resources;
using MISA.WEB09.QLTS.Common.Attributes;
using MISA.WEB09.QLTS.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.BL
{
    public class VoucherBL : BaseBL<Voucher>, IVoucherBL
    {
        #region Field

        private IVoucherDL _voucherDL;
        private IBaseDL<Voucher> _baseDL;
        

        #endregion
        #region Constructor
        public VoucherBL(IVoucherDL voucherDL) : base(voucherDL)
        {
            _voucherDL = voucherDL;
        }
        #endregion
        #region Method

        /// <summary>
        /// Lấy 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID của bản ghi cần lấy</param>
        /// <returns>Bản ghi có ID được truyền vào</returns>
        /// Created by: NNNINH (09/11/2022)
        public VoucherDetail GetVoucherDetailById(Guid voucherId,Guid assetId)
        {
            return _voucherDL.GetVoucherDetailById(voucherId,assetId);
        }
        /// <summary>
        /// Lấy danh sách các chứng từ có chọn lọc
        /// </summary>
        /// <param name="keyword">Từ để tìm kiếm theo số chứng từ và nội dung</param>
        /// <param name="limit">Số chứng từ muốn lấy</param>
        /// <param name="page">Số trang bắt đầu lấy</param>
        /// <returns>Danh sách các chứng từ sau khi chọn lọc và các giá trị khác</returns>
        /// Created by: NNNINH (27/12/2022)
        public PagingData<Voucher> FilterVouchers(string? keyword, int limit, int page)
        {
            return _voucherDL.FilterVouchers(keyword, limit, page);
        }

        /// <summary>
        /// Lấy chi tiết chứng từ
        /// </summary>
        /// <param name="voucherId">Id chứng từ</param>
        /// <param name="limit">Số bản ghi muốn lấy</param>
        /// <param name="page">Số trang bắt đầu lấy</param>
        /// <returns>Danh sách các tài sản theo chứng từ</returns>
        /// Created by:  NNNINH (01/01/2023)
        public PagingData<Asset> GetVoucherDetail(string? keyword, Guid voucherId, int limit, int page)
        {
            return _voucherDL.GetVoucherDetail(keyword, voucherId, limit, page);
        }

        

        /// <summary>
        /// Thêm mới chứng từ kèm detail
        /// <param name="backAddVoucherDetaill">Đối tượng chứng từ kèm danh sách tài sản cần chứng từ</param>
        /// <returns>Mã chứng từ vừa thêm</returns>
        /// Cretaed by: NNNINH (06/01/2023)
        public ServiceResponse BackAddVoucherDetail(BackAddVoucherDetaill backAddVoucherDetaill)
        {
            var newRecordID = Guid.NewGuid();    
            var validateResult = ValidateRequestData(backAddVoucherDetaill.Voucher, newRecordID);
            if (validateResult != null && validateResult.Success)
            {
                var res = _voucherDL.BackAddVoucherDetail(backAddVoucherDetaill, newRecordID );

                if (res != Guid.Empty)
                {
                    return new ServiceResponse
                    {
                        Success = true,
                        Data = res
                    };
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Data = new ErrorResult(
                            ErrorCode.InvalidInput,
                            Resource.DevMsg_InsertFailed,
                            Resource.UserMsg_InsertFailed,
                            Resource.MoreInfo_InsertFailed)
                    };
                }
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Data = validateResult?.Data
                };
            }
        }

        /// <summary>
        /// Cập nhật chứng từ kèm detail 
        /// </summary>
        /// <param name="voucherId">ID chứng từ đang sửa</param>
        /// <param name="backAddVoucherDetaill">Đối tượng chứng từ kèm danh sách tài sản cần chứng từ</param>
        /// <returns>Mã chứng từ vừa sửa</returns>
        /// Cretaed by: NNNINH (06/01/2023)
        public ServiceResponse UpadateVoucherDetail(Guid voucherId, BackAddVoucherDetaill backAddVoucherDetaill)
        {
            var validateResult = ValidateRequestData(backAddVoucherDetaill.Voucher, voucherId);

            if (validateResult != null && validateResult.Success)
            {
                var inputRecordID = _voucherDL.UpadateVoucherDetail(voucherId, backAddVoucherDetaill);

                if (inputRecordID != Guid.Empty)
                {
                    return new ServiceResponse
                    {
                        Success = true,
                        Data = inputRecordID
                    };
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Data = new ErrorResult(
                            ErrorCode.InvalidInput,
                            Resource.DevMsg_InsertFailed,
                            Resource.UserMsg_InsertFailed,
                            Resource.MoreInfo_InsertFailed)
                    };
                }
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Data = validateResult?.Data
                };
            }

        }
        #endregion
    }
}
