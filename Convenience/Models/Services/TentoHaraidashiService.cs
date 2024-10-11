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
    /// <summary>
    /// 店頭払出サービスクラス
    /// </summary>
    public class TentoHaraidashiService : ITentoHaraidashiService {

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        //店頭払出クラス用
        private ITentoHaraidashi _tentoHaraidashi { get; set; }

        /// <summary>
        /// 店頭払出ビュー・モデル（プロパティ）
        /// </summary>
        public TentoHaraidashiViewModel TentoHaraidashiViewModel { get; set; }

        /// <summary>
        /// 店頭払出ビューモデル設定
        /// </summary>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public TentoHaraidashiService(ConvenienceContext context, ITentoHaraidashi tentoHaraidashi) {
            this._context = context;
            this._tentoHaraidashi = tentoHaraidashi;
        }
        /// <summary>
        /// 初期表示用
        /// </summary>
        /// <returns></returns>
        public async Task<TentoHaraidashiViewModel> SetTentoHaraidashiViewModel() {
            //現在時刻
            DateTime CurrentDateTime = DateTime.Now;

            /*
             * キー入力用リスト設定
             */
            IList<HaraidashiDateTimeAndIdMatching> HaraidashiDateTimeAndIdMatchings = await CreateListWithTentoHaraidashiId(-5);
            HaraidashiDateTimeAndIdMatchings
                .Insert(0, new HaraidashiDateTimeAndIdMatching() { HaraidashiDateTime=CurrentDateTime, TentoHaraidashiId = null });
            IList<SelectListItem> selectListItem=MakeListWithTentoHaraidashiIdToSelectListItem(HaraidashiDateTimeAndIdMatchings);

            /*
             *  キー入力用にViewModelを設定
             */
            return this.TentoHaraidashiViewModel = new TentoHaraidashiViewModel() {
                HaraidashiDateAndId = JsonSerializer.Serialize(HaraidashiDateTimeAndIdMatchings[0]),
                ShohinMasters = default,
                TentoHaraidashiIdList= selectListItem
            };
        }

        /// <summary>
        /// 店頭払出セッティング
        /// </summary>
        /// <param name="argTentoHaraidashiViewModel">店頭払出ビューモデル</param>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public async Task<TentoHaraidashiViewModel> TentoHaraidashiSetting(TentoHaraidashiViewModel argTentoHaraidashiViewModel) {
            /*
             * 店頭払出日時＋コードの取得
             */
            HaraidashiDateTimeAndIdMatching haraidashiDateTimeAndIdMatching
                =JsonSerializer.Deserialize<HaraidashiDateTimeAndIdMatching>(argTentoHaraidashiViewModel.HaraidashiDateAndId);
            //店頭払出日時            
            DateTime postedHaraidashiDateTime = haraidashiDateTimeAndIdMatching.HaraidashiDateTime;
            //店頭払出コード
            string postedTentoHaraidashiId = haraidashiDateTimeAndIdMatching.TentoHaraidashiId;

            /*
             * 店頭払出ヘッダー＋実績のセット
             */
            TentoHaraidashiHeader tentoHaraidashiHeader = default;
            if (postedTentoHaraidashiId is null) //Postデータの店頭払出コードがnull＝新規
            {
                /*
                 * 店頭払出ヘッダ＋実績作成（新規の場合）
                 */
                tentoHaraidashiHeader = await _tentoHaraidashi.TentoHaraidashiSakusei(postedHaraidashiDateTime);
            }
            else
            {
                /*
                 * 店頭払出ヘッダ＋実績問い合わせ（登録済みデータ編集の場合）
                 */
                tentoHaraidashiHeader = await _tentoHaraidashi.TentoHaraidashiToiawase(postedTentoHaraidashiId);
            }

            /*
             * 商品マスタリストから店頭払出情報を参照するようにモデル組み換え
             */
            IList<ShohinMaster> shohinmasters=TransferToDisplayModel(tentoHaraidashiHeader).ToList();

            /*
             * キー入力用リスト設定
             * 第１画面で選択されたキーでセレクトリストを絞り込んでおく
             */
            IList<HaraidashiDateTimeAndIdMatching> HaraidashiDateTimeAndIdMatchings = new List<HaraidashiDateTimeAndIdMatching>();
            HaraidashiDateTimeAndIdMatchings.Add(haraidashiDateTimeAndIdMatching);
            IList<SelectListItem> selectListItems = MakeListWithTentoHaraidashiIdToSelectListItem(HaraidashiDateTimeAndIdMatchings);

            return this.TentoHaraidashiViewModel = new TentoHaraidashiViewModel() {
                HaraidashiDateAndId = argTentoHaraidashiViewModel.HaraidashiDateAndId,
                ShohinMasters = shohinmasters,
                TentoHaraidashiIdList = selectListItems
            };
        }

        /// <summary>
        /// 店頭払出Commit
        /// </summary>
        /// <param name="argTentoHaraidashiViewModel">店頭払出ビューモデル</param>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public async Task<TentoHaraidashiViewModel> TentoHaraidashiCommit(TentoHaraidashiViewModel argTentoHaraidashiViewModel) {
            /*
             * 店頭払出日時＋コードの取得
             */
            HaraidashiDateTimeAndIdMatching haraidashiDateTimeAndIdMatching
                = JsonSerializer.Deserialize<HaraidashiDateTimeAndIdMatching>(argTentoHaraidashiViewModel.HaraidashiDateAndId);

            //店頭払出日時 
            DateTime postedHaraidashiDateTime = haraidashiDateTimeAndIdMatching.HaraidashiDateTime;
            //店頭払出コード
            string tentoHaraidashiId = argTentoHaraidashiViewModel.ShohinMasters.SelectMany(x => x.ShiireMasters).SelectMany(x => x.TentoHaraidashiJissekis).Min(x => x.TentoHaraidashiId);

            /*
             * 店頭払出ヘッダ＋実績問い合わせ(Postデータ更新用ベース）
             */
            TentoHaraidashiHeader settingTentoHaraidashiHearder = await _tentoHaraidashi.TentoHaraidashiToiawase(tentoHaraidashiId);

            if (settingTentoHaraidashiHearder == null) {    //上記問い合わせデータなし
                /*
                 * 店頭払出ヘッダ＋実績作成(Postデータ更新用ベース）
                 */
                settingTentoHaraidashiHearder = await _tentoHaraidashi.TentoHaraidashiSakusei(postedHaraidashiDateTime);
            }

            /*
             * Postデータ更新用ベースにポストデータを上乗せする
             */
            foreach (var shohinmaster in argTentoHaraidashiViewModel.ShohinMasters) {       //Postされた商品マスタ
                foreach (var shiiremaster in shohinmaster.ShiireMasters) {                  //Postされた仕入マスタ
                    foreach (var tentoharaidashi in shiiremaster.TentoHaraidashiJissekis) { //Postされた店頭払出実績
                        var pickupTentoHaraidashiJisseki = settingTentoHaraidashiHearder.TentoHaraidashiJissekis    //上乗せ処理用に更新用ベースを検索
                            .Where(x => x.TentoHaraidashiId == tentoharaidashi.TentoHaraidashiId &&
                                x.ShiireSakiId == tentoharaidashi.ShiireSakiId &&
                                x.ShiirePrdId == tentoharaidashi.ShiirePrdId &&
                                x.ShohinId == tentoharaidashi.ShohinId).FirstOrDefault();
                        //上乗せ処理
                        var wHaraidashiCaseSu = pickupTentoHaraidashiJisseki.HaraidashiCaseSu;      //上乗せ前のデータを退避
                        pickupTentoHaraidashiJisseki.HaraidashiCaseSu += tentoharaidashi.HaraidashiCaseSu-wHaraidashiCaseSu;    //払出ケース数    
                        pickupTentoHaraidashiJisseki.HaraidashiSu += 
                            (tentoharaidashi.HaraidashiCaseSu - wHaraidashiCaseSu) * pickupTentoHaraidashiJisseki.ShiireMaster.ShiirePcsPerUnit;
                                                                                                                                //払出数
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
            /*
             * 更新エンティティ数を求める
             */
            int entities = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity).Count();
            
            /*
             * DB更新
             */
            await _context.SaveChangesAsync();

            /* 
             * DB更新後、再読み込み
             */
            haraidashiDateTimeAndIdMatching.TentoHaraidashiId = settingTentoHaraidashiHearder.TentoHaraidashiId;
            TentoHaraidashiHeader? queriedTentoHaraidashiHearder = await _tentoHaraidashi.TentoHaraidashiToiawase(tentoHaraidashiId);
            IList<ShohinMaster> shohinmasters = TransferToDisplayModel(queriedTentoHaraidashiHearder).ToList();
            /*
             * キー入力用リスト設定
             * 第１画面で選択されたキーでセレクトリストを絞り込んでおく
            */
            IList<HaraidashiDateTimeAndIdMatching> haraidashiDateTimeAndIdMatchings = new List<HaraidashiDateTimeAndIdMatching>();
            haraidashiDateTimeAndIdMatchings.Add(haraidashiDateTimeAndIdMatching);
            IList<SelectListItem> selectListItems = MakeListWithTentoHaraidashiIdToSelectListItem(haraidashiDateTimeAndIdMatchings);

            /*
             * DB更新処理結果を設定（表示用）
             */
            (bool IsValid, ErrDef errCd) = (true, ErrDef.NormalUpdate);

            return this.TentoHaraidashiViewModel = new TentoHaraidashiViewModel() {
                HaraidashiDateAndId = argTentoHaraidashiViewModel.HaraidashiDateAndId,
                ShohinMasters = shohinmasters,
                TentoHaraidashiIdList = selectListItems,
                IsNormal = IsValid,
                Remark = errCd == ErrDef.DataValid && entities > 0 || errCd != ErrDef.DataValid ? new Message().SetMessage(ErrDef.NormalUpdate).MessageText : null
            };
        }
        /// <summary>
        /// 商品マスタを軸に一覧構造をコンバート
        /// </summary>
        /// <param name="argTentoHaraidashiHeader"></param>
        /// <returns></returns>
        private IEnumerable<ShohinMaster> TransferToDisplayModel(TentoHaraidashiHeader argTentoHaraidashiHeader) {
            IEnumerable<ShohinMaster?> shohinmasters
                = argTentoHaraidashiHeader.TentoHaraidashiJissekis.GroupBy(x => x.ShiireMaster.ShohinMaster).Select(x => x.Key)
                    .OrderBy(x => x.ShohinId)
                    .ThenBy(x => x.ShiireMasters.FirstOrDefault().ShiireSakiId)
                    .ThenBy(x => x.ShiireMasters.FirstOrDefault().ShiirePrdId);
            return shohinmasters;
        }

        /// <summary>
        /// 引数（マイナス値）を使用して、その引数分の日数をさかのぼり、店頭払出日付と店頭払出コード一覧を作る
        /// </summary>
        /// <param name="argReverseDaysWithMinus">さかのぼる日数（マイナスでいれる）</param>
        /// <returns>店頭払出日時・店頭払出コード</returns>
        private async Task<IList<HaraidashiDateTimeAndIdMatching>> CreateListWithTentoHaraidashiId(int argReverseDaysWithMinus) {
            /*
             * 店頭払出ヘッダーを引数日数分さかのぼり、一覧を作る
             */
            IList<HaraidashiDateTimeAndIdMatching> HaraidashiDateTimeAndIdMatchings = await _context.TentoHaraidashiHearder
               .Where(x => x.HaraidashiDateTime >= DateTime.Now.AddDays(argReverseDaysWithMinus).Date.ToUniversalTime())
               .OrderByDescending(x => x.HaraidashiDateTime)
               .Select(x => new HaraidashiDateTimeAndIdMatching() { HaraidashiDateTime = x.HaraidashiDateTime, TentoHaraidashiId = x.TentoHaraidashiId })
               .ToListAsync();
            return HaraidashiDateTimeAndIdMatchings;
        }

        /// <summary>
        /// Ｖｉｅｗのselect項目のために店頭払出日付＋コードのリストを作成する
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        private IList<SelectListItem> MakeListWithTentoHaraidashiIdToSelectListItem(IEnumerable<HaraidashiDateTimeAndIdMatching> argHaraidashiDateTimeAndIdMatching) {
            // DateTime を日本標準時（JST）でフォーマット
            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            
            /*
             * 店頭払出日と店頭払出コードのリストをセレクトアイテムリストに変換する
             */
            IList<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var item in argHaraidashiDateTimeAndIdMatching) {
                string serializedString = JsonSerializer.Serialize(item);
                DateTime jstTime = TimeZoneInfo.ConvertTime(item.HaraidashiDateTime, jstZone);
                selectListItems.Add(new SelectListItem(
                    $"{item.TentoHaraidashiId?? "🆕新規"+ new string('-', 11)}:{jstTime.ToString("yyyy/MM/dd HH:mm:ss")}", serializedString));
            }
            return selectListItems;
        }
    }
}
