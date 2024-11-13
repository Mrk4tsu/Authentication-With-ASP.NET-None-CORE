using Authentication.Models;
using Authentication.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using UAParser;

namespace Authentication.Services
{
    public class UserDeviceDAO : BaseDAO
    {
        private DeviceVerificationTokenDAO tokenDAO = new DeviceVerificationTokenDAO();
        private AuthsDbContext db = null;
        public UserDeviceDAO()
        {
            db = new AuthsDbContext();
        }
        public async Task<UserDevice> GetDeviceAsync(int deviceId)
        {
            var model = await db.UserDevice.AsNoTracking().Include(u => u.Users).SingleOrDefaultAsync(ud => ud.Id == deviceId);
            return model;
        }
        public async Task<bool> SaveUserDeviceAsync(Users user)
        {
            string ipAddress = await GetPublicIPAsync();
            string deviceName = GetDeviceName();

            var eC = EmailCommon.GetInstance();
            //Gửi OTP                  
            string OTP = await tokenDAO.GenerationOTP(user);
            string subject = "Xác thực thiết bị";
            string body = $"Phát hiện lượt đăng nhập lạ tại: {ipAddress}. Mã OTP xác thực lượt đăng nhập của bạn là: {OTP}";           

            
            //Đã login thiết bị từ trước đó hay chưa
            var existedDevice = await db.UserDevice.FirstOrDefaultAsync(u => u.UserId == user.Id && u.IpAddress == ipAddress);
            if (existedDevice != null)
            {
                if (existedDevice.IsTrusted)
                {
                    existedDevice.LastLogin = DateTime.UtcNow;
                    db.Entry(existedDevice).State = EntityState.Modified;
                    return true; //Đã xác thực thiết bị
                }
                else
                {
                    await eC.SendEmail(user, subject, body);
                    return false;//Chưa xác thực
                }
            }
            else
            {
                var newUserDevice = new UserDevice
                {
                    UserId = user.Id,
                    DeviceName = deviceName,
                    IpAddress = ipAddress,
                    FistLogin = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    IsTrusted = false
                };
                db.UserDevice.Add(newUserDevice);
                await db.SaveChangesAsync().ConfigureAwait(false);

                await eC.SendEmail(user, subject, body);
                return false;// Lần đàu đăng nhập
            }
        }
        public async Task<string> GetPublicIPAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Gọi API để lấy địa chỉ IP công khai
                    HttpResponseMessage response = await client.GetAsync("https://api.ipify.org");
                    response.EnsureSuccessStatusCode();

                    string ipAddress = await response.Content.ReadAsStringAsync();
                    return ipAddress;
                }
                catch
                {                   
                    return null;
                }
            }
        }
        private string GetDeviceName()
        {
            HttpBrowserCapabilities capability = HttpContext.Current.Request.Browser;
            if (capability.IsMobileDevice)
            {
                var uaParser = Parser.GetDefault();
                ClientInfo c = uaParser.Parse(HttpContext.Current.Request.UserAgent);
                return c.Device.Family;
            }
            else
            {
                string deviceName = Environment.MachineName;
                return deviceName;
            }
        }
    }
}