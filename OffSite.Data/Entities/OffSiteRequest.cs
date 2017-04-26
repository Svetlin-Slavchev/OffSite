using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OffSite.Abstraction.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OffSite.Data.Entities
{
    public class OffSiteRequest
    {
        public OffSiteRequest()
        { }

        public OffSiteRequest(OffSiteStatus status, DateTime startDate, DateTime endDate,
            string reason, string userId, bool isHalfADayRequest = false)
        {
            this.Status = status;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Reason = reason;
            this.CreatedDate = DateTime.Now;
            this.Approved = false;
            this.UserFkId = userId;
            this.IsHalfADayRequest = isHalfADayRequest;
        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("Status")]
        public int StatusFkId { get; set; }

        public OffSiteStatus Status { get; set; }

        [Display(Name = "From")]
        [Required(ErrorMessage = "Test")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "To")]
        public DateTime EndDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Reason { get; set; }

        public bool Approved { get; set; }

        [ForeignKey("User")]
        public string UserFkId { get; set; }

        public ApplicationUser User { get; set; }

        public bool IsHalfADayRequest { get; set; }
    }
}
