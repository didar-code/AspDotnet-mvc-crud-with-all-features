using _1292886_MVC_CRUD_Project.DAL;
using _1292886_MVC_CRUD_Project.Models;
using _1292886_MVC_CRUD_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace _1292886_MVC_CRUD_Project.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly AppDbContext DB = new AppDbContext();

        [Authorize]
        public ActionResult Index(string search, string sortOrder, int? page,
           DateTime? fromDate, int? paymentTypeId, bool? isPaid)
        {

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentSort = sortOrder;

            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "date";
            ViewBag.PriceSortParm = sortOrder == "price" ? "price_desc" : "price";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.MobileSortParm = sortOrder == "mobile" ? "mobile_desc" : "mobile";
            ViewBag.PaymentSortParm = sortOrder == "payment" ? "payment_desc" : "payment";
            ViewBag.PropertiesSortParm = sortOrder == "properties" ? "properties_desc" : "properties";

            var sales = DB.Sales
                .Include(s => s.Properties)
                .Include(s => s.PaymentMethod)
                .AsQueryable();


           
            if (fromDate.HasValue)
            {
                sales = sales.Where(s => s.SaleDate >= fromDate.Value);
            }

         
            if (paymentTypeId.HasValue && paymentTypeId > 0)
            {
                sales = sales.Where(s => s.PaymentTypeId == paymentTypeId);
            }

            
            if (isPaid.HasValue)
            {
                sales = sales.Where(s => s.IsPaid == isPaid.Value);
            }



            if (!string.IsNullOrWhiteSpace(search))
            {
                sales = sales.Where(s =>
                    s.ClientName.Contains(search) ||
                    s.MobileNo.Contains(search) ||
                    s.PaymentMethod.PaymentType.Contains(search) ||
                    s.Properties.Any(p => p.PropertyType.Contains(search) || p.Location.Contains(search)));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    sales = sales.OrderByDescending(s => s.SaleDate);
                    break;
                case "date":
                    sales = sales.OrderBy(s => s.SaleDate);
                    break;
                case "price":
                    sales = sales.OrderBy(s => s.TotalPrice);
                    break;
                case "price_desc":
                    sales = sales.OrderByDescending(s => s.TotalPrice);
                    break;
                case "name":
                    sales = sales.OrderBy(s => s.ClientName);
                    break;
                case "name_desc":
                    sales = sales.OrderByDescending(s => s.ClientName);
                    break;
                case "mobile":
                    sales = sales.OrderBy(s => s.MobileNo);
                    break;
                case "mobile_desc":
                    sales = sales.OrderByDescending(s => s.MobileNo);
                    break;
                case "payment":
                    sales = sales.OrderBy(s => s.PaymentMethod.PaymentType);
                    break;
                case "payment_desc":
                    sales = sales.OrderByDescending(s => s.PaymentMethod.PaymentType);
                    break;
                case "properties":
                    sales = sales.OrderBy(s => s.Properties.Count());
                    break;
                case "properties_desc":
                    sales = sales.OrderByDescending(s => s.Properties.Count());
                    break;
                default:
                    sales = sales.OrderBy(s => s.SalesId);
                    break;
            }

            int pageSize = 3;
            int pageNumber = page ?? 1;

            ViewBag.FromDate = fromDate;
            ViewBag.PaymentTypeId = paymentTypeId;
            ViewBag.IsPaid = isPaid;
           
            ViewBag.PaymentMethods = DB.PaymentMethods
                .OrderBy(p => p.PaymentType)
                .ToList();

           
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                ViewBag.Roles = DB.TblRoles.OrderBy(r => r.RoleName).ToList();
                ViewBag.Users = DB.TblUsers.ToList();
            }



            return View(sales.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult IsMobileAvailable(string MobileNo, int SalesId = 0)
        {
            bool exists = DB.Sales.Any(s => s.MobileNo == MobileNo && s.SalesId != SalesId);

            return Json(!exists, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddPaymentMethod(string paymentType)
        {
            if (!string.IsNullOrWhiteSpace(paymentType))
            {
                paymentType = paymentType.Trim();

                bool exists = DB.PaymentMethods.Any(p => p.PaymentType == paymentType);
                if (!exists)
                {
                    DB.PaymentMethods.Add(new PaymentMethod
                    {
                        PaymentType = paymentType
                    });

                    DB.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePaymentMethod(int id)
        {
            var paymentMethod = DB.PaymentMethods.Find(id);

            if (paymentMethod != null)
            {
                bool isUsed = DB.Sales.Any(s => s.PaymentTypeId == id);

                if (!isUsed)
                {
                    DB.PaymentMethods.Remove(paymentMethod);
                    DB.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                roleName = roleName.Trim();

                bool exists = DB.TblRoles.Any(r => r.RoleName == roleName);
                if (!exists)
                {
                    DB.TblRoles.Add(new TblRole
                    {
                        RoleName = roleName
                    });
                    DB.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRole(int id)
        {
            var role = DB.TblRoles.Find(id);

            if (role != null)
            {
                bool hasUsers = DB.TblUsers.Any(u => u.RoleId == id);

                if (!hasUsers)
                {
                    DB.TblRoles.Remove(role);
                    DB.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreatePartial()
        {
            var vm = new SalesViewModel
            {
                SaleDate = DateTime.Today,
                IsPaid = false,
                Properties = new List<PropertyViewModel>
                {
                    new PropertyViewModel()
                },
                PaymentMethods = DB.PaymentMethods.ToList()
            };

            return PartialView("~/Views/Sales/_CreateSalesPartial.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public JsonResult CreateSale(SalesViewModel model)
        {
            ValidateImageFile(model.ProfileFile);

            if (!ModelState.IsValid)
            {
                model.PaymentMethods = DB.PaymentMethods.ToList();
                return Json(new
                {
                    success = false,
                    errors = GetModelErrors()
                });
            }

            string imagePath = model.ProfileFile != null
                ? GetImageName(model.ProfileFile)
                : "/Images/noImg.png";

            var sale = new Sale
            {
                SaleDate = model.SaleDate,
                TotalPrice = model.TotalPrice,
                ClientName = model.ClientName,
                MobileNo = model.MobileNo,
                ClientImage = imagePath,
                PaymentTypeId = model.PaymentTypeId,
                IsPaid = model.IsPaid
            };

            DB.Sales.Add(sale);
            DB.SaveChanges();

            if (model.Properties != null && model.Properties.Count > 0)
            {
                foreach (var item in model.Properties)
                {
                    if (item == null) continue;
                    if (string.IsNullOrWhiteSpace(item.PropertyType) && string.IsNullOrWhiteSpace(item.Location)) continue;

                    DB.Properties.Add(new Property
                    {
                        SalesId = sale.SalesId,
                        PropertyType = item.PropertyType,
                        Location = item.Location
                    });
                }

                DB.SaveChanges();
            }

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Index")
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditPartial(int id)
        {
            var sale = DB.Sales
                .Include(s => s.Properties)
                .Include(s => s.PaymentMethod)
                .FirstOrDefault(s => s.SalesId == id);

            if (sale == null)
            {
                return HttpNotFound("Sale not found");
            }

            var vm = new SalesViewModel
            {
                SalesId = sale.SalesId,
                SaleDate = sale.SaleDate,
                TotalPrice = sale.TotalPrice,
                ClientName = sale.ClientName,
                MobileNo = sale.MobileNo,
                PaymentTypeId = sale.PaymentTypeId,
                IsPaid = sale.IsPaid,
                ClientImage = sale.ClientImage,
                Properties = sale.Properties.Select(p => new PropertyViewModel
                {
                    PropertyId = p.PropertyId,
                    SalesId = p.SalesId,
                    PropertyType = p.PropertyType,
                    Location = p.Location
                }).ToList(),
                PaymentMethods = DB.PaymentMethods.ToList()
            };

            if (vm.Properties == null || vm.Properties.Count == 0)
            {
                vm.Properties = new List<PropertyViewModel>
                {
                    new PropertyViewModel()
                };
            }

            return PartialView("~/Views/Sales/_EditSalesPartial.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public JsonResult EditSale(SalesViewModel model, string OldClientImage)
        {
            ValidateImageFile(model.ProfileFile);

            if (!ModelState.IsValid)
            {
                model.PaymentMethods = DB.PaymentMethods.ToList();
                return Json(new
                {
                    success = false,
                    errors = GetModelErrors()
                });
            }

            var sale = DB.Sales
                .Include(s => s.Properties)
                .FirstOrDefault(s => s.SalesId == model.SalesId);

            if (sale == null)
            {
                return Json(new
                {
                    success = false,
                    errors = new[]
                    {
                        new
                        {
                            field = "SalesId",
                            messages = new[] { "Sale not found." }
                        }
                    }
                });
            }

            sale.SaleDate = model.SaleDate;
            sale.TotalPrice = model.TotalPrice;
            sale.ClientName = model.ClientName;
            sale.MobileNo = model.MobileNo;
            sale.PaymentTypeId = model.PaymentTypeId;
            sale.IsPaid = model.IsPaid;

            if (model.ProfileFile != null && model.ProfileFile.ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(sale.ClientImage) && !sale.ClientImage.EndsWith("noImg.png", StringComparison.OrdinalIgnoreCase))
                {
                    string oldPath = Server.MapPath(sale.ClientImage);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                sale.ClientImage = GetImageName(model.ProfileFile);
            }
            else
            {
                sale.ClientImage = OldClientImage;
            }

            var incomingExistingIds = (model.Properties ?? new List<PropertyViewModel>())
                .Where(p => p != null && p.PropertyId > 0)
                .Select(p => p.PropertyId)
                .ToList();

            var toRemove = sale.Properties
                .Where(p => !incomingExistingIds.Contains(p.PropertyId))
                .ToList();

            if (toRemove.Any())
            {
                DB.Properties.RemoveRange(toRemove);
            }

            if (model.Properties != null)
            {
                foreach (var item in model.Properties)
                {
                    if (item == null) continue;
                    if (string.IsNullOrWhiteSpace(item.PropertyType) && string.IsNullOrWhiteSpace(item.Location)) continue;

                    if (item.PropertyId > 0)
                    {
                        var existing = sale.Properties.FirstOrDefault(p => p.PropertyId == item.PropertyId);
                        if (existing != null)
                        {
                            existing.PropertyType = item.PropertyType;
                            existing.Location = item.Location;
                        }
                    }
                    else
                    {
                        sale.Properties.Add(new Property
                        {
                            SalesId = sale.SalesId,
                            PropertyType = item.PropertyType,
                            Location = item.Location
                        });
                    }
                }
            }

            DB.SaveChanges();

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Index")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public JsonResult DeleteSale(int id)
        {
            var sale = DB.Sales
                .Include(s => s.Properties)
                .FirstOrDefault(s => s.SalesId == id);

            if (sale == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Sale not found."
                });
            }

            if (!string.IsNullOrEmpty(sale.ClientImage) && !sale.ClientImage.EndsWith("noImg.png", StringComparison.OrdinalIgnoreCase))
            {
                string oldPath = Server.MapPath(sale.ClientImage);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            if (sale.Properties != null && sale.Properties.Any())
            {
                DB.Properties.RemoveRange(sale.Properties);
            }

            DB.Sales.Remove(sale);
            DB.SaveChanges();

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Index")
            });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPropertyImage(int propertyId, HttpPostedFileBase imageFile)
        {
            if (propertyId <= 0 || imageFile == null || imageFile.ContentLength == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var ext = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (string.IsNullOrWhiteSpace(ext) || !allowedExtensions.Contains(ext))
            {
                return RedirectToAction("Index", "Home");
            }

            const int maxFileSize = 4 * 1024 * 1024;
            if (imageFile.ContentLength > maxFileSize)
            {
                return RedirectToAction("Index", "Home");
            }

            var folderPath = Server.MapPath("~/Images/Properties/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var oldJpg = Path.Combine(folderPath, "property_" + propertyId + ".jpg");
            var oldJpeg = Path.Combine(folderPath, "property_" + propertyId + ".jpeg");
            var oldPng = Path.Combine(folderPath, "property_" + propertyId + ".png");

            if (System.IO.File.Exists(oldJpg)) System.IO.File.Delete(oldJpg);
            if (System.IO.File.Exists(oldJpeg)) System.IO.File.Delete(oldJpeg);
            if (System.IO.File.Exists(oldPng)) System.IO.File.Delete(oldPng);

            var filePath = Path.Combine(folderPath, "property_" + propertyId + ext);
            imageFile.SaveAs(filePath);

            return RedirectToAction("Index", "Home");
        }
        private void ValidateImageFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return;
            }

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (string.IsNullOrWhiteSpace(ext) || !allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError("ProfileFile", "Only jpg, jpeg and png files are allowed.");
                return;
            }

            const int maxFileSize = 2 * 1024 * 1024; 
            if (file.ContentLength > maxFileSize)
            {
                ModelState.AddModelError("ProfileFile", "Image size must be 2 MB or less.");
            }
        }

        private object GetModelErrors()
        {
            return ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    field = x.Key,
                    messages = x.Value.Errors
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage)
                        .ToArray()
                })
                .ToList();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserRole(int userId, int roleId)
        {
            var user = DB.TblUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            var role = DB.TblRoles.FirstOrDefault(r => r.Id == roleId);
            if (role == null)
            {
                TempData["Error"] = "Role not found.";
                return RedirectToAction("Index");
            }

            user.RoleId = roleId;
            DB.SaveChanges();

            TempData["Success"] = "User role updated successfully.";
            return RedirectToAction("Index");
        }
        private string GetImageName(HttpPostedFileBase file)
        {
            string filePath = "";

            if (file != null)
            {
                var imagesFolder = Server.MapPath("~/Images/");
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                filePath = "/Images/" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                file.SaveAs(Server.MapPath(filePath));
            }

            return filePath;
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


    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new HttpStatusCodeResult(401);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}

