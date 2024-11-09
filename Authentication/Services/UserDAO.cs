using Authentication.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Services
{
    public class UserDAO : BaseDAO
    {
        private AuthsDbContext db;
        public UserDAO()
        {
            db = new AuthsDbContext();
        }
        #region[Get User]
        public Users GetUserReadOnly(string username)
        {
            var model = db.Users.AsNoTracking().SingleOrDefault(u => Lower(u.Username) == Lower(username));
            return model;
        }
        public async Task<Users> GetUsersAsync(int id)
        {
            var model = await db.Users.SingleOrDefaultAsync(u => u.Id == id);
            return model;
        }
        #endregion
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
            db.Users.Add(model);
            await db.SaveChangesAsync().ConfigureAwait(false);
            return model.Id;
        }
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
    }
}