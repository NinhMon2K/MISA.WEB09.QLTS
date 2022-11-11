using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.DL;
using MISA.WEB09.QLTS.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.WEB09.QLTS.Common.Resources;
using MISA.WEB09.QLTS.Common.Enums;

namespace MISA.WEB09.QLTS.BL
{
    public class BaseBL<T> : IBaseBL<T>
    {

        #region Field
        private IBaseDL<T> _baseDL;
        #endregion

        #region Constructor
        public BaseBL(IBaseDL<T> baseDL)
        {
            _baseDL = baseDL;
        }
        #endregion


        #region API Get
        /// <summary>
        /// Lấy danh sách toàn bộ bản ghi
        /// </summary>
        /// <returns>Danh sách toàn bộ bản ghi</returns>
        /// Cretaed by: NNNINH (09/11/2022)
        public IEnumerable<T> GetAllRecords()
        {
            return _baseDL.GetAllRecords();
        }
        /// <summary>
        /// Lấy 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID của bản ghi cần lấy</param>
        /// <returns>Bản ghi có ID được truyền vào</returns>
        /// Created by: NNNINH (09/11/2022)
        public T GetRecordById(Guid recordId)
        {
            return _baseDL.GetRecordById(recordId);
        }

        /// <summary>
        /// Lấy danh sách các bản ghi theo từ khóa
        /// </summary>
        /// <param name="keyword">Từ để tìm kiếm bản ghi</param>
        /// <param name="type">Loại dữ liệu được tìm kiếm</param>
        /// <returns>Danh sách các bản ghi sau khi chọn lọc</returns>
        /// Created by: NNNINH (09/11/2022)
        public PagingData<T> FilterRecords(string? keyword, string type)
        {
            return _baseDL.FilterRecords(keyword, type); 
          
        }
        #endregion

        #region API Insert
        /// <summary>
        /// Thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Đối tượng bản ghi cần thêm mới</param>
        /// <returns>ID của bản ghi vừa thêm. Return về Guid rỗng nếu thêm mới thất bại</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        public ServiceResponse InsertRecord(T record)
        {
            var validateResult = ValidateRequestData(record, Guid.Empty);

            if (validateResult != null && validateResult.Success)
            {
                var newRecordID = _baseDL.InsertRecord(record);

                if (newRecordID != Guid.Empty)
                {
                    return new ServiceResponse
                    {
                        Success = true,
                        Data = newRecordID
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

        #region ValidateData 
        /// <summary>
        /// Validate dữ liệu truyền lên từ API
        /// </summary>
        /// <param name="record">Đối tượng cần validate</param>
        /// <param name="recordId">Id đối tượng cần validate</param>
        /// <returns>Đối tượng ServiceResponse mô tả validate thành công hay thất bại</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        private ServiceResponse ValidateRequestData(T record, Guid recordId)
        {
            // Validate dữ liệu đầu vào
            var properties = typeof(T).GetProperties();
            var validateFailures = new List<string>();

            if (record != null)
            {
                // Duyệt qua từng phần tử
                foreach (var property in properties)
                {
                    // Lấy giá trị của thuộc tính đó
                    var propertyValue = property.GetValue(record, null);
                    // Kiểm tra property có require
                    var IsNotNullOrEmptyAttribute = (IsNotNullOrEmptyAttribute?)Attribute.GetCustomAttribute(property, typeof(IsNotNullOrEmptyAttribute));
                    // Kiểm tra xem property đã có attibute Required không ho
                    if (IsNotNullOrEmptyAttribute != null && string.IsNullOrEmpty(propertyValue?.ToString()))
                    {
                        validateFailures.Add(IsNotNullOrEmptyAttribute.ErrorMessage);
                    }           
                    var IsNotDuplicateAttribute = (IsNotDuplicateAttribute?)Attribute.GetCustomAttribute(property, typeof(IsNotDuplicateAttribute));
                    if (IsNotDuplicateAttribute != null)
                    {
                        int count = _baseDL.DuplicateRecordCode(propertyValue, recordId);
                        if (count > 0) validateFailures.Add(IsNotDuplicateAttribute.ErrorMessage);
                    }
                }
            }

            if (validateFailures.Count > 0)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Data = validateFailures
                };
            }
            return new ServiceResponse
            {
                Success = true
            };
        } 
        #endregion




    }
}
