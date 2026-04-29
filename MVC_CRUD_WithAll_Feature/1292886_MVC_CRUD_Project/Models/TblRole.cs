using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _1292886_MVC_CRUD_Project.Models
{
    public class TblRole
    {
        public TblRole()
        {
            this.Users = new HashSet<TblUser>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        public virtual ICollection<TblUser> Users { get; set; }
    }
}