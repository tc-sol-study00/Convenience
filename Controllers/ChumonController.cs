using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Chumon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Convenience.Models.Properties.Message;


namespace Convenience.Controllers {
    public class ChumonController : Controller {
        private readonly ConvenienceContext _context;

        private static readonly string IndexName = "ChumonJisseki";

        //private IChumonService chumonService;

        private IChumonService chumonService;

        public ChumonController(ConvenienceContext context) {
            _context = context;
            chumonService = new ChumonService(_context);
        }

        private void KeepObject() {
            JsonSerializerOptions options = new() {
                //Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true,
            };
            TempData[IndexName] = JsonSerializer.Serialize(chumonService.chumon.ChumonJisseki,options);
        }

        private void GetObject() {
            if (TempData.Peek(IndexName) != null) {
                chumonService.chumon.ChumonJisseki = JsonSerializer.Deserialize<ChumonJisseki>((string)TempData[IndexName]);
            }
            else if (chumonService == null) {
                KeepObject();
            }
        }

        public async Task<IActionResult> KeyInput() {
            ChumonKeysViewModel keymodel = SetChumonKeysViewModel();
            return View(keymodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KeyInput(ChumonKeysViewModel inChumonKeysViewModel) {
            ChumonViewModel chumonViewModel;

            if (!ModelState.IsValid) {
                throw new InvalidOperationException("Postデータエラー");
            }

            if (inChumonKeysViewModel.ChumonDate == DateOnly.FromDateTime(new DateTime(1, 1, 1))) {
                chumonViewModel = chumonService.ChumonSetting(inChumonKeysViewModel.ShiireSakiId, DateOnly.FromDateTime(DateTime.Now));
            }
            else {
                chumonViewModel = chumonService.ChumonSetting(inChumonKeysViewModel.ShiireSakiId, inChumonKeysViewModel.ChumonDate);
            }
            KeepObject();
            return View("ChumonMeisai", chumonViewModel);
        }

        public async Task<IActionResult> ChumonMeisai(ChumonViewModel inChumonViewModel) {

            return View(inChumonViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChumonMeisai(int id, ChumonViewModel ChumonViewModel) {

            //Thread.Sleep(10000);
            GetObject();
            if (!ModelState.IsValid) {
                throw new Exception("Postデータエラー");
            };

            ModelState.Clear();

            foreach (var item in ChumonViewModel.ChumonJisseki.ChumonJissekiMeisais) {
                item.ShiireMaster = null;
            }

            (ChumonJisseki chumonJisseki, int entities, bool isValid, ErrDef errCd)
                = chumonService.ChumonCommit(ChumonViewModel.ChumonJisseki);

            var chumonViewModel = new ChumonViewModel {
                ChumonJisseki = chumonJisseki,
                IsNormal = isValid,
                Remark = errCd == ErrDef.NormalUpdate && entities > 0 || errCd != ErrDef.NormalUpdate ? Message().SetMessage(errCd).MessageText : null
            };
            KeepObject();
            return View(chumonViewModel);
        }

        public ChumonKeysViewModel SetChumonKeysViewModel() {
            var list = _context.ShiireSakiMaster.OrderBy(s => s.ShiireSakiId).Select(s => new SelectListItem { Value = s.ShiireSakiId, Text = s.ShiireSakiId + " " + s.ShiireSakiKaisya }).ToList();

            return (new ChumonKeysViewModel() {
                ShiireSakiId = null,
                ChumonDate = DateOnly.FromDateTime(DateTime.Today),
                ShiireSakiList = list
            });
        }

    }
}