using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _1292886_MVC_CRUD_Project.ViewModels
{
    public class RegisterVm
    {
        [Required]
        public string UserName { get; set; }
        public int Id { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }
        public int RoleId { get; set; }
    }

    public class LoginVm
    {
        [Required]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}