using Convenience.Models.Interfaces;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.Zaiko;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;

namespace Convenience.Controllers {

    /// <summary>
    /// 倉庫在庫検索コントローラ
    /// </summary>
    public class ZaikoController : Controller , ISharedTools {
        //private readonly ConvenienceContext _context;

        private readonly IZaikoService zaikoService;

        /// <summary>
        /// Postデータ引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "ZaikoPostData";

        /// <summary>
        /// CSVファイル作成（ＤＩ用）
        /// </summary>
        private readonly IConvertObjectToCsv _convertObjectToCsv;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="zaikoService">在庫サービスクラスＤＩ注入用</param>
        public ZaikoController(IZaikoService zaikoService, IConvertObjectToCsv convertObjectToCsv) {
            this.zaikoService = zaikoService;
            this._convertObjectToCsv = convertObjectToCsv;
        }

        public IActionResult Index() {
            ZaikoViewModel zaikoViewModel = new();
            return View(zaikoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ZaikoViewModel inZaikoModel) {
            KeyEventRec[] keydata = inZaikoModel.KeyEventList;
            SelecteWhereItem[] selecteWhereItemArray = inZaikoModel.SelecteWhereItemArray;

            ZaikoViewModel zaikoViewModel =
                await zaikoService.KeyInput(keydata, selecteWhereItemArray);

            // キーワードエリアの保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(inZaikoModel);

            return View(zaikoViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> DownLoad() {
            ZaikoViewModel viewmodel
                = ISharedTools.ConvertFromSerial<ZaikoViewModel>(TempData.Peek(IndexName)?.ToString() ?? throw new Exception("tempdataなし"));

            KeyEventRec[] keydata = viewmodel.KeyEventList;
            SelecteWhereItem[] selecteWhereItemArray = viewmodel.SelecteWhereItemArray;

            ZaikoViewModel zaikoViewModel =
                await zaikoService.KeyInput(keydata, selecteWhereItemArray);

            //このコントローラの名前を認識
            string fileName = _convertObjectToCsv.GetFileName(this.GetType().Name);

            //モデルデータを取り出すし、ＣＳＶに変換
            IEnumerable<ZaikoListLine> modeldatas = zaikoViewModel.ZaikoListLines;
            string filename = _convertObjectToCsv.ConvertToCSV<ZaikoListLine, CSVMapping.SokoZaikoCSV>(modeldatas, fileName);

            //バイトに変換しファイルをhttp出力
            byte[] fileBytes = System.IO.File.ReadAllBytes(filename);

            return File(fileBytes, "text/csv", fileName);
        }

    }
}