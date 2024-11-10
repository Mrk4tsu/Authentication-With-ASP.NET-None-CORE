using Authentication.Models;
using Authentication.Utilities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Authentication.Services
{
    public class UserDAO : BaseDAO
    {
        private SimpleHash simpleHash = SimpleHash.GetInstance();
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
        public async Task<Users> GetUsersAsync(int id)
        {
            var model = await db.Users.SingleOrDefaultAsync(u => u.Id == id);
            return model;
        }
        #endregion
        public int Login(string username, string password, bool remember)
        {
            var user = GetUserReadOnly(username);
            if (user != null)
            {
                if (!user.Status)
                    return -2;//Tài khoản đang bị khóa
                if (SimpleHash.GetInstance().Hash(password) != user.Password)
                    return -3;//Mật khẩu không chính xác
                return 1;
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
            model.UserCODE = GenerateUserCODE();
            model.CreatedDate = DateTime.UtcNow;
            model.ModifiedDate = DateTime.UtcNow;
            model.Status = true;
            model.IsVerifedEmail = false;
            db.Users.Add(model);
            await db.SaveChangesAsync().ConfigureAwait(false);

            var verifyUrl = $"/active/{simpleHash.Hash(model.Username)}";
            var link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, verifyUrl);

            string subject = $"Xác minh địa chỉ Email tài khoản!";
            string body = EmailCommon.GetInstance().BodyWelcomeEmail(link, model.UserCODE, model.Username);
            await EmailCommon.GetInstance().SendEmail(model, subject, body);
            return model.Id;
        }
        public async Task VerifyEmail(Users users)
        {
            users.IsVerifedEmail = true;

            db.Entry(users).State = EntityState.Modified;
            await db.SaveChangesAsync().ConfigureAwait(false);
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