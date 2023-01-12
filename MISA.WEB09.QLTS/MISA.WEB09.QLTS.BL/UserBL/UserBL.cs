using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.BL
{
    public class UserBL : BaseBL<User>, IUserBL
    {
        #region Field

        private IUserDL _userDL;

        #endregion
        public UserBL(IUserDL userDL) : base(userDL)
        {
            _userDL = userDL;
        }

        #region Method
        /// <summary>
        /// Lấy người dùng bằng tên đăng nhập và mật khẩu
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Người dùng hợp lệ</returns>
        public User GetByUsernamePassword(string username, string password)
        {
            return _userDL.GetByUsernamePassword(username, password);
        } 
        #endregion
    }
}
