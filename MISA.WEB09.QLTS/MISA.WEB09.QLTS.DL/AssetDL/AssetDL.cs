using Dapper;
using MISA.WEB09.QLTS.Common.Entities;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.DL
{
    public class AssetDL :BaseDL<Asset>, IAssetDL
    {
        /// <summary>
        /// Sinh mã tài sản tiếp theo
        /// </summary>
        /// <returns>Mã tài sản tiếp theo</returns>
        /// Cretaed by: NNNINH (09/11/2022)
        public string NextAssetCode()
        {
            // Khai báo tên prodecure Insert
            string storedProcedureName = "Proc_asset_GetNextCode";

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            string nextAssetCode = "";
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                nextAssetCode = mysqlConnection.QueryFirstOrDefault<string>(storedProcedureName, commandType: System.Data.CommandType.StoredProcedure);
            }
            // Xử lý dữ liệu trả về
            if (nextAssetCode != null)
            {
                return nextAssetCode;
            }
            else
            {
                return "";
            }
        }
    }
}
