using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.Common.Entities
{
    public class User
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public Guid user_id { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// Thành phố
        /// </summary>
        public string? city { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string? unit { get; set; }

        /// <summary>
        /// Mã Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Thông điệp
        /// </summary>
        public string? Message { get; set; }
    }
}
