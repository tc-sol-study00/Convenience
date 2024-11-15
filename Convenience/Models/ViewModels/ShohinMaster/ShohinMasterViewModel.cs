using Convenience.Models.Interfaces;
using static Convenience.Models.Services.ShohinMasterService;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShohinMaster, Convenience.Models.Services.ShohinMasterService.PostMasterData, Convenience.Models.ViewModels.ShohinMaster.ShohinMasterViewModel>;


namespace Convenience.Models.ViewModels.ShohinMaster {
    public class ShohinMasterViewModel : IMasterRegistrationViewModel {
        public IList<PostMasterData> PostMasterDatas { get; set; }

        /// <summary>
        /// 処理が正常がどうか（正常=true)
        /// </summary>
        public bool? IsNormal { get; set; }
        /// <summary>
        /// 処理結果（ＤＢ反映結果）表示内容
        /// </summary>
        public string? Remark { get; set; }

        public ShohinMasterViewModel() {
            PostMasterDatas = new List<PostMasterData>();
            Remark = string.Empty;
        }
    }
}
