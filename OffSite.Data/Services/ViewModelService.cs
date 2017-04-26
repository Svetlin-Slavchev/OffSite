using OffSite.Abstraction.Interfaces;
using System.Collections.Generic;

namespace OffSite.Data.Services
{
    public abstract class ViewModelService<TViewModel, TDataModel> : IViewModelService<TViewModel, TDataModel>
    {
        public ViewModelService()
        { }

        public abstract IEnumerable<TViewModel> Build(IEnumerable<TDataModel> dataModel);
        public abstract TViewModel Build(TDataModel dataModel);
        public abstract bool Create(TViewModel model);
        public abstract bool Delete(TViewModel model);
        public abstract TDataModel Get(object id);
        public abstract IEnumerable<TDataModel> GetAll();
        public abstract bool Update(TViewModel model);
    }
}
