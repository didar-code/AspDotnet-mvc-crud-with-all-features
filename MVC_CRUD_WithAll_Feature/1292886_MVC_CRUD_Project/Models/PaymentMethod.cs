using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _1292886_MVC_CRUD_Project.Models
{
    public class PaymentMethod
    {
        public PaymentMethod()
        {
            this.Sales = new HashSet<Sale>();
        }
        [Key]
        public int PaymentTypeId { get; set; }

        public string PaymentType { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}