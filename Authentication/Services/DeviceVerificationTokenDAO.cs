using Authentication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Authentication.Services
{
    public class DeviceVerificationTokenDAO : BaseDAO
    {
        private AuthsDbContext db = null;
        public DeviceVerificationTokenDAO()
        {
            db = new AuthsDbContext();
        }
        public async Task<string> GenerationOTP(Users users)
        {
            // Tìm mã OTP còn hiệu lực (nếu có) của người dùng
            var activeOtp = await db.DeviceVerificationToken.FirstOrDefaultAsync(o => o.UserId == users.Id && o.Status && o.ExpiredTime > DateTime.UtcNow);
            // Nếu có OTP cũ còn hiệu lực, hủy hiệu lực và xóa nó
            if (activeOtp != null)
            {
                db.DeviceVerificationToken.Remove(activeOtp); // Hoặc set IsActive = false nếu bạn muốn lưu lại bản ghi OTP cũ
                await db.SaveChangesAsync();
            }
            string otpCode = await RandomNumberAsync(6);
            var newOTP = new DeviceVerificationToken
            {
                UserId = users.Id,
                Token = otpCode,
                CreateTime = DateTime.UtcNow,
                ExpiredTime = DateTime.UtcNow.AddMinutes(10),
                Status = true //Mã trả về true còn hiệu lực, false đã hết hạn hoặc dùng rồi
            };

            db.DeviceVerificationToken.Add(newOTP);
            await db.SaveChangesAsync();

            return otpCode;
        }

    }
}