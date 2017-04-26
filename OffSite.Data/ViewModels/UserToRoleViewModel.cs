using System;
using System.ComponentModel.DataAnnotations;

namespace OffSite.Data.ViewModels
{
    public class UserToRoleViewModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RolesId { get; set; }
    }
}
