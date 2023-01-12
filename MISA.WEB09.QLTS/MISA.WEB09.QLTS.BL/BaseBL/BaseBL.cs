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
using System.Xml.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

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
            var newRecordID = Guid.NewGuid();
            var validateResult = ValidateRequestData(record, newRecordID);
            if (validateResult != null && validateResult.Success)
            {
                var res = _baseDL.InsertRecord(record, newRecordID);

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
        #endregion

        #region API Update
        /// <summary>
        /// Cập nhật 1 bản ghi
        /// </summary>
        /// <param name="recordId">ID bản ghi cần cập nhật</param>
        /// <param name="record">Đối tượng cần cập nhật theo</param>
        /// <returns>Đối tượng sau khi cập nhật</returns>
        /// Cretaed by: NNNINH (11/11/2022)
        public ServiceResponse UpdateRecord(Guid recordId, T record)
        {
            var validateResult = ValidateRequestData(record, recordId);

            if (validateResult != null && validateResult.Success)
            {
                var inputRecordID = _baseDL.UpdateRecord(recordId, record);

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

        #region API Delete
        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">ID bản ghi cần xóa</param>
        /// <returns>ID bản ghi vừa xóa</returns>
        /// Cretaed by: NNNINH (11/11/2022)
        public Guid DeleteRecord(Guid recordId)
        {
            return _baseDL.DeleteRecord(recordId);
        }

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="recordIdList">Danh sách ID các bản ghi cần xóa</param>
        /// <returns>Danh sách ID các bản ghi đã xóa</returns>
        /// Cretaed by: NNNINH (11/11/2022)
        public int DeleteMultiRecords(List<string> recordIdList)
        {
            return _baseDL.DeleteMultiRecords(recordIdList);
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
            //var validateFailures = new List<string>();

            // duyệt qua từng phần tử
            foreach (var property in properties)
            {
                // Lấy giá trị của thuộc tính đó
                var propertyValue = property.GetValue(record, null);
                // Kiểm tra property có require
                var IsNotNullOrEmptyAttribute = (IsNotNullOrEmptyAttribute?)Attribute.GetCustomAttribute(property, typeof(IsNotNullOrEmptyAttribute));
                // Kiểm tra xem property đã có attibute Required không 
                if (IsNotNullOrEmptyAttribute != null && string.IsNullOrEmpty(propertyValue?.ToString()))
                {
                    validateFailures.Add(IsNotNullOrEmptyAttribute.ErrorMessage);
                }

                // Kiểm tra xem property Attribure dùng để xác định 1 property không được trùng lặp 
                var IsNotDuplicateAttribute = (IsNotDuplicateAttribute?)Attribute.GetCustomAttribute(property, typeof(IsNotDuplicateAttribute));
                if (IsNotDuplicateAttribute != null)
                {
                    int count = _baseDL.DuplicateRecordCode(propertyValue, recordId);
                    if (count > 0) validateFailures.Add(IsNotDuplicateAttribute.ErrorMessage);
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

        /// <summary>
        /// Binding format style cho file excel
        /// </summary>
        /// <param name="workSheet">Sheet cần binding format</param>
        /// <param name="record">Danh sách bản ghi</param>
        /// Author: NNNINH (26/11/2022)
        private void BindingFormatForExcel(ExcelWorksheet workSheet, IEnumerable<T> records)
        {
            // Lấy ra các property có attribute name là ExcelColumnNameAttribute 
            var excelColumnProperties = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(ExcelColumnNameAttribute), true).Length > 0);

            // Lấy ra tên column cuối cùng (tính cả số thứ tự)
            var lastColumnName = (char)('A' + (excelColumnProperties.Count() + 1));

            // Tạo phần tiêu đề cho file excel
            using (var range = workSheet.Cells[$"A1:{lastColumnName}1"])
            {
                range.Merge = true;
                range.Style.Font.Bold = true;
                range.Style.Font.Size = 16;
                range.Style.Font.Name = "Arial";
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Value = "DANH SÁCH TÀI SẢN";
            }
            // Gộp các ô từ A2 đến ô cuối cùng của dòng 2
            workSheet.Cells[$"A2:{lastColumnName}2"].Merge = true;

            // Style chung cho tất cả bảng
            using (var range = workSheet.Cells[$"A3:{lastColumnName}{records.Count() + 3}"])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Font.Name = "Times New Roman";
                range.Style.Font.Size = 11;
                range.AutoFitColumns();
            }

            // Lấy ra các property có attribute name là ExcelColumnNameAttribute và đổ vào header của table
            int columnIndex = 1;
            workSheet.Cells[3, columnIndex].Value = "Số thứ tự";
            columnIndex++;
            foreach (var property in excelColumnProperties)
            {
                var excelColumnName = (property.GetCustomAttributes(typeof(ExcelColumnNameAttribute), true)[0] as ExcelColumnNameAttribute).ColumnName;
                workSheet.Cells[3, columnIndex].Value = excelColumnName;
                columnIndex++;
            }
            workSheet.Cells[3, columnIndex].Value = "Giá trị còn lại";
            columnIndex++;

            // Style cho header của table
            using (var range = workSheet.Cells[$"A3:{lastColumnName}3"])
            {
                range.Style.Font.Bold = true;
                range.Style.Font.Size = 10;
                range.Style.Font.Name = "Arial";
                range.Style.Font.Color.SetColor(Color.Black);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            // Đổ dữ liệu từ list nhân viên vào các côt tương ứng
            int rowIndex = 4;
            int stt = 1; // Số thứ tự
            float cost = 0;
            float depreciation_year = 0;
            foreach (var record in records)
            {
                columnIndex = 1;
                workSheet.Cells[rowIndex, columnIndex].Value = stt;
                columnIndex++;
                foreach (var property in excelColumnProperties)
                {
                    // Lấy ra giá trị của property
                    var propertyValue = property.GetValue(record);
                    // Trả về đối số kiểu cơ bản của kiểu nullable đã chỉ định.
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    var value = "";
                    switch (propertyType.Name)
                    {
                        case "DateTime":
                            value = (propertyValue as DateTime?)?.ToString("dd/MM/yyyy"); // Định dạng ngày tháng
                            workSheet.Cells[rowIndex, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            break;
                        default:
                            value = propertyValue?.ToString();
                            break;
                    }
                    if (columnIndex == 6)
                    {
                        cost = (float)(decimal)propertyValue;
                    }
                    if (columnIndex == 8)
                    {
                        depreciation_year = (float)(decimal)propertyValue;
                    }
                    // Đổ dữ liệu vào cột
                    workSheet.Cells[rowIndex, columnIndex].Value = value;
                    workSheet.Column(columnIndex).AutoFit();
                    columnIndex++;
                }
                workSheet.Cells[rowIndex, columnIndex].Value = (cost - depreciation_year).ToString();
                columnIndex++;
                rowIndex++;
                stt++;
            }
        }


        #region API Excel
        /// <summary>
        /// Xuất file excel danh sách bản ghi
        /// </summary>
        /// <returns>Đối tượng Stream chứa file excel</returns>
        /// Author: NNNINH (26/11/2022)
        public Stream ExportExcel(IEnumerable<T> record)
        {
            var employees = _baseDL.GetAllRecords();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var stream = new MemoryStream();
            var package = new ExcelPackage(stream);
            var workSheet = package.Workbook.Worksheets.Add("Danh sách tài sản");
            package.Workbook.Properties.Author = "Nguyễn Nghĩa Ninh";
            package.Workbook.Properties.Title = "Danh sách tài sản";
            BindingFormatForExcel(workSheet, record);
            package.Save();
            stream.Position = 0; // Đặt con trỏ về đầu file để đọc
            return package.Stream;
        }
        #endregion

        #region API NextCode
        /// <summary>
        /// Sinh mã tài sản tiếp theo
        /// </summary>
        /// <returns>Mã tài sản tiếp theo</returns>
        /// Cretaed by: NNNINH (09/11/2022) 
        public string NextAssetCode()
        {
            return _baseDL.GetNextAssetCode();
        }
        #endregion
    }
}
