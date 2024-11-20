using Convenience.Models.DataModels;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.ShiireSakiMaster;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 仕入先マスタサービス用インターフェース
    /// </summary>
    public interface IShiireSakiMasterService : IMasterRegistrationService<ShiireSakiMaster, ShiireSakiMasterService.PostMasterData, ShiireSakiMasterViewModel> {

    }
}
