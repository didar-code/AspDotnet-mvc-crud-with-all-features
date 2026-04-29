using _1292886_MVC_CRUD_Project.DAL;
using _1292886_MVC_CRUD_Project.Helpers;
using _1292886_MVC_CRUD_Project.Models;
using _1292886_MVC_CRUD_Project.ViewModels;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace _1292886_MVC_CRUD_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Login(LoginVm vm, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(vm);
            }

            var user = db.TblUsers.FirstOrDefault(u => u.UserName == vm.UserName);
            if (user == null || !PasswordHelper.Verify(vm.Password, user.PasswordHash, user.PasswordSalt))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                ViewBag.ReturnUrl = returnUrl;
                return View(vm);
            }

            var roleName = db.TblRoles
                             .Where(r => r.Id == user.RoleId)
                             .Select(r => r.RoleName)
                             .FirstOrDefault() ?? "User";

            var ticket = new FormsAuthenticationTicket(
                1,
                user.UserName,
                DateTime.Now,
                DateTime.Now.AddMinutes(60),
                false,
                roleName
            );

            var encrypted = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                HttpOnly = true
            };
            Response.Cookies.Add(cookie);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Sales");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        private int EnsureRoleExists(string roleName)
        {
            var role = db.TblRoles.FirstOrDefault(r => r.RoleName == roleName);

            if (role != null)
                return role.Id;

            role = new TblRole
            {
                RoleName = roleName
            };

            db.TblRoles.Add(role);
            db.SaveChanges();

            return role.Id;
        }
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (db.TblUsers.Any(u => u.UserName == vm.UserName))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(vm);
            }

            PasswordHelper.CreateHash(vm.Password, out var hash, out var salt);

            int adminRoleId = EnsureRoleExists("Admin");
            int userRoleId = EnsureRoleExists("User");

            // First user = Admin, others = User
            int assignedRoleId = !db.TblUsers.Any() ? adminRoleId : userRoleId;

            db.TblUsers.Add(new TblUser
            {
                UserName = vm.UserName,
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = assignedRoleId
            });

            db.SaveChanges();

            return RedirectToAction("Login");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateUser()
        {
            ViewBag.Roles = db.TblRoles.ToList();
            return View();
        }

        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public ActionResult CreateUser(RegisterVm vm)
        {
            if (vm.RoleId == 0)
                ModelState.AddModelError("RoleId", "Please select a role.");

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = db.TblRoles.ToList();
                return View(vm);
            }

            if (db.TblUsers.Any(u => u.UserName == vm.UserName))
            {
                ModelState.AddModelError("", "Username already exists.");
                ViewBag.Roles = db.TblRoles.ToList();
                return View(vm);
            }

            PasswordHelper.CreateHash(vm.Password, out var hash, out var salt);

            db.TblUsers.Add(new TblUser
            {
                UserName = vm.UserName,
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = vm.RoleId
            });

            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}