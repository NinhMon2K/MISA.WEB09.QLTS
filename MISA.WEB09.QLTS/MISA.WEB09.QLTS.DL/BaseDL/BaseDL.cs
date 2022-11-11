using Dapper;
using MISA.WEB09.QLTS.Common.Attributes;
using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.Common.Resources;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                 records =(List<T>) mysqlConnection.Query<T>(
                    storedProcedureName,
                    commandType: System.Data.CommandType.StoredProcedure);
            }
            return records;
        }


        public List<string> DeleteMultiRecords(List<string> recordIdList)
        {
            throw new NotImplementedException();
        }

        public Guid DeleteRecord(Guid recordId)
        {
            throw new NotImplementedException();
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

        public PagingData<T> FilterRecords(string? keyword, string type)
        {
            throw new NotImplementedException();
        }

        

        public T GetRecordById(Guid recordId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Đối tượng bản ghi cần thêm mới</param>
        /// <returns>ID của bản ghi vừa thêm. Return về Guid rỗng nếu thêm mới thất bại</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        public Guid InsertRecord(T record)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var newRecordID = Guid.NewGuid();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object propertyValue;
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    propertyValue = newRecordID;
                }
                else
                {
                    propertyValue = property.GetValue(record, null);
                }
                parameters.Add($"v_{propertyName}", propertyValue);
            }

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
                return newRecordID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Guid UpdateRecord(Guid recordId, T record)
        {
            throw new NotImplementedException();
        }
    }
}
