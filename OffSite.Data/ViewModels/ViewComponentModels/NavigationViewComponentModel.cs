using OffSite.Abstraction.Entities;
using OffSite.Data.Entities;

namespace OffSite.Data.ViewModels.ViewComponentModels
{
    public class NavigationViewComponentModel
    {
        public ApplicationUser CurrentUser { get; set; }
        public bool IsAdmin { get; set; }
    }
}
