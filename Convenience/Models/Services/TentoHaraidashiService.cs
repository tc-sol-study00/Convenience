using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using static Convenience.Models.Properties.Message;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Convenience.Models.Services {
    public class TentoHaraidashiService {

        //DBコンテキスト
        private readonly ConvenienceContext _context;

        //店頭払出クラス用
        public TentoHaraidashi TentoHaraidashi { get; set; }
        /// <summary>
        /// 店頭払出ビューモデル設定
        /// </summary>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public TentoHaraidashiService(ConvenienceContext context) {
            this._context = context;
            this.TentoHaraidashi = new TentoHaraidashi(_context);
        }
        /// <summary>
        /// 初期表示用
        /// </summary>
        /// <returns></returns>
        public async Task<TentoHaraidashiViewModel> SetTentoHaraidashiViewModel() {
            DateTime CurrentDateTime = DateTime.Now;

            IList<ShohinMaster> shohinhmasters = default;

            var idList = await CreateListWithTentoHaraidashiId(-5);
            idList.Insert(0, new HaraidashiDateTimeAndIdMatching() { HaraidashiDateTime=CurrentDateTime, TentoHaraidashiId = null });

            var list=MakeListWithTentoHaraidashiIdToSelectListItem(idList);

            return new TentoHaraidashiViewModel() {
                HaraidashiDateAndId = JsonSerializer.Serialize(idList[0]),
                ShohinMasters = shohinhmasters,
                TentoHaraidashiIdList= list
            };
        }

        /// <summary>
        /// 店頭払出セッティング
        /// </summary>
        /// <param name="argTentoHaraidashiViewModel">店頭払出ビューモデル</param>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public async Task<TentoHaraidashiViewModel> TentoHaraidashiSetting(TentoHaraidashiViewModel argTentoHaraidashiViewModel) {
            HaraidashiDateTimeAndIdMatching haraidashiDateTimeAndIdMatching
                =JsonSerializer.Deserialize<HaraidashiDateTimeAndIdMatching>(argTentoHaraidashiViewModel.HaraidashiDateAndId);

            DateTime postedHaraidashiDateTime = haraidashiDateTimeAndIdMatching.HaraidashiDateTime;
            TentoHaraidashiHeader tentoHaraidashiHeader = await TentoHaraidashi.TentoHaraidashiSakusei(postedHaraidashiDateTime);

            IList<ShohinMaster> shohinmasters=TransferToDisplayModel(tentoHaraidashiHeader);

            var idList = new List<HaraidashiDateTimeAndIdMatching>();
            idList.Add(haraidashiDateTimeAndIdMatching);

            var list = MakeListWithTentoHaraidashiIdToSelectListItem(idList);

            return new TentoHaraidashiViewModel() {
                HaraidashiDateAndId = argTentoHaraidashiViewModel.HaraidashiDateAndId,
                ShohinMasters = shohinmasters,
                TentoHaraidashiIdList = list
            };
        }

        /// <summary>
        /// 店頭払出Commit
        /// </summary>
        /// <param name="argTentoHaraidashiViewModel">店頭払出ビューモデル</param>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public async Task<TentoHaraidashiViewModel> TentoHaraidashiCommit(TentoHaraidashiViewModel argTentoHaraidashiViewModel) {

            HaraidashiDateTimeAndIdMatching haraidashiDateTimeAndIdMatching
                = JsonSerializer.Deserialize<HaraidashiDateTimeAndIdMatching>(argTentoHaraidashiViewModel.HaraidashiDateAndId);

            DateTime postedHaraidashiDateTime = haraidashiDateTimeAndIdMatching.HaraidashiDateTime;

            string tentoHaraidashiId = argTentoHaraidashiViewModel.ShohinMasters.SelectMany(x => x.ShiireMasters).SelectMany(x => x.TentoHaraidashiJissekis).Min(x => x.TentoHaraidashiId);

            TentoHaraidashiHeader settingTentoHaraidashiHearder = await TentoHaraidashi.TentoHaraidashiToiawase(tentoHaraidashiId);

            if (settingTentoHaraidashiHearder == null) {
                settingTentoHaraidashiHearder = await TentoHaraidashi.TentoHaraidashiSakusei(postedHaraidashiDateTime);
            }
            else {
                settingTentoHaraidashiHearder = await TentoHaraidashi.TentoHaraidashiToiawase(tentoHaraidashiId);
            }
            foreach (var shohinmaster in argTentoHaraidashiViewModel.ShohinMasters) {
                foreach (var shiiremaster in shohinmaster.ShiireMasters) {
                    foreach (var tentoharaidashi in shiiremaster.TentoHaraidashiJissekis) {
                        var pickupTentoHaraidashiJisseki = settingTentoHaraidashiHearder.TentoHaraidashiJissekis
                            .Where(x => x.TentoHaraidashiId == tentoharaidashi.TentoHaraidashiId &&
                                x.ShiireSakiId == tentoharaidashi.ShiireSakiId &&
                                x.ShiirePrdId == tentoharaidashi.ShiirePrdId &&
                                x.ShohinId == tentoharaidashi.ShohinId).FirstOrDefault();
                        var wHaraidashiCaseSu = pickupTentoHaraidashiJisseki.HaraidashiCaseSu;
                        pickupTentoHaraidashiJisseki.HaraidashiCaseSu += tentoharaidashi.HaraidashiCaseSu-wHaraidashiCaseSu;
                        pickupTentoHaraidashiJisseki.HaraidashiSu += (tentoharaidashi.HaraidashiCaseSu- wHaraidashiCaseSu) * pickupTentoHaraidashiJisseki.ShiireMaster.ShiirePcsPerUnit;
                        /*
                         * 倉庫在庫調整
                         */
                        ShiireMaster pickupShiireMaster = pickupTentoHaraidashiJisseki.ShiireMaster;
                        ShohinMaster pickupShohinMaster = pickupShiireMaster.ShohinMaster;
                        SokoZaiko pickupSokoZaiko = pickupTentoHaraidashiJisseki.ShiireMaster.SokoZaiko;
                        pickupSokoZaiko.SokoZaikoCaseSu -= tentoharaidashi.HaraidashiCaseSu - wHaraidashiCaseSu;
                        pickupSokoZaiko.SokoZaikoSu -= (tentoharaidashi.HaraidashiCaseSu - wHaraidashiCaseSu) * pickupShiireMaster.ShiirePcsPerUnit;

                        /*
                         * 店頭在庫調整
                         */
                        var pickupTentoZaiko = pickupShohinMaster.TentoZaiko;
                        var wTentoZaiko = pickupTentoZaiko.ZaikoSu;
                        pickupTentoZaiko.ZaikoSu += (tentoharaidashi.HaraidashiCaseSu - wHaraidashiCaseSu) * pickupShiireMaster.ShiirePcsPerUnit;
                    }
                }
            }

            var entities = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity).Count();
            
 
            await _context.SaveChangesAsync();

            var queriedTentoHaraidashiHearder = await TentoHaraidashi.TentoHaraidashiToiawase(tentoHaraidashiId);
            IList<ShohinMaster> shohinmasters= TransferToDisplayModel(queriedTentoHaraidashiHearder);

            var idList = new List<HaraidashiDateTimeAndIdMatching>();
            idList.Add(new HaraidashiDateTimeAndIdMatching() { HaraidashiDateTime = postedHaraidashiDateTime, TentoHaraidashiId = tentoHaraidashiId });

            var list = MakeListWithTentoHaraidashiIdToSelectListItem(idList);

            (bool IsValid, ErrDef errCd) = (true, ErrDef.NormalUpdate);

            return new TentoHaraidashiViewModel() {
                HaraidashiDateAndId = argTentoHaraidashiViewModel.HaraidashiDateAndId,
                ShohinMasters = shohinmasters,
                TentoHaraidashiIdList = list,
                IsNormal = IsValid,
                Remark = errCd == ErrDef.DataValid && entities > 0 || errCd != ErrDef.DataValid ? new Message().SetMessage(ErrDef.NormalUpdate).MessageText : null
            };
        }

        public IList<ShohinMaster> TransferToDisplayModel(TentoHaraidashiHeader argTentoHaraidashiHeader) {
            IList<ShohinMaster> shohinmaster 
                = argTentoHaraidashiHeader.TentoHaraidashiJissekis.GroupBy(x => x.ShiireMaster.ShohinMaster).Select(x => x.Key)
                    .OrderBy(x=>x.ShohinId).ThenBy( x => x.ShiireMasters.FirstOrDefault().ShiireSakiId).ThenBy(x => x.ShiireMasters.FirstOrDefault().ShiirePrdId)
                    .ToList();
            return shohinmaster;
        }

        /// <summary>
        /// 引数（マイナス値）を使用して、その引数分の日数をさかのぼり、店頭払出日付と店頭払出コード一覧を作る
        /// </summary>
        /// <param name="argReverseDaysWithMinus">さかのぼる日数（マイナスでいれる）</param>
        /// <returns>店頭払出日時・店頭払出コード</returns>
        private async Task<List<HaraidashiDateTimeAndIdMatching>> CreateListWithTentoHaraidashiId(int argReverseDaysWithMinus) {
            var listData = await _context.TentoHaraidashiHearder
               .Where(x => x.HaraidashiDateTime >= DateTime.Now.AddDays(argReverseDaysWithMinus).Date.ToUniversalTime())
               .OrderBy(x => x.HaraidashiDateTime)
               .Select(x => new HaraidashiDateTimeAndIdMatching() { HaraidashiDateTime = x.HaraidashiDateTime, TentoHaraidashiId = x.TentoHaraidashiId })
               .ToListAsync();
            return listData;
        }

        /// <summary>
        /// Ｖｉｅｗのselect項目のために店頭払出日付＋コードのリストを作成する
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        private List<SelectListItem> MakeListWithTentoHaraidashiIdToSelectListItem(List<HaraidashiDateTimeAndIdMatching> idList) {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var rec in idList) {
                string serializedString = JsonSerializer.Serialize(rec);
                list.Add(new SelectListItem($"{rec.HaraidashiDateTime.ToString("yyyy/MM/dd HH:mm:ss")}:{rec.TentoHaraidashiId ?? "新規"}", serializedString));
            }
            return list;
        }
}
}
