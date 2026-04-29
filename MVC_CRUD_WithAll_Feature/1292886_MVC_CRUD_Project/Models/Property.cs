using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace _1292886_MVC_CRUD_Project.Models
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        public string PropertyType { get; set; }

        public string Location { get; set; }

        [Required]
        public int SalesId { get; set; }

        [ForeignKey("SalesId")]
        public virtual Sale Sale { get; set; }
    }
}