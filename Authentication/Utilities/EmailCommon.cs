using Authentication.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;


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
            var email = new MimeMessage();
            var fromAddress = new MailboxAddress("Mrkatsu", "yourmail@gmail.com");
            var toAddress = new MailboxAddress(users.FullName, users.Email);
            email.From.Add(fromAddress);
            email.To.Add(toAddress);

            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };
            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Note: Mật khẩu ứng dụng là mật khẩu thay thế sử dụng mật khẩu chính của Gmail
                // (Không thể sử dụng mật khẩu chính thức của Gmail)
                await smtp.AuthenticateAsync("yourmail@gmail.com", "Mật khẩu ứng dụng");

                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
        }
       
        public string BodyWelcomeEmail(string url, string code, string user)
        {
            string logoUrl = "https://raw.githubusercontent.com/Mrk4tsu/Mrk4tsu/refs/heads/main/assets/MRKATSU.png";
            string html = $@"<div style=""display:flex; justify-content:center"">
                                <div style=""width: fit-content; border-radius:15px; font-family: 'Montserrat Medium', sans-serif; background: rgba(39, 61, 82); text-align: center; padding: 10px 40px; "">
                                    <div style="" margin: 20px 0px;"">
                                        <a href='https://github.com/Mrk4tsu'><img style=""width: 250px;"" src='{logoUrl}' /></a>
                                        <p style=""font-size: 30px; color: #fff;"">Chào mừng đến với <span style=""color: #28a745; font-weight: bold;"">MrKatsu</span></p>                                     
                                        <p style=""font-size: 16px; color: #fff;"">Xin chào {user}, đây là thư dùng 1 lần để thực hiện xác minh tài khoản. Vui lòng lưu trữ mã xác thực người dùng, mọi quyền sử dụng tài khoản sẽ phụ thuộc vào <b>MÃ XÁC THỰC NGƯỜI DÙNG</b> này.</p>
                                        <p style=""font-size: 30px; color: #fff;"">Mã xác thực người dùng: </p>
                                        <a style=""background: rgba(39, 101, 112) ;font-size:13px; color: #fff; padding: 10px 15px; margin-bottom:15px; border-radius: 15px; text-decoration: none; "">{code}</a>
                                        <p style=""color:orangered; font-style:italic; margin-top: 15px;"">Vui lòng không cung cấp mã tránh mất tài khoản!</p>
                                        <a style=""background: #28a745; color: #fff; padding: 10px 20px; border-radius: 5px; text-decoration: none; "" href='{url}'>Xác minh email</a>
                                        
                                    </div>
                                </div>
                            </div>";
            return html;
        }
        
    }
}