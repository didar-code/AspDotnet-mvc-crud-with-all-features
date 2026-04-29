using _1292886_MVC_CRUD_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _1292886_MVC_CRUD_Project.ViewModels
{
    public class SalesViewModel
    {
        public int SalesId { get; set; }

        [Required(ErrorMessage = "Sale Date is required")]
        [Display(Name = "Sale Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDate { get; set; } = DateTime.Today;


        [Required(ErrorMessage = "Total Price is required")]
        [Range(100000, 100000000, ErrorMessage = "Price must be between 100000 and 100000000")]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }


        [Required(ErrorMessage = "Client Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }


        [Required(ErrorMessage = "Mobile number is required")]
        [Display(Name = "Mobile No")]
        [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Enter a valid Bangladeshi mobile number")]
        [Remote("IsMobileAvailable", "Sales", AdditionalFields = "SalesId", ErrorMessage = "This mobile number already exists")]
        public string MobileNo { get; set; }

        [Display(Name = "Client Image")]
        public string ClientImage { get; set; }


        [Required(ErrorMessage = "Payment Type is required")]
        [Display(Name = "Payment Type")]
        public int PaymentTypeId { get; set; }


        [Display(Name = "Paid?")]
        public bool IsPaid { get; set; }


        [Display(Name = "Upload Client Picture")]
        public HttpPostedFileBase ProfileFile { get; set; }


        public string PaymentTypeName { get; set; }


        public virtual IList<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();


        public virtual IList<Sale> Sales { get; set; } = new List<Sale>();


        public virtual IList<PropertyViewModel> Properties { get; set; } = new List<PropertyViewModel>();
    }



    public class PropertyViewModel
    {
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Property Type is required")]
        [StringLength(50)]
        [Display(Name = "Property Type")]
        public string PropertyType { get; set; }


        [Required(ErrorMessage = "Location is required")]
        [StringLength(100)]
        [Display(Name = "Location")]
        public string Location { get; set; }


        public int SalesId { get; set; }
    }
}