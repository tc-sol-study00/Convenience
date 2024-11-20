using Convenience.Models.DataModels;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.NaigaiClassMaster;
using static Convenience.Models.Services.NaigaiClassMasterService;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 内外区分マスタサービス用インターフェース
    /// </summary>
    public interface INaigaiClassMasterService : IMasterRegistrationService<NaigaiClassMaster, PostMasterData, NaigaiClassMasterViewModel> {

    }
}
