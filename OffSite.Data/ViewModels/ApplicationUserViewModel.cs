using OffSite.Abstraction.ViewModels;
using System.Collections.Generic;

namespace OffSite.Data.ViewModels
{
    public class ApplicationUserViewModel : IApplicationUserViewModel
    {
        public ApplicationUserViewModel()
        { }

        public ApplicationUserViewModel(string id, string name, string email, IList<string> role)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.RoleName = role;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IList<string> RoleName { get; set; }
    }
}
