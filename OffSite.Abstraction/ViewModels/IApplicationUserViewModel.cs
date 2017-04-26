using System.Collections.Generic;

namespace OffSite.Abstraction.ViewModels
{
    public interface IApplicationUserViewModel
    {
        string Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        IList<string> RoleName { get; set; }
    }
}
