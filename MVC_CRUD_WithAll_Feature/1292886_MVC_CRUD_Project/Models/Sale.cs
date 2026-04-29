using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace _1292886_MVC_CRUD_Project.Models
{
    public class Sale
    {
        public Sale()
        {
            this.Properties = new HashSet<Property>();
        }
        [Key]
        public int SalesId { get; set; }

        [Required, Display(Name = "Sale Date"), DataType(DataType.Date),
            DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDate { get; set; }

        [Required]
        [Column(TypeName = "decimal")]
        public decimal TotalPrice { get; set; }

        public string ClientName { get; set; }
        public string MobileNo { get; set; }
        public string ClientImage { get; set; }

        [Required]
        public int PaymentTypeId { get; set; }

        [Required]
        public bool IsPaid { get; set; }
        [ForeignKey("PaymentTypeId")]
        public virtual PaymentMethod PaymentMethod { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
    }
}