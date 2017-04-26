using System;
using System.ComponentModel.DataAnnotations;

namespace OffSite.Data.ViewModels
{
    public class OffSiteRequestViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int SelectedVacationStatusId { get; set; }

        public string SelectedStatusName { get; set; }

        [Required(ErrorMessage = "The 'From date' field is required.")]
        [Display(Name = "From date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "The 'To date' field is required.")]
        [Display(Name = "To date")]
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }

        public bool Approved { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool TakeHalfADay { get; set; }
    }
}
