using Convenience.Models.DataModels;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.ShohinMaster;

namespace Convenience.Models.Interfaces {
    public interface IShohinMasterService : IMasterRegistrationService<ShohinMaster, ShohinMasterService.PostMasterData, ShohinMasterViewModel> {

    }
}
