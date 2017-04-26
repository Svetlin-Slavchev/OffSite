using OffSite.Abstraction.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OffSite.Data.Entities
{
    public class NotificationMessage
    {
        public NotificationMessage()
        { }

        public NotificationMessage(int vacationRequest, string selectedUser, string approverUser,
            bool approved = false, bool approvedDone = false)
        {
            OffSiteRequestFkId = vacationRequest;
            SelectedUserFkId = selectedUser;
            ApproverUserFkId = approverUser;
            Approved = approved;
            ApprovedDone = approvedDone;
        }

        public int Id { get; set; }

        [ForeignKey("OffSiteRequest")]
        public int OffSiteRequestFkId { get; set; }

        public OffSiteRequest OffSiteRequest { get; set; }

        public bool Approved { get; set; }

        public bool ApprovedDone { get; set; }

        [ForeignKey("ApproverUser")]
        public string ApproverUserFkId { get; set; }

        public ApplicationUser ApproverUser { get; set; }

        [ForeignKey("SelectedUser")]
        public string SelectedUserFkId { get; set; }

        public ApplicationUser SelectedUser { get; set; }

        public bool Viewed { get; set; }
    }
}
