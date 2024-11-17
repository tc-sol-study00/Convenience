using Convenience.Models.Interfaces;
using static Convenience.Models.Services.NaigaiClassMasterService;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.NaigaiClassMaster, Convenience.Models.Services.NaigaiClassMasterService.PostMasterData, Convenience.Models.ViewModels.NaigaiClassMaster.NaigaiClassMasterViewModel>;
using Microsoft.AspNetCore.Mvc.Rendering;
using Convenience.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Convenience.Models.DataModels;


namespace Convenience.Models.ViewModels.NaigaiClassMaster {
    public class NaigaiClassMasterViewModel : IMasterRegistrationViewModel, IMasterRegistrationSelectList {
        [JsonIgnore]
        public ConvenienceContext _context { get; set; }
        public IList<PostMasterData> PostMasterDatas { get; set; }

        private readonly IMasterRegistrationSelectList my;

        /// <summary>
        /// 処理が正常がどうか（正常=true)
        /// </summary>
        public bool? IsNormal { get; set; }
        /// <summary>
        /// 処理結果（ＤＢ反映結果）表示内容
        /// </summary>
        public string? Remark { get; set; }

        public NaigaiClassMasterViewModel() {
            PostMasterDatas = new List<PostMasterData>();
            IsNormal = default;
            Remark = string.Empty;
            my = this;
        }

        // 依存性注入に対応したコンストラクタ
        public NaigaiClassMasterViewModel(ConvenienceContext context) {
            _context = context;
            PostMasterDatas = new List<PostMasterData>();
            IsNormal = default;
            Remark = string.Empty;
            my = this;
    /*
            ShiireSakiList = my.SetSelectList<NaigaiClassMaster>();
            ShohinList = my.SetSelectList<DataModels.ShohinMaster>();
    */
        }
    }
}

