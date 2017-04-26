using System;
using System.ComponentModel.DataAnnotations;

namespace OffSite.Data.ViewModels
{
    public class WatchersApproversToUserViewModel
    {
       
        public Guid WatchersId { get; set; }

        [Required]
        public Guid ApproversId { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
