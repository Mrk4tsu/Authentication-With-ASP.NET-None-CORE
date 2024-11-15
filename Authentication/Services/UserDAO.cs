using Authentication.Models;
using Authentication.Utilities;
using Authentication.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Services.Description;

namespace Authentication.Services
{
    public class UserDAO : BaseDAO
    {
        private SimpleHash simpleHash = SimpleHash.GetInstance();
        private EmailCommon emailCommon = EmailCommon.GetInstance();
        private UserDeviceDAO userDevice = new UserDeviceDAO();
        private AuthsDbContext db;
        public UserDAO()
        {
            db = new AuthsDbContext();
        }
        #region[Get User]
        public Users GetUserReadOnly(string username)
        {
            var model = db.Users.AsNoTracking().SingleOrDefault(u => u.Username == username);
            return model;
        }
        public async Task<Users> GetUserByCodeReadOnly(string code)
        {
            var model = await db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.UserCODE == code);
            return model;
        }
        public async Task<List<Users>> GetListUsersAsync()
        {
            var model = await db.Users.AsNoTracking().ToListAsync();
            return model;
        }
        public async Task<Users> GetUsersAsync(int id)
        {
            var model = await db.Users.SingleOrDefaultAsync(u => u.Id == id);
            return model;
        }
        #endregion
        public async Task<int> Login(string username, string password, bool remember)
        {
            var user = GetUserReadOnly(username);
            if (user != null)
            {
                if (!user.Status)
                    return -2;//Tài khoản đang bị khóa
                if (SimpleHash.GetInstance().Hash(password) != user.Password)
                    return -3;//Mật khẩu không chính xác
                else
                {
                    bool active = await userDevice.SaveUserDeviceAsync(user);
                    if (active)
                    {
                        return 1;
                    }
                    return -4;
                }
            }
            //Không có tài khoản trong databse
            return -1;
        }
        public async Task<int> InsertUserAsync(Users model)
        {
            if (await IsExistingAccount(model.Username))
            {
                return -1;
            }
            if (await IsExistingEmail(model.Email))
            {
                return -2;
            }
            model.CreatedDate = DateTime.UtcNow;
            model.ModifiedDate = DateTime.UtcNow;
            model.Status = true;
            model.IsVerifedEmail = false;
            db.Users.Add(model);
            await db.SaveChangesAsync().ConfigureAwait(false);

            return model.Id;
        }
        public async Task<bool> VerifyEmail(Users users)
        {
            if (!users.IsVerifedEmail)
            {
                users.IsVerifedEmail = true;

                db.Entry(users).State = EntityState.Modified;
                await db.SaveChangesAsync().ConfigureAwait(false);
            }
            return false;
        }
        public async Task<bool> VerifyToken(int userId, string inputToken)
        {
            var deviceVerification = await db.DeviceVerificationToken
                .OrderByDescending(dv => dv.CreateTime)
                .FirstOrDefaultAsync(dv => dv.UserId == userId && dv.Status == true);

            

            if (deviceVerification != null && DateTime.UtcNow <= deviceVerification.ExpiredTime)
            {
                // Kiểm tra Token và thời hạn
                if (deviceVerification.Token == inputToken)
                {
                    deviceVerification.Status = false; // Đánh dấu Token đã sử dụng
                    db.Entry(deviceVerification).State = EntityState.Modified;

                    // Truy vấn UserDevice dựa trên UserId và DeviceName hoặc IpAddress
                    var u = await db.UserDevice
                        .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DeviceName == userDevice.DeviceName && ud.IpAddress == userDevice.IpAddress);

                    if (u != null)
                    {
                        u.IsTrusted = true; // Đặt IsTrusted thành true
                        db.Entry(u).State = EntityState.Modified;
                    }

                    await db.SaveChangesAsync().ConfigureAwait(false);
                    return true; // Token hợp lệ
                }
                else
                    return false; // Token không hợp lệ
            }

            return false; // Token không hợp lệ hoặc đã hết hạn
        }
        public async Task<bool> RequestResetCode(string email)
        {
            var account = await db.Users.Where(a => a.Email == email).FirstOrDefaultAsync();
            if (account != null)
            {
                string resetCode = Guid.NewGuid().ToString();

                var verifyUrl = "/Auths/ResetPassword/" + resetCode;
                var link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, verifyUrl);
                string body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
    "<br/><br/><a href=" + link + ">Reset Password link</a>";
                await emailCommon.SendEmail(account, "Reset Password", body);
                account.CodeResetPassword = resetCode;

                db.Configuration.ValidateOnSaveEnabled = false;
                await db.SaveChangesAsync();

                return true;
            }
            return false;
        }
        public async Task<ResetPasswordModel> GetResetPasswordModelAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var user = await db.Users.AsNoTracking().Where(a => a.CodeResetPassword == id).FirstOrDefaultAsync();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel
                {
                    ResetCode = id
                };
                return model;
            }
            return null;
        }
        public async Task<bool> ConfirmResetPassword(ResetPasswordModel model)
        {
            var user = await db.Users.Where(a => a.CodeResetPassword == model.ResetCode).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Password = simpleHash.Hash(model.NewPassword);
                user.CodeResetPassword = "";
                db.Configuration.ValidateOnSaveEnabled = false;
                await db.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            return false;
        }
        #region[Check]
        public async Task<bool> IsExistingAccount(string username)
        {
            return await db.Users
                .AsNoTracking()
                .Where(a => a.Username == username)
                .Select(a => a.Id)
                .AnyAsync();
        }
        public async Task<bool> IsExistingEmail(string mail)
        {
            return await db.Users
                .AsNoTracking()
                .Where(a => a.Email == mail)
                //.Select(a => a.Id)
                .AnyAsync();
        }

        internal object GetUserByEmail(string emailID)
        {
            var account = db.Users.Where(a => a.Email == emailID).FirstOrDefault();
            return account;
        }
        #endregion
    }
}