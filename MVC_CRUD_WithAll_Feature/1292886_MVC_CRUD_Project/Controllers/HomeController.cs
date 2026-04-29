using _1292886_MVC_CRUD_Project.DAL;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace _1292886_MVC_CRUD_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext DB = new AppDbContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.TotalRevenue = DB.Sales.Any() ? DB.Sales.Sum(s => s.TotalPrice) : 0;
            ViewBag.TotalUsers = DB.TblUsers.Count();
            ViewBag.TotalRoles = DB.TblRoles.Count();
            ViewBag.TotalPaymentMethods = DB.PaymentMethods.Count();

            ViewBag.RecentProperties = DB.Properties
                .OrderByDescending(p => p.PropertyId)
                .Take(6)
                .ToList();

            ViewBag.RecentActivitySales = DB.Sales
                .Include(s => s.PaymentMethod)
                .OrderByDescending(s => s.SalesId)
                .Take(5)
                .ToList();
            ViewBag.TotalSales = DB.Sales.Count();
            ViewBag.PaidSales = DB.Sales.Count(s => s.IsPaid);
            ViewBag.UnpaidSales = DB.Sales.Count(s => !s.IsPaid);
            ViewBag.TotalProperties = DB.Properties.Count();

            ViewBag.CashSales = DB.Sales.Count(s => s.PaymentMethod != null && s.PaymentMethod.PaymentType == "Cash");
            ViewBag.BankSales = DB.Sales.Count(s => s.PaymentMethod != null && s.PaymentMethod.PaymentType == "Bank");
            ViewBag.NagadSales = DB.Sales.Count(s => s.PaymentMethod != null && s.PaymentMethod.PaymentType == "Nagad");
            ViewBag.BkashSales = DB.Sales.Count(s => s.PaymentMethod != null && s.PaymentMethod.PaymentType == "Bkash");

            ViewBag.HomeCount = DB.Properties.Count(p => p.PropertyType == "Home");
            ViewBag.FlatCount = DB.Properties.Count(p => p.PropertyType == "Flat");
            ViewBag.ApartmentCount = DB.Properties.Count(p => p.PropertyType == "Apartment");
            ViewBag.LandCount = DB.Properties.Count(p => p.PropertyType == "Land");

            ViewBag.Roles = DB.TblRoles
                .OrderBy(r => r.RoleName)
                .ToList();

            ViewBag.Users = DB.TblUsers.ToList();

            ViewBag.PaymentMethods = DB.PaymentMethods
                .OrderBy(p => p.PaymentType)
                .ToList();

            var recentSales = DB.Sales
                .Include(s => s.PaymentMethod)
                .Include(s => s.Properties)
                .OrderByDescending(s => s.SalesId)
                .Take(5)
                .ToList();

            return View(recentSales);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DB.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}