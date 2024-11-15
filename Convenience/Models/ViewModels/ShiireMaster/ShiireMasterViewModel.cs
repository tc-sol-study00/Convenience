using Convenience.Models.Interfaces;
using static Convenience.Models.Services.ShiireMasterService;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShiireMaster, Convenience.Models.Services.ShiireMasterService.PostMasterData, Convenience.Models.ViewModels.ShiireMaster.ShiireMasterViewModel>;
using Microsoft.AspNetCore.Mvc.Rendering;
using Convenience.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Convenience.Models.DataModels;


namespace Convenience.Models.ViewModels.ShiireMaster {
    public class ShiireMasterViewModel : IMasterRegistrationViewModel, IMasterRegistrationSelectList {
        [JsonIgnore]
        public ConvenienceContext _context { get; set; }
        public IList<PostMasterData> PostMasterDatas { get; set; }

        public IList<SelectListItem> ShiireSakiList { get; set; }

        public IList<SelectListItem> ShohinList { get; set; }
        
        private readonly IMasterRegistrationSelectList my;


        /// <summary>
        /// 処理が正常がどうか（正常=true)
        /// </summary>
        public bool? IsNormal { get; set; }
        /// <summary>
        /// 処理結果（ＤＢ反映結果）表示内容
        /// </summary>
        public string? Remark { get; set; }

        // パラメータレスコンストラクタ
        public ShiireMasterViewModel() {
            PostMasterDatas = new List<PostMasterData>();
            IsNormal = default;
            Remark = string.Empty;
            ShiireSakiList = new List<SelectListItem>();
            ShohinList = new List<SelectListItem>();
            my = this;
        }

        // 依存性注入に対応したコンストラクタ
        public ShiireMasterViewModel(ConvenienceContext context) {
            _context = context;
            PostMasterDatas = new List<PostMasterData>();
            IsNormal = default;
            Remark = string.Empty;
            my = this;
            ShiireSakiList = my.SetSelectList<ShiireSakiMaster>();
            ShohinList = my.SetSelectList<DataModels.ShohinMaster>();

            /*
            // SelectList の初期化
            ShiireSakiList = _context.ShiireSakiMaster.AsNoTracking()
                .OrderBy(x => x.ShiireSakiId)
                .Select(x => new SelectListItem { Value = x.ShiireSakiId, Text = $"{x.ShiireSakiId}:{x.ShiireSakiKaisya}" })
                .ToList();

            ShohinList = _context.ShohinMaster.AsNoTracking()
                .OrderBy(x => x.ShohinId)
                .Select(x => new SelectListItem { Value = x.ShohinId, Text = $"{x.ShohinId}:{x.ShohinName}" })
                .ToList();
            */
        }
    }
}

