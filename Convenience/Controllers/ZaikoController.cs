using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.Zaiko;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {

    /// <summary>
    /// 倉庫在庫検索コントローラ
    /// </summary>
    public class ZaikoController : Controller {
        private readonly ConvenienceContext _context;

        private readonly IZaikoService zaikoService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="zaikoService">在庫サービスクラスＤＩ注入用</param>
        public ZaikoController(ConvenienceContext context,IZaikoService zaikoService) {
            this._context = context;
            this.zaikoService = zaikoService;
            //zaikoService = new ZaikoService(_context);
        }

        public async Task<IActionResult> Index() {
            ZaikoViewModel zaikoViewModel = new ZaikoViewModel() { };
            return View(zaikoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ZaikoViewModel inZaikoModel) {
            var keydata = inZaikoModel.KeyEventList;
            var selecteWhereItemArray = inZaikoModel.SelecteWhereItemArray;

            ZaikoViewModel zaikoViewModel = new ZaikoViewModel() {
                zaikoListLine = await zaikoService.KeyInput(keydata, selecteWhereItemArray)
            };
            return View(zaikoViewModel);
        }
    }
}