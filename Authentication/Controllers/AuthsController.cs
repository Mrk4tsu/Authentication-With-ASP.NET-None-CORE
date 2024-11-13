using Antlr.Runtime.Misc;
using Authentication.Models;
using Authentication.Services;
using Authentication.Utilities;
using Authentication.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Authentication.Controllers
{
    public class AuthsController : Controller
    {
        private UserDAO userDAO = new UserDAO();
        private SimpleHash simpleHash = SimpleHash.GetInstance();
        #region[Đăng ký]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        /*
         Xử lý logic:
            - Mail xác nhận đăng ký là mail duy nhất nhận được mã chính xác của người dùng
            - Mọi thông tin sau đó sẽ mã hóa, người dùng và chủ web cũng không thể biết được trừ khi có email
            - Suy ra email chứa mã xác nhận là quan trọng nhất
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userCode = userDAO.GenerateUserCODE();
                var user = new Users
                {
                    Username = model.Username.ToLowerInvariant(),
                    Password = simpleHash.Hash(model.Password),
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    UserCODE = simpleHash.Hash(userCode),
                };
                var result = await userDAO.InsertUserAsync(user);

                switch (result)
                {
                    case -1:
                        ModelState.AddModelError("", "Tài khoản đã tồn tại");
                        break;
                    case -2:
                        ModelState.AddModelError("", "Email đã tồn tại");
                        break;
                    default:
                        var verifyUrl = Url.Action("VerifyEmail", "Auths", new { code = user.UserCODE }, protocol: Request.Url.Scheme);


                        string subject = $"[Welcome] Xác minh địa chỉ Email tài khoản!";
                        string body = EmailCommon.GetInstance().BodyWelcomeEmail(verifyUrl, userCode, user.FullName);
                        await EmailCommon.GetInstance().SendEmail(user, subject, body);
                        return RedirectToAction("Login", "Auths");
                }

            }
            return View(model);
        }
        #endregion
        #region[Đăng nhập]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                var result = await userDAO.Login(model.Username, model.Password, model.RememberMe);
                switch (result)
                {
                    case 1:
                        //Nếu người dùng tick ghi nhớ đăng nhập thì sẽ lưu cookies 30 ngày, ngx lại là 3 phút
                        int expirationTime = model.RememberMe ? (int)(DateTime.Now.AddDays(30) - DateTime.Now).TotalMinutes : (int)(DateTime.Now.AddMinutes(3) - DateTime.Now).TotalMinutes;

                        var ticket = new FormsAuthenticationTicket(model.Username, model.RememberMe, expirationTime);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(expirationTime);
                        //Đặt HttpOnly = true ngăn cookie được truy cập bởi mã JavaScript
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        return RedirectToAction("Index", "Home");
                    case -2:
                        ModelState.AddModelError("", "Tài khoản đang bị khóa");
                        message = "Tài khoản đang bị khóa";
                        break;
                    case -3:
                        ModelState.AddModelError("", "Mật khẩu không chính xác");
                        message = "Mật khẩu không chính xác";
                        break;
                    case -4:
                        return RedirectToAction("ActiveDevice", "Auths");
                    default:
                        ModelState.AddModelError("", "Tài khoản không tồn tại");
                        message = "Tài khoản không tồn tại";
                        break;
                }
            }
            ViewBag.Message = message;
            return View(model);
        }
        #endregion
        [HttpGet]
        public ActionResult ActiveDevice()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ActiveDevice(string input)
        {
            return View(input);
        }
        public async Task<ActionResult> VerifyEmail(string code)
        {
            bool status = false;
            var user = await userDAO.GetUserByCodeReadOnly(code);
            if (user != null)
            {
                var result = await userDAO.VerifyEmail(user);
                if (result)
                {
                    status = true;
                }
                else
                    ViewBag.Message = "Yêu cầu đã được thực hiện!";
            }
            else
            {
                ViewBag.Message = "Yêu cầu không hợp lệ!";
            }

            ViewBag.Status = status;
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Auths");
        }
    }
}