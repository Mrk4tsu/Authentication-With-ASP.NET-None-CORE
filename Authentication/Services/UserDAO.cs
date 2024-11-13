using Authentication.Models;
using Authentication.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Authentication.Services
{
    public class UserDAO : BaseDAO
    {
        private SimpleHash simpleHash = SimpleHash.GetInstance();
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
        #endregion
    }
}