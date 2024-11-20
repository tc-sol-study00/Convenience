using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static Convenience.Models.Properties.Config.Message;

namespace Convenience.Models.Services {
    /// <summary>
    /// 店頭払出サービスクラス
    /// </summary>
    public class TentoHaraidashiService : ITentoHaraidashiService, ISharedTools {

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        //店頭払出クラス用
        private ITentoHaraidashi TentoHaraidashi { get; set; }

        /// <summary>
        /// 店頭払出ビュー・モデル（プロパティ）
        /// </summary>
        public TentoHaraidashiViewModel TentoHaraidashiViewModel { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public TentoHaraidashiService(ConvenienceContext context, ITentoHaraidashi tentoHaraidashi) {
            this._context = context;
            this.TentoHaraidashi = tentoHaraidashi;
            this.TentoHaraidashiViewModel = new TentoHaraidashiViewModel();
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
                .Insert(0, new HaraidashiDateTimeAndIdMatching() { HaraidashiDateTime = CurrentDateTime, TentoHaraidashiId = null });
            IList<SelectListItem> selectListItem = MakeListWithTentoHaraidashiIdToSelectListItem(HaraidashiDateTimeAndIdMatchings);

            /*
             *  キー入力用にViewModelを設定
             */
            HaraidashiDateTimeAndIdMatching KeyInputListTop = HaraidashiDateTimeAndIdMatchings[0] ?? throw new InvalidDataException("キー入力用リストエラー");

            return this.TentoHaraidashiViewModel = new() {
                HaraidashiDateAndId = JsonSerializer.Serialize(KeyInputListTop),
                ShohinMasters = default,
                TentoHaraidashiIdList = selectListItem
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
            _ = argTentoHaraidashiViewModel?.HaraidashiDateAndId ?? throw new ArgumentException("店頭払出ビューモデルがセットされていません");
            HaraidashiDateTimeAndIdMatching? haraidashiDateTimeAndIdMatching
                = JsonSerializer.Deserialize<HaraidashiDateTimeAndIdMatching>(argTentoHaraidashiViewModel.HaraidashiDateAndId)
                ?? throw new InvalidDataException("店頭払出日時コードがセットされていません");

            //店頭払出日時            
            DateTime postedHaraidashiDateTime = 
                haraidashiDateTimeAndIdMatching.HaraidashiDateTime > DateTime.MinValue ? 
                haraidashiDateTimeAndIdMatching.HaraidashiDateTime : throw new InvalidDataException("払出日時がセットされていません");
            //店頭払出コード(新規の時はnull)
            string? postedTentoHaraidashiId = haraidashiDateTimeAndIdMatching.TentoHaraidashiId;

            /*
             * 店頭払出ヘッダー＋実績のセット
             */
            TentoHaraidashiHeader? tentoHaraidashiHeader;
            if (postedTentoHaraidashiId == default) //Postデータの店頭払出コードがnull＝新規
            {
                /*
                 * 店頭払出ヘッダ＋実績作成（新規の場合）
                 */
                tentoHaraidashiHeader = await TentoHaraidashi.TentoHaraidashiSakusei(postedHaraidashiDateTime);
            }
            else {
                /*
                 * 店頭払出ヘッダ＋実績問い合わせ（登録済みデータ編集の場合）
                 */
                tentoHaraidashiHeader = await TentoHaraidashi.TentoHaraidashiToiawase(postedTentoHaraidashiId);
            }

            /*
             * 商品マスタリストから店頭払出情報を参照するようにモデル組み換え
             */
            IList<ShohinMaster> shohinmasters = tentoHaraidashiHeader != null ?
            TransferToDisplayModel(tentoHaraidashiHeader).ToList() : throw new NoDataFoundException(" 店頭払出ヘッダ＋実績のデータがありません");

            /*
             * キー入力用リスト設定
             * 第１画面で選択されたキーでセレクトリストを絞り込んでおく
             */
            IList<HaraidashiDateTimeAndIdMatching> HaraidashiDateTimeAndIdMatchings = new List<HaraidashiDateTimeAndIdMatching> {
                haraidashiDateTimeAndIdMatching
            };
            IList<SelectListItem> selectListItems = MakeListWithTentoHaraidashiIdToSelectListItem(HaraidashiDateTimeAndIdMatchings);

            return this.TentoHaraidashiViewModel = new() {
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
            _ = argTentoHaraidashiViewModel?.HaraidashiDateAndId ?? throw new ArgumentException("店頭払出ビューモデルがセットされていません");
            HaraidashiDateTimeAndIdMatching haraidashiDateTimeAndIdMatching
                = JsonSerializer.Deserialize<HaraidashiDateTimeAndIdMatching>(argTentoHaraidashiViewModel.HaraidashiDateAndId)
                ?? throw new InvalidDataException("店頭払出日時コードがセットされていません");

            //店頭払出日時 
            DateTime postedHaraidashiDateTime = haraidashiDateTimeAndIdMatching.HaraidashiDateTime;
            //店頭払出コード
            string tentoHaraidashiId = argTentoHaraidashiViewModel?.ShohinMasters?
                .Where(x => x.ShiireMasters != null)
                .SelectMany(x => x.ShiireMasters!)
                .Where(x => x.TentoHaraidashiJissekis != null)
                .SelectMany(x => x.TentoHaraidashiJissekis!)
                .Min(x => x.TentoHaraidashiId)
                ?? throw new NoDataFoundException("店頭払出コードがセットされていません");

            /*
             * 店頭払出ヘッダ＋実績問い合わせ(Postデータ更新用ベース）
             */
            TentoHaraidashiHeader? settingTentoHaraidashiHearder = await TentoHaraidashi.TentoHaraidashiToiawase(tentoHaraidashiId);

            /*
             * 店頭払出ヘッダ＋実績作成(Postデータ更新用ベース）
             */

            if (ISharedTools.IsNotExistCheck(settingTentoHaraidashiHearder)) {
                settingTentoHaraidashiHearder = await TentoHaraidashi.TentoHaraidashiSakusei(postedHaraidashiDateTime);
            }
            /*
             * Postデータから店頭払出実績を抽出する
             */

            List<TentoHaraidashiJisseki> postedTentoHaraidashiJissekis
                = argTentoHaraidashiViewModel.ShohinMasters.SelectMany(sm => sm.ShiireMasters!.SelectMany(sim => sim.TentoHaraidashiJissekis!)).ToList();

            /*
             * Postデータを上書きしてＤＢ更新準備をする
             */
            settingTentoHaraidashiHearder!.TentoHaraidashiJissekis= TentoHaraidashi.TentoHaraidashiUpdate(postedTentoHaraidashiJissekis);

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
            TentoHaraidashiHeader? queriedTentoHaraidashiHearder = await TentoHaraidashi.TentoHaraidashiToiawase(tentoHaraidashiId)
                ?? throw new NoDataFoundException("店頭払出実績がありません");
            IList<ShohinMaster> shohinmasters = TransferToDisplayModel(queriedTentoHaraidashiHearder).ToList();
            /*
             * キー入力用リスト設定
             * 第１画面で選択されたキーでセレクトリストを絞り込んでおく
            */
            IList<HaraidashiDateTimeAndIdMatching> haraidashiDateTimeAndIdMatchings = new List<HaraidashiDateTimeAndIdMatching> {
                haraidashiDateTimeAndIdMatching
            };
            IList<SelectListItem> selectListItems = MakeListWithTentoHaraidashiIdToSelectListItem(haraidashiDateTimeAndIdMatchings);

            /*
             * DB更新処理結果を設定（表示用）
             */
            (bool IsValid, ErrDef errCd) = (true, ErrDef.NormalUpdate);

            return this.TentoHaraidashiViewModel = new () {
                HaraidashiDateAndId = argTentoHaraidashiViewModel.HaraidashiDateAndId,
                ShohinMasters = shohinmasters,
                TentoHaraidashiIdList = selectListItems,
                IsNormal = IsValid,
                Remark = errCd == ErrDef.DataValid && entities > 0 || errCd != ErrDef.DataValid ? new Message().SetMessage(ErrDef.NormalUpdate)?.MessageText??string.Empty : null
            };
        }
        /// <summary>
        /// 商品マスタを軸に一覧構造をコンバート
        /// </summary>
        /// <param name="argTentoHaraidashiHeader"></param>
        /// <returns></returns>
        private static IEnumerable<ShohinMaster> TransferToDisplayModel(TentoHaraidashiHeader argTentoHaraidashiHeader) {
            IEnumerable<ShohinMaster> shohinmasters
                = argTentoHaraidashiHeader.TentoHaraidashiJissekis.GroupBy(x => x.ShiireMaster?.ShohinMaster)
                    .Select(x => x.Key!)
                    .Where(x => x.ShiireMasters != null && x.ShohinId != null)
                    .OrderBy(x => x.ShohinId)
                    .ThenBy(x => x.ShiireMasters?.FirstOrDefault()?.ShiireSakiId)
                    .ThenBy(x => x.ShiireMasters?.FirstOrDefault()?.ShiirePrdId);
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
            IList<HaraidashiDateTimeAndIdMatching> HaraidashiDateTimeAndIdMatchings =
                await TentoHaraidashi.TentoHaraidashiHeaderList(x => x.HaraidashiDateTime >= DateTime.Now.AddDays(argReverseDaysWithMinus).Date.ToUniversalTime())
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
        private static IList<SelectListItem> MakeListWithTentoHaraidashiIdToSelectListItem(IEnumerable<HaraidashiDateTimeAndIdMatching> argHaraidashiDateTimeAndIdMatching) {
            // DateTime を日本標準時（JST）でフォーマット
            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

            /*
             * 店頭払出日と店頭払出コードのリストをセレクトアイテムリストに変換する
             */
            IList<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (HaraidashiDateTimeAndIdMatching item in argHaraidashiDateTimeAndIdMatching) {
                string serializedString = JsonSerializer.Serialize(item);
                DateTime jstTime = TimeZoneInfo.ConvertTime(item.HaraidashiDateTime, jstZone);
                selectListItems.Add(new SelectListItem(
                    $"{item.TentoHaraidashiId ?? "🆕新規" + new string('-', 11)}:{jstTime:yyyy/MM/dd HH:mm:ss}", serializedString));
            }
            return selectListItems;
        }
    }
}
