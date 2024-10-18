using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Chumon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;


namespace Convenience.Controllers {

    /// <summary>
    /// サービスを使わない注文コントローラ（演習対象外）
    /// </summary>
    public class Chumon2Controller : Controller {
        private readonly ConvenienceContext _context;

        //private IChumonService chumonService;

        private readonly IChumon chumon;

        public Chumon2Controller(ConvenienceContext context) {
            _context = context;
            chumon = new Chumon(_context);
        }

        public async Task<IActionResult> KeyInput() {
            ChumonKeysViewModel keymodel = await SetChumonKeysViewModel();
            ViewData["Title"] = "商品注文２";
            return View("/Views/Chumon/KeyInput.cshtml",keymodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KeyInput(ChumonKeysViewModel inChumonKeysViewModel) {

            if (!ModelState.IsValid) {
                throw new InvalidOperationException("Postデータエラー");
            }

            //注文実績モデル変数定義
            ChumonJisseki? chumonJisseki;

            string inShiireSakiId = inChumonKeysViewModel.ShiireSakiId 
                ?? throw new ArgumentException("仕入先コードセットなし");
            DateOnly inChumonDate = inChumonKeysViewModel.ChumonDate > DateOnly.MinValue 
                ? inChumonKeysViewModel.ChumonDate 
                : throw new ArgumentException("注文日付セットなし");

            //もし、引数の注文日付がない場合（画面入力の注文日付が入力なしだと、1年1月1日になる
            if (DateOnly.FromDateTime(new DateTime(1, 1, 1)) == inChumonDate) {
                chumonJisseki = await chumon.ChumonSakusei(inShiireSakiId, DateOnly.FromDateTime(DateTime.Now));   //注文日付が指定なし→注文作成
            }
            else {
                //注文日付指定あり→注文問い合わせ
                chumonJisseki = await chumon.ChumonToiawase(inShiireSakiId, inChumonDate);

                //注文問い合わせでデータがない場合(null)は、注文作成
                chumonJisseki ??= await chumon.ChumonSakusei(inShiireSakiId, inChumonDate);
            }

            ViewData["Title"] = "商品注文２";

            return View("/Views/Chumon/ChumonMeisai.cshtml", new ChumonViewModel() { ChumonJisseki = chumonJisseki });

        }

        public IActionResult ChumonMeisai(ChumonViewModel inChumonViewModel) {

            ViewData["Title"] = "商品注文２";

            return View("/Views/Chumon/ChumonMeisai.cshtml",inChumonViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChumonMeisai(int id, ChumonViewModel ChumonViewModel) {

            ModelState.Clear(); //←これ入れておかないと再表示後のPostではまるよ？

            //仕入マスタより先はループっているので、更新前に縁をきる
            if (ChumonViewModel.ChumonJisseki.ChumonJissekiMeisais != null) {
                foreach (var item in ChumonViewModel.ChumonJisseki.ChumonJissekiMeisais) {
                    item.ShiireMaster = null;
                }
            }

            //注文実績更新（データ更新・追加共有）
            ChumonJisseki chumonJisseki = await chumon.ChumonUpdate(ChumonViewModel.ChumonJisseki)
                ?? throw new Exception("DB更新エラー");

            //データが更新されているかEFから聞く
            var entities = _context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity).Count();

            //ＤＢ更新
            await _context.SaveChangesAsync();

            //ＤＢ更新結果をもう一度問い合わせて、再表示させる

            chumonJisseki = await chumon.ChumonToiawase(ChumonViewModel.ChumonJisseki.ShiireSakiId, ChumonViewModel.ChumonJisseki.ChumonDate)
                ?? throw new Exception("DB更新後のデータが見つかりません");

            var chumonViewModel = new ChumonViewModel {
                ChumonJisseki = chumonJisseki!,
                IsNormal = true,
                Remark = entities!=0 ? "更新しました":string.Empty 
            };

            ViewData["Title"] = "商品注文２";
            return View("/Views/Chumon/ChumonMeisai.cshtml",chumonViewModel);
        }

        public async Task<ChumonKeysViewModel> SetChumonKeysViewModel() {
            var list = await _context.ShiireSakiMaster.OrderBy(s => s.ShiireSakiId).Select(s => new SelectListItem { Value = s.ShiireSakiId, Text = s.ShiireSakiId + " " + s.ShiireSakiKaisya }).ToListAsync();

            return (new ChumonKeysViewModel() {
                ShiireSakiId = null,
                ChumonDate = DateOnly.FromDateTime(DateTime.Today),
                ShiireSakiList = list
            });
        }

    }
}