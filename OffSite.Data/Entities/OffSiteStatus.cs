using System.ComponentModel.DataAnnotations;

namespace OffSite.Data.Entities
{
    public class OffSiteStatus
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }
    }
}
