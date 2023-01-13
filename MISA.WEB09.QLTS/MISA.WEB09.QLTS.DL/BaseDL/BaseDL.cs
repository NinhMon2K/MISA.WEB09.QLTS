using Dapper;
using MISA.WEB09.QLTS.Common.Attributes;
using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.Common.Resources;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MISA.WEB09.QLTS.DL
{
    public class BaseDL<T> : IBaseDL<T>
    {
        /// <summary>
        /// Lấy danh sách toàn bộ bản ghi
        /// </summary>
        /// <returns>Danh sách toàn bộ bản ghi</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        public IEnumerable<T> GetAllRecords()
        {
            // Khai báo tên stored procedure
            string storedProcedureName = String.Format(Resource.Proc_GetAll, typeof(T).Name);

            // Khởi tạo kết nối tới DB MySQL
            var records = new List<T>();
            using (var mysqlConnection = new MySqlConnection(DataContext.MySqlConnectionString))
            {
                // Thực hiện gọi vào DB
                records = (List<T>)mysqlConnection.Query<T>(
                   storedProcedureName,
                   commandType: System.Data.CommandType.StoredProcedure);
            }
            return records;
        }

        /// <summary>
        /// Lấy 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID của bản ghi cần lấy</param>
        /// <returns>Bản ghi có ID được truyền vào</returns>
        /// Created by:  NNNINH (10/11/2022)
        public T GetRecordById(Guid recordId)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    parameters.Add($"v_{property.Name}", recordId);
                    break;
                }
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            T record;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure Insert
                string storedProcedureName = String.Format(Resource.Proc_Select, typeof(T).Name);

                // Thực hiện gọi vào DB để chạy procedure
                record = mysqlConnection.QueryFirstOrDefault<T>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            return record;
        }

        /// <summary>
        /// Thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Đối tượng bản ghi cần thêm mới</param>
        /// <returns>ID của bản ghi vừa thêm. Return về Guid rỗng nếu thêm mới thất bại</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        public Guid InsertRecord(T record, Guid recordId)
        {
            DynamicParameters parameters = PramatersProperties(recordId, record);

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int numberOfAffectedRows = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure Insert
                string storedProcedureName = String.Format(Resource.Proc_Add, typeof(T).Name);

                // Thực hiện gọi vào DB để chạy procedure
                numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }

            // Xử lý dữ liệu trả về
            if (numberOfAffectedRows > 0)
            {
                return recordId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Cập nhật 1 bản ghi
        /// </summary>
        /// <param name="recordId">ID bản ghi cần cập nhật</param>
        /// <param name="record">Đối tượng cần cập nhật theo</param>
        /// <returns>ID của bản ghi sau khi cập nhật. Return về Guid rỗng nếu cập nhật thất bại</returns>
        /// Cretaed by: NNNINH (11/11/2022)
        public Guid UpdateRecord(Guid recordId, T record)
        {
            DynamicParameters parameters = PramatersProperties(recordId, record);

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int numberOfAffectedRows = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure Insert
                string storedProcedureName = String.Format(Resource.Proc_Update, typeof(T).Name);

                // Thực hiện gọi vào DB để chạy procedure
                numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            // Xử lý dữ liệu trả về
            if (numberOfAffectedRows > 0)
            {
                return recordId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Chuẩn bị đầu vào cho proceduce
        /// </summary>
        /// <param name="recordId">ID bản ghi</param>
        /// <param name="record">Đối tượng lấy các biến</param>
        /// <returns>ID bản ghi vừa xóa</returns>
        /// Cretaed by: NNNINH (11/01/2023)
        public static DynamicParameters PramatersProperties(Guid recordId, T record)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object propertyValue;
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    propertyValue = recordId;
                }
                else
                {
                    propertyValue = property.GetValue(record, null);
                }
                parameters.Add($"v_{propertyName}", propertyValue);
            }

            return parameters;
        }

        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">ID bản ghi cần xóa</param>
        /// <returns>ID bản ghi vừa xóa</returns>
        /// Cretaed by: NNNINH (11/11/2022)
        public Guid DeleteRecord(Guid recordId)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    parameters.Add($"v_{property.Name}", recordId);
                    break;
                }
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int numberOfAffectedRows = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure Insert
                string storedProcedureName = String.Format(Resource.Proc_Delete, typeof(T).Name);

                // Thực hiện gọi vào DB để chạy procedure
                numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            // Xử lý dữ liệu trả về
            if (numberOfAffectedRows > 0)
            {
                return recordId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="recordIdList">Danh sách ID các bản ghi cần xóa</param>
        /// <returns>Danh sách ID các bản ghi đã xóa</returns>
        /// Cretaed by:  NNNINH (11/11/2022)
        public int DeleteMultiRecords(List<string> recordIdList)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var properties = typeof(T).GetProperties();
            var propertyName = "";
            foreach (var property in properties)
            {
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    propertyName = property.Name;
                    break;
                }
            }

            // Khởi tạo kết nối tới DB MySQL
            int numberOfAffectedRows = 0;
            string connectionString = DataContext.MySqlConnectionString;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure
                string storedProcedureName = String.Format(Resource.Proc_BatchDelete, typeof(T).Name);

                mysqlConnection.Open();

                // Bắt đầu transaction.
                using (var transaction = mysqlConnection.BeginTransaction())
                {
                    try
                    {
                        // Chuyển đổi list sang json
                        var listDatas = JsonSerializer.Serialize(recordIdList);
                        // Chuẩn bị tham số đầu vào
                        var parameters = new DynamicParameters();
                        parameters.Add($"v_{propertyName}s", listDatas);
                        numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);

                        if (numberOfAffectedRows == recordIdList.Count)
                        {
                            transaction.Commit();

                            return numberOfAffectedRows;
                        }
                        else
                        {
                            transaction.Rollback();
                            numberOfAffectedRows = 0;
                            return numberOfAffectedRows;
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        mysqlConnection.Close();
                    }
                    finally
                    {
                        mysqlConnection.Close();
                    }
                }
            }
            return numberOfAffectedRows;
        }

        /// <summary>
        /// Kiểm tra trùng mã bản ghi
        /// </summary>
        /// <param name="recordCode">Mã cần xét trùng</param>
        /// <param name="recordId">Id bản ghi đưa vào (nếu là sửa)</param>
        /// <returns>Số lượng mã tài sản bị trùng</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        public int DuplicateRecordCode(object recordCode, Guid recordId)
        {
            // Khai báo tên prodecure
            string storedProcedureName = String.Format(Resource.Proc_DuplicateCode, typeof(T).Name);

            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var isNotDuplicateAttribute = (IsNotDuplicateAttribute)Attribute.GetCustomAttribute(property, typeof(IsNotDuplicateAttribute));
                if (isNotDuplicateAttribute != null)
                {
                    parameters.Add($"v_{property.Name}", recordCode);
                }

                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    parameters.Add($"v_{property.Name}", recordId);
                }
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int duplicates = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                duplicates = mysqlConnection.QueryFirstOrDefault<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }

            return duplicates;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PagingData<T> FilterRecords(string? keyword, string type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sinh mã tài sản tiếp theo
        /// </summary>
        /// <returns>Mã tài sản tiếp theo</returns>
        /// Cretaed by: NNNINH (09/11/2022)
        public string GetNextAssetCode()
        {
            // Khai báo tên prodecure
            string storedProcedureName = String.Format(Resource.Proc_GetLastsCode, typeof(T).Name);

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            string nextAssetCode = "";
            var recordsCode = "";
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                recordsCode = mysqlConnection.QueryFirstOrDefault<string>(storedProcedureName, commandType: System.Data.CommandType.StoredProcedure);
                //Xử lý kết quả trả về
                string lastNum = Regex.Match(recordsCode, @"(\d+)(?!.*\d)").Value;
                int lenOfLastNum = lastNum.Length; // chiều dài mã số

                // Mã không có bất kỳ ký tự số nào
                if (lastNum == "") return recordsCode + '1';

                int numVal = Int32.Parse(lastNum); // Ép sang kiểu số
                int lastIndex = recordsCode.LastIndexOf(lastNum); // Chỉ số xuất hiện "lastNum" tính từ cuối lên

                // Tạo mã số mới
                string newNumCode = (numVal + 1).ToString(); // mã số mới = mã số cũ + 1
                int lenOfNewNumCode = newNumCode.Length; // Lấy chiều dài mã số mới
                // Tạo mã số mới hoàn chỉnh
                if (lenOfNewNumCode < lenOfLastNum) // Ex: old code: 00023, new code: 24 => new code: '000' + '24'
                    newNumCode = recordsCode.Substring(lastIndex, lenOfLastNum - lenOfNewNumCode) + newNumCode;

                // xử lý kết quả đầu ra
                // Xóa mã số cũ
                recordsCode = recordsCode.Remove(lastIndex, lenOfLastNum);
                // Insert mã số mới
                recordsCode = recordsCode.Insert(lastIndex, newNumCode);
            }
            // Xử lý dữ liệu trả về
            if (recordsCode != null)
            {
                return recordsCode;
            }
            else
            {
                return "";
            }
        }
    }
}
