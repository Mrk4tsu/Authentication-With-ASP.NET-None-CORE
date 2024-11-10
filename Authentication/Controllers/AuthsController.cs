using Authentication.Models;
using Authentication.Services;
using Authentication.Utilities;
using Authentication.ViewModels;
using System;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    Username = model.Username.ToLowerInvariant(),
                    Password = simpleHash.Hash(model.Password),
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone
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
        public ActionResult Login(LoginViewModel model)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                var result = userDAO.Login(model.Username, model.Password, model.RememberMe);
                switch (result)
                {
                    case 1:
                        //Nếu người dùng tick ghi nhớ đăng nhập thì sẽ lưu cookies 30 ngày, ngx lại là 3 phút
                        var expirationTIme = model.RememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddMinutes(3);
                        var authTicket = new FormsAuthenticationTicket(
                                version: 1,
                                name: model.Username,
                                issueDate: DateTime.Now,
                                expiration: expirationTIme,
                                isPersistent: model.RememberMe,
                                userData: model.Username
                            );
                        string enCryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, enCryptedTicket);
                        Response.Cookies.Add(authCookie);

                        return RedirectToAction("Index", "Home");
                    case -2:
                        ModelState.AddModelError("", "Tài khoản đang bị khóa");
                        message = "Tài khoản đang bị khóa";
                        break;
                    case -3:
                        ModelState.AddModelError("", "Mật khẩu không chính xác");
                        message = "Mật khẩu không chính xác";
                        break;
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
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Auths");
        }
    }
}