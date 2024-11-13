using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.Zaiko;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {

    /// <summary>
    /// 倉庫在庫検索コントローラ
    /// </summary>
    public class ZaikoController : Controller {
        //private readonly ConvenienceContext _context;

        private readonly IZaikoService zaikoService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="zaikoService">在庫サービスクラスＤＩ注入用</param>
        public ZaikoController(IZaikoService zaikoService) {
            //this._context = context;
            this.zaikoService = zaikoService;
            //zaikoService = new ZaikoService(_context);
        }

        public IActionResult Index() {
            ZaikoViewModel zaikoViewModel = new();
            return View(zaikoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ZaikoViewModel inZaikoModel) {
            var keydata = inZaikoModel.KeyEventList;
            var selecteWhereItemArray = inZaikoModel.SelecteWhereItemArray;

            ZaikoViewModel zaikoViewModel =
                await zaikoService.KeyInput(keydata, selecteWhereItemArray);

            return View(zaikoViewModel);
        }
    }
}