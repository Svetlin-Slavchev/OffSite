using OffSite.Abstraction.Interfaces;
using OffSite.Data.Entities;
using OffSite.Data.ViewModels;
using System;
using System.Collections.Generic;

namespace OffSite.Data.Services
{
    public class OffSiteRequestService : IViewModelService<OffSiteRequestViewModel, OffSiteRequest>
    {
        public IEnumerable<OffSiteRequestViewModel> Build(IEnumerable<OffSiteRequest> dataModel)
        {
            throw new NotImplementedException();
        }

        public OffSiteRequestViewModel Build(OffSiteRequest dataModel)
        {
            throw new NotImplementedException();
        }

        public bool Create(OffSiteRequestViewModel model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(OffSiteRequestViewModel model)
        {
            throw new NotImplementedException();
        }

        public OffSiteRequest Get(object id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OffSiteRequest> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Update(OffSiteRequestViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
