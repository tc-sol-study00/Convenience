using Convenience.Models.Interfaces;
using static Convenience.Models.Services.ShiireMasterService;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShiireMaster, Convenience.Models.Services.ShiireMasterService.PostMasterData, Convenience.Models.ViewModels.ShiireMaster.ShiireMasterViewModel>;
using Microsoft.AspNetCore.Mvc.Rendering;
using Convenience.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Convenience.Models.ViewModels.ShiireMaster {
    public class ShiireMasterViewModel : IMasterRegistrationViewModel {

        private readonly ConvenienceContext? _context;
        public IList<PostMasterData> PostMasterDatas { get; set; }

        public List<SelectListItem> ShiireSakiList { get; set; }

        public List<SelectListItem> ShohinList { get; set; }

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
            Remark = string.Empty;
            ShiireSakiList = new List<SelectListItem>();
            ShohinList = new List<SelectListItem>();
        }

        // 依存性注入に対応したコンストラクタ
        public ShiireMasterViewModel(ConvenienceContext context) {
            _context = context;
            PostMasterDatas = new List<PostMasterData>();
            Remark = string.Empty;

            // SelectList の初期化
            ShiireSakiList = _context.ShiireSakiMaster.AsNoTracking()
                .OrderBy(x => x.ShiireSakiId)
                .Select(x => new SelectListItem { Value = x.ShiireSakiId, Text = $"{x.ShiireSakiId}:{x.ShiireSakiKaisya}" })
                .ToList();

            ShohinList = _context.ShohinMaster.AsNoTracking()
                .OrderBy(x => x.ShohinId)
                .Select(x => new SelectListItem { Value = x.ShohinId, Text = $"{x.ShohinId}:{x.ShohinName}" })
                .ToList();
        }
    }
}

