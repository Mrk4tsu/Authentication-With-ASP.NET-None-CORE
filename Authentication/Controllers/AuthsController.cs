using Authentication.Models;
using Authentication.Services;
using Authentication.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Authentication.Controllers
{
    public class AuthsController : Controller
    {
        private UserDAO userDAO = new UserDAO();
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
                    Username = model.Username,
                    Password = model.Password,
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
    }
}