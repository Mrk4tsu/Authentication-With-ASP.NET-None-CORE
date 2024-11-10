using Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace Authentication.Utilities
{
    public class EmailCommon
    {
        private static EmailCommon instance;
        public static EmailCommon GetInstance()
        {
            if (instance == null)
            {
                instance = new EmailCommon();
            }
            return instance;
        }
        public async Task SendEmail(Users users, string subject, string body)
        {
            var fromEmail = new MailAddress("thang.ndu.63cntt@ntu.edu.vn", "MrKatsu");
            var toEmail = new MailAddress(users.Email);

            var fromEmailPassword = "Thangkatsu@2104";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
        public string BodyWelcomeEmail(string url, string code, string user)
        {
            string logoUrl = "https://raw.githubusercontent.com/Mrk4tsu/Mrk4tsu/refs/heads/main/assets/MRKATSU.png";
            string html = $@"<div style=""display:flex; justify-content:center"">
                                <div style=""width: fit-content; border-radius:15px; font-family: 'Montserrat Medium', sans-serif; background: rgba(39, 61, 82); text-align: center; padding: 10px 40px; "">
                                    <div style="" margin: 20px 0px;"">
                                        <img style=""width: 250px;"" src='{logoUrl}' />
                                        <p style=""font-size: 30px; color: #fff;"">Chào mừng {user.ToUpper()} đến với <span style=""color: #28a745; font-weight: bold;"">MrKatsu</span></p>
                                        <p style=""font-size: 16px; color: #fff;"">Đây là thư dùng 1 lần để thực hiện xác minh tài khoản.</p>
                                        <a style=""background: #28a745; color: #fff; padding: 10px 20px; border-radius: 5px; text-decoration: none; "" href='{url}'>Xác minh tài khoản</a>
                                        <p style=""font-size: 30px; color: #fff;"">Mã xác thực người dùng: </p>
                                        <a style=""background: rgba(39, 101, 112) ;font-size:13px; color: #fff; padding: 10px 15px; margin-bottom:15px; border-radius: 15px; text-decoration: none; "">{code}</a>
                                        <p style=""color:orangered; font-style:italic; margin-top: 15px;"">Vui lòng không cung cấp mã tránh mất tài khoản!</p>
                                    </div>
                                </div>
                            </div>";
            return html;
        }
    }
}