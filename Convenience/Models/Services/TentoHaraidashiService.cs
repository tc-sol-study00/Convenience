using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            //var xxx = await TentoHaraidashi.TentoHaraidashiToiawase("20240930-13-001");

            //var yyy = TransferToDisplayModel(xxx);

            var idList = await TentoHaraidashi.CreateListWithTentoHaraidashiId(-5);
            idList.Insert(0, new { HaraidashiDateTime=CurrentDateTime, TentoHaraidashiId = string.Empty });

            List<SelectListItem> list =new List<SelectListItem>();
            foreach (var rec in idList) {
                string dateString=rec.HaraidashiDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                list.Add(new SelectListItem($"{dateString}:{rec.TentoHaraidashiId??string.Empty}", $"{dateString}:{rec.TentoHaraidashiId ?? string.Empty}"));
            }
            return new TentoHaraidashiViewModel() {
                HaraidashiDateAndId = string.Empty,
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
            DateTime CurrentDateTime = argTentoHaraidashiViewModel.HaraidashiDateAndId.;
            await TentoHaraidashi.TentoHaraidashiSakusei(CurrentDateTime);
            return new TentoHaraidashiViewModel();
        }
        /// <summary>
        /// 店頭払出Commit
        /// </summary>
        /// <param name="argTentoHaraidashiViewModel">店頭払出ビューモデル</param>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public async Task<TentoHaraidashiViewModel> TentoHaraidashiCommit(TentoHaraidashiViewModel argTentoHaraidashiViewModel) {

            TentoHaraidashiHeader settingTentoHaraidashiHearder = await TentoHaraidashi.TentoHaraidashiToiawase("20240930-13-001");

            if (settingTentoHaraidashiHearder == null) {
                var postedHaraidashiDate = argTentoHaraidashiViewModel.HaraidashiDate;
                settingTentoHaraidashiHearder = await TentoHaraidashi.TentoHaraidashiSakusei(postedHaraidashiDate);
            }

            foreach (var shohinmaster in argTentoHaraidashiViewModel.ShohinMasters) {
                foreach (var shiiremaster in shohinmaster.ShiireMasters) {
                    foreach (var tentoharaidashi in shiiremaster.TentoHaraidashiJissekis) {
                        var pickupTentoHaraidashiJisseki = settingTentoHaraidashiHearder.TentoHaraidashiJissekis
                            .Where(x => x.TentoHaraidashiId == tentoharaidashi.TentoHaraidashiId &&
                                x.ShiireSakiId == tentoharaidashi.ShiireSakiId &&
                                x.ShiirePrdId == tentoharaidashi.ShiirePrdId &&
                                x.ShohinId == tentoharaidashi.ShohinId).FirstOrDefault();

                        pickupTentoHaraidashiJisseki.HaraidashiCaseSu = tentoharaidashi.HaraidashiCaseSu;
                        pickupTentoHaraidashiJisseki.HaraidashiSu = tentoharaidashi.HaraidashiCaseSu * shiiremaster.ShiirePcsPerUnit;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return new TentoHaraidashiViewModel();
        }

        public List<ShohinMaster> TransferToDisplayModel(TentoHaraidashiHeader argTentoHaraidashiHeader) {
            List<ShohinMaster> shohinmaster = argTentoHaraidashiHeader.TentoHaraidashiJissekis.GroupBy(x => x.ShiireMaster.ShohinMaster).Select(x => x.Key)
            .ToList();

            return shohinmaster;
        }
    }
}
