using System.Collections.Generic;

namespace OffSite.Abstraction.Interfaces
{
    public interface IViewModelService<TViewModel, TDataModel>
    {
        // Get from db.
        TDataModel Get(object id);
        IEnumerable<TDataModel> GetAll();

        // Work with db.
        bool Create(TViewModel model);
        bool Update(TViewModel model);
        bool Delete(TViewModel model);

        // Map db entity to viem model.
        TViewModel Build(TDataModel dataModel);
        IEnumerable<TViewModel> Build(IEnumerable<TDataModel> dataModel);
    }
}
