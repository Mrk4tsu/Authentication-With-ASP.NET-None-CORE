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

            var fromEmailPassword = "******";
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
    }
}