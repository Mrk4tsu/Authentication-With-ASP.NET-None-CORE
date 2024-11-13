using Authentication.Models;
using Authentication.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using UAParser;

namespace Authentication.Services
{
    public class UserDeviceDAO : BaseDAO
    {
        public string IpAddress { get; set; }
        public string DeviceName { get; set; }
        private DeviceVerificationTokenDAO tokenDAO = new DeviceVerificationTokenDAO();
        private AuthsDbContext db = null;
        
        public UserDeviceDAO()
        {
            db = new AuthsDbContext();
            this.DeviceName = GetDeviceName();
            this.IpAddress = GetPublicIP();
        }
        public async Task<UserDevice> GetDeviceAsync(int deviceId)
        {
            var model = await db.UserDevice.AsNoTracking().Include(u => u.Users).SingleOrDefaultAsync(ud => ud.Id == deviceId);
            return model;
        }
        public async Task<UserDevice> GetExistedDeviceAsync(int userId, string deviceName, string ipAddress)
        {
            var existedDevice = await db.UserDevice.FirstOrDefaultAsync(u => u.UserId == userId && u.IpAddress == ipAddress && u.DeviceName == deviceName);
            return existedDevice;
        }
        public async Task<bool> SaveUserDeviceAsync(Users user)
        {
            var eC = EmailCommon.GetInstance();
            //Gửi OTP                  
            string OTP = await tokenDAO.GenerationOTP(user);
            string subject = "Xác thực thiết bị";
            string body = $"Phát hiện lượt đăng nhập lạ tại: {this.IpAddress}. Mã OTP xác thực lượt đăng nhập của bạn là: {OTP}";           

            
            //Đã login thiết bị từ trước đó hay chưa
            var existedDevice = await GetExistedDeviceAsync(user.Id, this.DeviceName, this.IpAddress);
            if (existedDevice != null)
            {
                if (existedDevice.IsTrusted)
                {
                    existedDevice.LastLogin = DateTime.UtcNow;
                    db.Entry(existedDevice).State = EntityState.Modified;
                    await db.SaveChangesAsync().ConfigureAwait(false); // Lưu các thay đổi vào cơ sở dữ liệu

                    return true; //Đã xác thực thiết bị
                }
                else
                {
                    existedDevice.LastLogin = DateTime.UtcNow;
                    db.Entry(existedDevice).State = EntityState.Modified;
                    await db.SaveChangesAsync().ConfigureAwait(false); // Lưu các thay đổi vào cơ sở dữ liệu

                    await eC.SendEmail(user, subject, body);
                    return false;//Chưa xác thực
                }
            }
            else
            {
                var newUserDevice = new UserDevice
                {
                    UserId = user.Id,
                    DeviceName = this.DeviceName,
                    IpAddress = this.IpAddress,
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
        public string GetPublicIP()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Gọi API để lấy địa chỉ IP công khai
                    HttpResponseMessage response = client.GetAsync("https://api.ipify.org").Result;
                    response.EnsureSuccessStatusCode();

                    string ipAddress = response.Content.ReadAsStringAsync().Result;
                    return ipAddress;
                }
                catch
                {
                    return null;
                }
            }
        }
        public string GetDeviceName()
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