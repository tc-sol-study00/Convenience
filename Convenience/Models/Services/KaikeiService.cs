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
    /// <summary>
    /// 会計サービスクラス
    /// </summary>
    public class KaikeiService : IKaikeiService {

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// 会計クラス用
        /// </summary>
        private IKaikei Kaikei { get; set; }

        /// <summary>
        /// 会計ビュー・モデル（プロパティ）
        /// </summary>
        public KaikeiViewModel KaikeiViewModel { get; set; }

        /// <summary>
        /// TempData用
        /// </summary>
        private readonly ITempDataDictionary _tempData;

        private const string tempDataIndex = "KaikeiService";
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// <para>DBコンテキスト</para>
        /// <para>会計クラスのＤＩ</para>
        /// <para>コントローラ以外でtempdata扱いが出来ようにするＤＩ（２つ）</para>
        /// </remarks>
        public KaikeiService(
            ConvenienceContext context,
            IKaikei kaikei,
            ITempDataDictionaryFactory tempDataFactory,
            IActionContextAccessor actionContextAccessor) {
            this._context = context;    //DBコンテキスト用
            this.Kaikei = kaikei;      //会計クラス
            this.KaikeiViewModel = new KaikeiViewModel(_context);
            //TemoData用ＤＩ
            if (actionContextAccessor.ActionContext != null && actionContextAccessor.ActionContext.HttpContext != null) { // null チェック
                _tempData = tempDataFactory.GetTempData(actionContextAccessor.ActionContext.HttpContext);
            } else {
                throw new InvalidOperationException("HttpContext is not available.");
            }
        }
        /// <summary>
        /// 会計ビューモデル設定（初期画面用）
        /// </summary>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public async Task<KaikeiViewModel> SetKaikeiViewModel() {

            /*
             * 現在時間
             */
            DateTime CurrentDateTime = DateTime.Now;

            /*
             * ５日前を対象に過去の会計データを抽出し、新規分含めキー入力の選択リストを作成する
             */
            var kaikeiHeaderList = await SetKeyInputList(-5, CurrentDateTime);
            string defaultsetting = kaikeiHeaderList.Count > 0 ? kaikeiHeaderList[0].Value  //新規分データを初期値設定
                : throw new NoDataFoundException("選択リスト0件");

            /*
             * ビューモデルの作成
             */
            this.KaikeiViewModel = new(_context) {
                KaikeiDateAndId = defaultsetting,
                KaikeiHeaderList = kaikeiHeaderList
            };
            //TempDataにビューモデルを保存
            SetViewModelToTempData(this.KaikeiViewModel);

            return this.KaikeiViewModel;
        }

        /// <summary>
        /// 会計セッティング
        /// </summary>
        /// <param name="argKaikeiViewModel">会計ビューモデル</param>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public async Task<KaikeiViewModel> KaikeiSetting(KaikeiViewModel argKaikeiViewModel) {

            _ = argKaikeiViewModel?.KaikeiDateAndId ?? throw new ArgumentException("引数なし");

            /*
             * 引数のデータ取り出し
             */

            //Json化されているキーデータをモデルに変換
            UriageDateTimeAndIdMatching uriageDateTimeAndIdMatching
                = JsonSerializer.Deserialize<UriageDateTimeAndIdMatching>(argKaikeiViewModel.KaikeiDateAndId)
                ?? throw new ArgumentException("Jsonデータなし");

            string uriageDatetimeId = uriageDateTimeAndIdMatching.UriageDatetimeId ?? string.Empty; //売上日時コード
            DateTime uriageDatetime = uriageDateTimeAndIdMatching.UriageDatetime;   //売上日時

            /*
             * 会計ヘッダ＋実績を初期設定
             */
            KaikeiHeader kaikeiHeader = (uriageDatetimeId == string.Empty
                ? Kaikei.KaikeiSakusei(uriageDatetime)       //新規
                : await Kaikei.KaikeiToiawase(uriageDatetimeId!, x => x.KaikeiSeq))!;   //すでにある場合

            /*
             * 選択されたキーデータで選択リストを作成する（一件だけ）
             */
            IList<SelectListItem> kaikeiHeaderList = SetKeyInputList(uriageDateTimeAndIdMatching);

            /*
             * 会計を一つづつ入れる時に商品コードをいれる用に、商品リストを作成する
             * 以下のリストは、実際の商品にバーコードがついていればいらない
             */
            IList<SelectListItem> shohinList = new List<SelectListItem>();
            shohinList = _context.ShohinMaster
                .OrderBy(x => x.ShohinId)
                .Select(x => new SelectListItem { Value = x.ShohinId, Text = $"{x.ShohinId}:{x.ShohinName}" })
                .ToList();

            /*
             * ビューモデルの作成
             */
            this.KaikeiViewModel = new KaikeiViewModel(_context) {
                KaikeiDateAndId = argKaikeiViewModel.KaikeiDateAndId,
                KaikeiHeaderList = kaikeiHeaderList,
                KaikeiHeader = kaikeiHeader,
                ShohinList = shohinList,
            };

            //TempDataにビューモデルを保存
            SetViewModelToTempData(this.KaikeiViewModel);

            return this.KaikeiViewModel;

        }

        /// <summary>
        /// 会計Add
        /// </summary>
        /// <param name="argKaikeiViewModel">会計ビューモデル</param>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public async Task<KaikeiViewModel> KaikeiAdd(KaikeiViewModel argKaikeiViewModel) {

            /*
             * TempDataからのデータ復帰
             */
            this.KaikeiViewModel = GetViewModelToTempData();
            Kaikei.KaikeiHeader = this.KaikeiViewModel.KaikeiHeader;

            //会計実績がnullなら、０件リスト化
            if (this.KaikeiViewModel.KaikeiHeader.KaikeiJissekis is null) {
                this.KaikeiViewModel.KaikeiHeader.KaikeiJissekis = new List<KaikeiJisseki>();
            }

            IKaikeiJissekiForAdd kaikeiJissekiforAdd = argKaikeiViewModel.KaikeiJissekiforAdd;

            /*
             * 画面入力された会計実績を追加（画面上のみＤＢにはまだ反映させない
             */
            this.KaikeiViewModel.KaikeiHeader.KaikeiJissekis
                = await Kaikei.KaikeiAddcommodity(kaikeiJissekiforAdd);

            //post前に入力されていた商品コードを表示用にセットしておく
            this.KaikeiViewModel.KaikeiJissekiforAdd.ShohinId = kaikeiJissekiforAdd.ShohinId;

            //TempDataにビューモデルを保存
            SetViewModelToTempData(this.KaikeiViewModel);

            return this.KaikeiViewModel;
        }

        /// <summary>
        /// 会計Commit
        /// </summary>
        /// <param name="argKaikeiViewModel">会計ビューモデル</param>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public async Task<KaikeiViewModel> KaikeiCommit(KaikeiViewModel argKaikeiViewModel) {

            /*
             * TempDataからのデータ復帰し、プロパティへセット
             */
            this.KaikeiViewModel = GetViewModelToTempData();

            /*
             * Postされたデータから、会計ヘッダー＋実績を取り出す
             */
            KaikeiHeader postedKaikeiHeader = argKaikeiViewModel.KaikeiHeader;

            /*
             * postされたデータを、プロパティに置いたデータに上書き
             */
            KaikeiHeader updatedKaikeiHeader = await Kaikei.KaikeiUpdate(postedKaikeiHeader);

            //売上日時コード抽出（上記のメソッドで発番されるので、このタイミング
            string postedUriageDatetimeId = updatedKaikeiHeader.UriageDatetimeId;

            var entities = _context.ChangeTracker.Entries();

            /*
             * DB更新
             */
            _context.SaveChanges();

            /*
             * ビューデータの作成
             */
            KaikeiViewModel reQueryKaikeiViewModel = new(_context) {
                //ＤＢ更新後の再問合せ
                KaikeiHeader = await Kaikei.KaikeiToiawase(postedUriageDatetimeId, x => x.KaikeiSeq)
                ?? throw new NoDataFoundException("更新したデータがＤＢ上で見つかりません"),

                //処理結果（とりあえずＯＫ）
                IsNormal = true,
                Remark = new Message().SetMessage(ErrDef.NormalUpdate)?.MessageText,

                //選択されたキーデータで選択リストを作成する（一件だけ）
                KaikeiHeaderList
                = SetKeyInputList(new UriageDateTimeAndIdMatching(postedKaikeiHeader.UriageDatetime, postedKaikeiHeader.UriageDatetimeId))

            };

            //再描画後、前画面でデータ入力した商品コードをセットしておく
            //一行しかないから、かならず[0]はある

            if (reQueryKaikeiViewModel.KaikeiHeaderList.Count > 0) {
                reQueryKaikeiViewModel.KaikeiDateAndId = reQueryKaikeiViewModel.KaikeiHeaderList[0].Value;
            } else {
                throw new Exception("商品リストエラー");
            }
            //
            //reQueryKaikeiViewModel.KaikeiJissekiforAdd = new KaikeiJissekiForAdd(_context);

            this.KaikeiViewModel = reQueryKaikeiViewModel;
            //TempDataにビューモデルを保存
            SetViewModelToTempData(this.KaikeiViewModel);

            return this.KaikeiViewModel;
        }

        /// <summary>
        /// 商品名称を返す
        /// </summary>
        /// <param name="argShohinId">商品コード</param>
        /// <returns>string 商品名称</returns>
        /// <remarks>
        /// <para>画面からajaxで、商品コードを投げてくるから、商品名称を返す</para>
        /// </remarks>
        public async Task<string> GetShohinName(string argShohinId) {
            var shohinName = await _context.ShohinMaster.Where(x => x.ShohinId.Equals(argShohinId)).Select(x => x.ShohinName).FirstOrDefaultAsync();

            shohinName ??= string.Empty;
            return shohinName;
        }
        /// <summary>
        /// ビューモデルをtempデータに退避
        /// </summary>
        /// <param name="argKaikeiViewModel">ビューモデル</param>
        private void SetViewModelToTempData(KaikeiViewModel argKaikeiViewModel) {
            string serializedString = JsonSerializer.Serialize(argKaikeiViewModel, new JsonSerializerOptions() {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true,
            });
            _tempData[tempDataIndex] = serializedString;
        }
        /// <summary>
        /// tempデータからビューモデルへ復帰
        /// </summary>
        /// <returns></returns>
        private KaikeiViewModel GetViewModelToTempData() {
            string tempData = _tempData[tempDataIndex]?.ToString() ?? throw new ArgumentException("TempDataなし");
            KaikeiViewModel kaikeiViewModel = JsonSerializer.Deserialize<KaikeiViewModel>(tempData) ?? throw new Exception("Jsonデータ不正");
            return kaikeiViewModel;
        }
        /// <summary>
        /// キー入力用リスト作成（１画面目用）
        /// </summary>
        /// <param name="argDaysAgo"></param>
        /// <param name="argCurrentDateTime"></param>
        /// <returns>IList<SelectListItem>キー入力用リスト（新規分＋過去分）</returns>
        private async Task<IList<SelectListItem>> SetKeyInputList(int argDaysAgo, DateTime argCurrentDateTime) {

            /*
             * ５日前を対象に過去の会計データを抽出し、キー入力の選択リストを作成する
             */
            IList<UriageDateTimeAndIdMatching> uriageDateTimeAndIdMatchings = await _context.KaikeiHeader.Where(x => x.UriageDatetime >= DateTime.Now.AddDays(argDaysAgo).Date.ToUniversalTime())
                    .OrderByDescending(x => x.UriageDatetime)
                    .Select(x => new UriageDateTimeAndIdMatching {
                        UriageDatetime = x.UriageDatetime,
                        UriageDatetimeId = x.UriageDatetimeId
                    })
                    .ToListAsync();
            /*
             * 上記キー入力の選択リストに、今から入力する会計を新規で一覧のトップにいれる
             */
            uriageDateTimeAndIdMatchings.Insert(0, new UriageDateTimeAndIdMatching() {
                UriageDatetime = argCurrentDateTime,
                UriageDatetimeId = string.Empty
            });

            return SetSelectListItem(uriageDateTimeAndIdMatchings);//リストアイテム化
        }

        /// <summary>
        /// キー入力用リスト作成（２画面目以降用）
        /// </summary>
        /// <param name="argUriageDateTimeAndIdMatching">セットするリスト内容</param>
        /// <returns>IList<SelectListItem>キー入力用リスト（一行だけ） </returns>
        /// <remarks>
        /// <para>リストの中で選択されたものだけに絞る</para>
        /// </remarks>
        private static IList<SelectListItem> SetKeyInputList(UriageDateTimeAndIdMatching argUriageDateTimeAndIdMatching) {
            IList<UriageDateTimeAndIdMatching> uriageDateTimeAndIdMatchings
                = new List<UriageDateTimeAndIdMatching> {
                    argUriageDateTimeAndIdMatching
                };
            return SetSelectListItem(uriageDateTimeAndIdMatchings);//リストアイテム化
        }

        /// <summary>
        /// リストアイテム化
        /// </summary>
        /// <param name="argUriageDateTimeAndIdMatchings">セットするリスト内容</param>
        /// <returns>IList<SelectListItem> 表示用リスト化されたもの</returns>
        private static IList<SelectListItem> SetSelectListItem(IList<UriageDateTimeAndIdMatching> argUriageDateTimeAndIdMatchings) {
            /*
             * 表示用リストアイテムを設定する
             */
            //string? defaultsetting;
            IList<SelectListItem> kaikeiHeaderList = new List<SelectListItem>();
            for (int i = 0; i < argUriageDateTimeAndIdMatchings.Count; i++) {
                var item = argUriageDateTimeAndIdMatchings[i];

                string serializedString = JsonSerializer.Serialize(item);
                kaikeiHeaderList.Add(new SelectListItem($"{(item.UriageDatetimeId == string.Empty ? "新規" : item.UriageDatetimeId)}:{item.UriageDatetime}", serializedString));

                //if (i == 0) defaultsetting = serializedString;  
            }
            return kaikeiHeaderList;
        }
    }
}
