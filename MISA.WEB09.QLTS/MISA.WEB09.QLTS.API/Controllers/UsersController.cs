using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MISA.WEB09.QLTS.BL;
using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.Common.Enums;
using MISA.WEB09.QLTS.Common.Resources;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MISA.WEB09.QLTS.API.Controllers
{
    public class UsersController : BasesController<User>
    {
        #region Field
        private IUserBL _userBL;
        private IConfiguration _config;

        #endregion
        public UsersController(IConfiguration config, IUserBL userBL) : base(userBL)
        {
            _userBL = userBL;
            _config = config;

        }

        [HttpGet("LogOut")]
        public async Task<IActionResult> LoginOut ()
        {
            var user = new User();
             await HttpContext.SignOutAsync(
                       CookieAuthenticationDefaults.AuthenticationScheme);
            return StatusCode(StatusCodes.Status200OK,user);
        }



        /// <summary>
        /// Lấy người dùng bằng tên đăng nhập và mật khẩu
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Người dùng hợp lệ</returns>
        [HttpGet("signin/{username}/{password}")]
        public async Task<IActionResult> GetByUsernamePassword([FromRoute] string username, [FromRoute] string password)
        {
            try
            {
                var passwordHash = MD5Hash(password);
                User user = _userBL.GetByUsernamePassword(username, passwordHash);
              
                // Xử lý dữ liệu trả về
                if (user.user_id != Guid.Empty)
                {
                    user.Token = GenerateToken(user);          
                    return StatusCode(StatusCodes.Status200OK, user);
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResult(
                        ErrorCode.LoginFailed,
                        Resource.DevMsg_LoginFailed,
                        Resource.UserMsg_LoginFailed,
                        username + "/" + password,
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
        /// Lấy mã token cho user 
        /// </summary>
        /// <param name="User">Đối tượng đăng nhập</param>
        /// <returns>Mã token</returns>
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name ,user.username),
                new Claim("password",user.password),
                new Claim(ClaimTypes.Role, "Administrator"),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Chuyển mật khẩu thành mã hashMD5  
        /// </summary>
        /// <param name="text">Mật khẩu đăng nhập</param>
        /// <returns>Mã MD5Hash</returns>
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}
