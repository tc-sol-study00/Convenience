using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.ShiireMaster;
using static Convenience.Models.Services.ShiireMasterService;

namespace Convenience.Models.Interfaces {
    public interface IShiireMasterService : IMasterRegistrationService<ShiireMaster, PostMasterData, ShiireMasterViewModel> {

    }
}
