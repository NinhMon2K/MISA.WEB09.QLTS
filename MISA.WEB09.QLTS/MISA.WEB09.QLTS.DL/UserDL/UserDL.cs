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
    public class UserDL : BaseDL<User>, IUserDL
    {
        /// <summary>
        /// Lấy người dùng bằng tên đăng nhập và mật khẩu
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Người dùng hợp lệ</returns>
        public User GetByUsernamePassword(string username, string password)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            parameters.Add("v_username", username);
            parameters.Add("v_password", password);

            // Khai báo tên prodecure
            string storedProcedureName = "Proc_user_GetByUsernamePassword";

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            User user;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Thực hiện gọi vào DB để chạy procedure
                  user = mysqlConnection.QueryFirstOrDefault<User>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            if (user != null) return user;
            else return new User
            {
                user_id = Guid.Empty,
                username = username,
                password = password,
            };
        }
    }
}
