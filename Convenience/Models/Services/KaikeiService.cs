using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Kaikei;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Convenience.Models.Properties.Message;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Convenience.Models.Services {
    public class KaikeiService : IKaikeiService  {

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        //会計クラス用
        private IKaikei kaikei { get; set; }

        /// <summary>
        /// 会計ビュー・モデル（プロパティ）
        /// </summary>
        public KaikeiViewModel KaikeiViewModel { get; set; }

        /// <summary>
        /// TempData用
        /// </summary>
        private readonly ITempDataDictionary _tempData;

        /// <summary>
        /// 会計ビューモデル設定
        /// </summary>
        /// <returns>TentoHaraidashiViewModel 会計ビューモデル</returns>
        public KaikeiService(ConvenienceContext context, ITempDataDictionaryFactory tempDataFactory, IActionContextAccessor actionContextAccessor) {
            this._context = context;
            this.kaikei = new Kaikei(_context);
            _tempData = tempDataFactory.GetTempData(actionContextAccessor.ActionContext.HttpContext);
        }
        /// <summary>
        /// 会計ビューモデル設定
        /// </summary>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public async Task<KaikeiViewModel> SetKaikeiViewModel() {

            DateTime CurrentDateTime= DateTime.Now;

            IList<UriageDateTimeAndIdMatching> uriageDateTimeAndIdMatchings;
            uriageDateTimeAndIdMatchings = _context.KaikeiHeader.Where(x => x.UriageDatetime >= DateTime.Now.AddDays(-5).Date.ToUniversalTime())
                .OrderByDescending(x => x.UriageDatetime)
                .Select(x => new UriageDateTimeAndIdMatching { UriageDatetime = x.UriageDatetime, UriageDatetimeId = x.UriageDatetimeId })
                .ToList();

            uriageDateTimeAndIdMatchings.Insert(0,new UriageDateTimeAndIdMatching() {
                UriageDatetime = CurrentDateTime,
                UriageDatetimeId = null
            });

            string serializedString=default;
            var kaikeiHeaderList = new List<SelectListItem>();
            foreach (var item in uriageDateTimeAndIdMatchings) {
                serializedString = JsonSerializer.Serialize(item);
                kaikeiHeaderList.Add(new SelectListItem($"{item.UriageDatetimeId??"新規"}:{item.UriageDatetime}", serializedString));
            }

            this.KaikeiViewModel = new KaikeiViewModel() {
                KaikeiDateAndId = serializedString,
                KaikeiHeaderList = kaikeiHeaderList
            };
            SetViewModelToTempData(this.KaikeiViewModel);
            return this.KaikeiViewModel;
        }

        /// <summary>
        /// 会計セッティング
        /// </summary>
        /// <param name="argKaikeiViewModel">会計ビューモデル</param>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public async Task<KaikeiViewModel> KaikeiSetting(KaikeiViewModel argKaikeiViewModel) {
            UriageDateTimeAndIdMatching uriageDateTimeAndIdMatching 
                = JsonSerializer.Deserialize<UriageDateTimeAndIdMatching>(argKaikeiViewModel.KaikeiDateAndId);
            var uriageDatetimeId = uriageDateTimeAndIdMatching.UriageDatetimeId;
            var uriageDatetime = uriageDateTimeAndIdMatching.UriageDatetime;

            KaikeiHeader kaikeiHeader = default;
            if (uriageDatetimeId is null) {
                kaikeiHeader = await kaikei.KaikeiSakusei(uriageDatetime);
            }
            else {
                kaikeiHeader = await kaikei.KaikeiToiawase(uriageDatetimeId);
            }

            IList<UriageDateTimeAndIdMatching> uriageDateTimeAndIdMatchings= new List<UriageDateTimeAndIdMatching>();
            uriageDateTimeAndIdMatchings.Add(uriageDateTimeAndIdMatching);
            
            var kaikeiHeaderList = new List<SelectListItem>();
            foreach (var item in uriageDateTimeAndIdMatchings) {
                string serializedString = JsonSerializer.Serialize(item);
                kaikeiHeaderList.Add(new SelectListItem($"{item.UriageDatetimeId??"新規"}:{item.UriageDatetime}", serializedString));
            }

            IList<SelectListItem> shohinList= new List<SelectListItem>();
            shohinList = _context.ShohinMaster
                .OrderBy(x => x.ShohinId)
                .Select(x => new SelectListItem { Value = x.ShohinId, Text = $"{x.ShohinId}:{x.ShohinName}" })
                .ToList();

            this.KaikeiViewModel = new KaikeiViewModel() {
                KaikeiDateAndId = argKaikeiViewModel.KaikeiDateAndId,
                KaikeiHeaderList = kaikeiHeaderList,
                KaikeiHeader = kaikeiHeader,
                ShohinList= shohinList
            };

            SetViewModelToTempData(this.KaikeiViewModel);
            return this.KaikeiViewModel;

        }

        /// <summary>
        /// 会計Add
        /// </summary>
        /// <param name="argKaikeiViewModel">会計ビューモデル</param>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public async Task<KaikeiViewModel> KaikeiAdd(KaikeiViewModel argKaikeiViewModel) {

            this.KaikeiViewModel = GetViewModelToTempData();
            kaikei.KaikeiHeader = this.KaikeiViewModel.KaikeiHeader;

            if (this.KaikeiViewModel.KaikeiHeader.KaikeiJissekis is null) {
                this.KaikeiViewModel.KaikeiHeader.KaikeiJissekis = new List<KaikeiJisseki>();
            }

            var kaikeiJissekiforAdd = argKaikeiViewModel.KaikeiJissekiforAdd;


            this.KaikeiViewModel.KaikeiHeader.KaikeiJissekis 
                = await kaikei.KaikeiAddcommodity(argKaikeiViewModel.KaikeiJissekiforAdd);

            SetViewModelToTempData(this.KaikeiViewModel);

            return this.KaikeiViewModel;
        }

        /// <summary>
        /// 会計Commit
        /// </summary>
        /// <param name="argKaikeiViewModel">会計ビューモデル</param>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public async Task<KaikeiViewModel> KaikeiCommit(KaikeiViewModel argKaikeiViewModel) {

            var postedKaikeiHeader=argKaikeiViewModel.KaikeiHeader;

            var updatedKaikeiHeader = await kaikei.KaikeiUpdate(postedKaikeiHeader);
            var postedUriageDatetimeId = updatedKaikeiHeader.UriageDatetimeId;

            _context.SaveChanges();

            argKaikeiViewModel.KaikeiHeader=await kaikei.KaikeiToiawase(postedUriageDatetimeId);

            argKaikeiViewModel.IsNormal = true;
            argKaikeiViewModel.Remark = new Message().SetMessage(ErrDef.NormalUpdate).MessageText;

            return argKaikeiViewModel;
        }
        private void SetViewModelToTempData(KaikeiViewModel argKaikeiViewModel) {
            string serializedString = JsonSerializer.Serialize(argKaikeiViewModel, new JsonSerializerOptions() {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true,
            });
            _tempData["test"] = serializedString;
        }
        private KaikeiViewModel GetViewModelToTempData() {
            KaikeiViewModel kaikeiViewModel = JsonSerializer.Deserialize<KaikeiViewModel>((string)_tempData["test"]);
            return kaikeiViewModel;
        }

    }

}
