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
        public async Task SaveUserDeviceAsync(Users user)
        {
            var eC = EmailCommon.GetInstance();

            string ipAddress = await GetPublicIPAsync();
            string deviceName = GetDeviceName();
            //Đã login thiết bị từ trước đó hay chưa
            var existedDevice = await db.UserDevice.FirstOrDefaultAsync(u => u.UserId == user.Id && u.IpAddress == ipAddress);
            if (existedDevice != null)
            {
                if (existedDevice.IsTrusted)
                {
                    existedDevice.LastLogin = DateTime.UtcNow;
                    db.Entry(existedDevice).State = EntityState.Modified;
                }
                else
                {
                    //Gửi OTP
                    //string subject = "Xác thực thiết bị";
                    //string body = "Mã OTP của bạn là: " + RandomNumber(6);
                    //await eC.SendEmail(user, subject, body);
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