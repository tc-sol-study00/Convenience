using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Convenience.Models.Properties {
    /// <summary>
    /// 店頭払出クラス
    /// </summary>
    public class Kaikei : IKaikei {

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// <para>プロパティ</para>
        /// <para>店頭払出ヘッダー</para>
        /// </summary>
        public KaikeiHeader? KaikeiHeader { get; set; }

        /// <summary>
        /// コンストラクタ（ASP用）
        /// </summary>
        /// <param name="context"></param>
        public Kaikei(ConvenienceContext context) {
            this._context = context;
        }

        /// <summary>
        /// 会計コード発番
        /// </summary>
        /// <param name="argCurrentDateTime">会計日時</param>
        /// <returns>会計コード(yyyyMMdd-HH-001～999、HHは店頭払出日時の時間部分</returns>
        private string UriageDatetimeIdHatsuban(DateTime argCurrentDateTime) {
            string dateArea = argCurrentDateTime.ToString("yyyyMMdd-HH");

            string? maxUriageDatetimeId = _context.KaikeiHeader
                .Where(x => x.UriageDatetimeId.StartsWith(dateArea)).Max(s => s.UriageDatetimeId);

            uint seq = 0;
            if (maxUriageDatetimeId is null) {
                seq = 1;
            }
            else {
                seq = uint.Parse(maxUriageDatetimeId.Substring(12, 3)) + 1;
            }
            return $"{dateArea}-{seq:000}";
        }

        /// <summary>
        /// <para>会計作成（新規会計用）</para>
        /// <para>新規会計ヘッダーと実績を作成する</para>
        /// </summary>
        /// <param name="argCurrentDateTime">適用日時</param>
        /// <returns>KaikeiHeader 会計ヘッダー＋実績</returns>
        /// <remarks>
        /// <para>①会計ヘッダーの作成</para>
        /// <para>②会計実績の作成</para>
        /// </remarks>
        public async Task<KaikeiHeader> KaikeiSakusei(DateTime argCurrentDateTime) {
            this.KaikeiHeader = new KaikeiHeader() {
                UriageDatetimeId = null,
                UriageDatetime = argCurrentDateTime,
                KaikeiJissekis = new List<KaikeiJisseki>()
            };
            return this.KaikeiHeader;
        }

        /// <summary>
        /// 会計実績の品目追加
        /// </summary>
        /// <param name="argKaikeiJisseki"></param>
        /// <returns>KaikeiJisseki 品目を追加された会計実績</returns>
        public async Task<IList<KaikeiJisseki>> KaikeiAddcommodity(IKaikeiJissekiForAdd argKaikeiJisseki) {

            /*
             * 初期化
             */

            //商品コード
            string shohinId = argKaikeiJisseki.ShohinId;

            //売上日時
            var uriageDatetime = this.KaikeiHeader.UriageDatetime;

            //会計実績格納用変数
            KaikeiJisseki? kaikeiJisseki = default;

            //プロパティ内会計実績がnullの場合は、ゼロリスト化して、add準備する
            if (this.KaikeiHeader.KaikeiJissekis is null) {
                this.KaikeiHeader.KaikeiJissekis = new List<KaikeiJisseki>();
            }

            /*
             * Postされた商品コードより、プロパティ内会計実績を検索し関連する会計実績を抽出する
             */
            kaikeiJisseki = this.KaikeiHeader.KaikeiJissekis
                .Where(x => x.ShohinId.Equals(shohinId))
                .FirstOrDefault();

            /*
             * 関係する商品マスタを問い合わせる
             */
            var shohinMaster = _context.ShohinMaster.Where(x => x.ShohinId == shohinId).FirstOrDefault();
            if (shohinMaster is null) {
                throw new ArgumentException("引数で渡された商品コードがＤＢに見つかりません");
            }

            

            /*
             * 画面上で追加された項目を追加する
             */

            if (kaikeiJisseki is null) { //Postされた商品コードが、プロパティ内会計実績にあるか
                /*
                 * プロパティ内会計実績に、Postされた商品コードがない場合
                 */

                /*
                 * 会計実績の各項目セット
                 */
                kaikeiJisseki = new KaikeiJisseki();
                kaikeiJisseki.UriageSu = argKaikeiJisseki.UriageSu;     //売上数
                kaikeiJisseki.ShohinId = shohinId;                      //商品コード
                argKaikeiJisseki.UriageKingaku = argKaikeiJisseki.UriageSu * shohinMaster.ShohinTanka;
                kaikeiJisseki.UriageKingaku = argKaikeiJisseki.UriageKingaku;
                                                                        //売上金額                
                /* 消費税関連処理 */
                ShohizeiKeisan(argKaikeiJisseki, kaikeiJisseki);        //内外区分
                                                                        //消費税率
                                                                        //税込み金額
                kaikeiJisseki.UriageDatetimeId = this.KaikeiHeader.UriageDatetimeId;
                                                                        //売上日時コード
                kaikeiJisseki.ShohinTanka = shohinMaster.ShohinTanka;   //商品単価
                kaikeiJisseki.UriageDatetime = uriageDatetime;          //売上日時
                kaikeiJisseki.ShohinMaster = shohinMaster;              //商品マスタ
                kaikeiJisseki.TentoZaiko =
                    await ZaikoConnection(kaikeiJisseki.ShohinId, kaikeiJisseki.UriageDatetime, kaikeiJisseki.UriageSu, null);
                                                                        //店頭在庫
                kaikeiJisseki.KaikeiHeader = this.KaikeiHeader;         //会計ヘッダーを親に差す

                /*
                 * 会計実績をプロパティにセット
                 */
                this.KaikeiHeader.KaikeiJissekis.Insert(0, kaikeiJisseki);
            }
            else {
                /*
                 * プロパティ内会計実績に、Postされた商品コードがある場合
                 */

                /*
                 * 会計実績の各項目セット
                 */
                var tempUriageSu = kaikeiJisseki.UriageSu;
                kaikeiJisseki.UriageSu += argKaikeiJisseki.UriageSu;    //売上数
                                                                        //商品コードは既にセットされている
                argKaikeiJisseki.UriageKingaku = kaikeiJisseki.UriageSu * shohinMaster.ShohinTanka;
                kaikeiJisseki.UriageKingaku = argKaikeiJisseki.UriageKingaku;
                                                                        //売上金額
                ShohizeiKeisan(argKaikeiJisseki, kaikeiJisseki);        //内外区分
                                                                        //消費税率
                                                                        //税込み金額
                                                                        //売上日時コードは既にセットされている
                //kaikeiJisseki.UriageDatetimeId = this.KaikeiHeader.UriageDatetimeId;
                                                                        //商品単価は既にセットされている
                                                                        //売上日時は既にセットされている
                                                                        //商品マスタはすでにセットされている
                kaikeiJisseki.TentoZaiko =
                    await ZaikoConnection(kaikeiJisseki.ShohinId, kaikeiJisseki.UriageDatetime, kaikeiJisseki.UriageSu - tempUriageSu, kaikeiJisseki.TentoZaiko);
                                                                        //店頭在庫
                                                                        //会計ヘッダーはすでに親をさしている
            }
            return (KaikeiHeader.KaikeiJissekis);
        }

        /// <summary>
        /// 会計問い合わせ(ソート指示なし）
        /// </summary>
        /// <param name="argTentoHaraidashiId">店頭払出コード</param>
        /// <returns>会計実績ヘッダー＋実績</returns>
        public async Task<KaikeiHeader?> KaikeiToiawase(string argTentoHaraidashiId) {
            return(await KaikeiToiawase<object>(argTentoHaraidashiId, null));
        }

        /// <summary>
        /// 会計問い合わせ
        /// </summary>
        /// <typeparam name="T">ソート指示された項目のタイプ</typeparam>
        /// <param name="argTentoHaraidashiId">店頭払出コード</param>
        /// <param name="orderexpression">ソート指示用ラムダ式</param>
        /// <returns>会計実績ヘッダー＋実績</returns>
        public async Task<KaikeiHeader?> KaikeiToiawase<T>(string argTentoHaraidashiId, Expression<Func<KaikeiJisseki,T>>? orderexpression) {

           /*
            * Linq組み立て
            */
           //Where部分
           var qty1 = _context.KaikeiHeader
                .Where(x => x.UriageDatetimeId.Equals(argTentoHaraidashiId));

            //OrderBy指示するか、そのままか
            IIncludableQueryable<KaikeiHeader, IEnumerable<KaikeiJisseki>> qty2;
            if (orderexpression is null) {
                qty2 = qty1.Include(x => x.KaikeiJissekis);
            }
            else {
                qty2 = qty1.Include(x => x.KaikeiJissekis.AsQueryable().OrderBy(orderexpression));
            }

            //以降の記述（Include)
            var qty3 = qty2
                .ThenInclude(x => x.ShohinMaster)
                .Include(x => x.KaikeiJissekis)
                .ThenInclude(x => x.TentoZaiko)
                .Include(x => x.KaikeiJissekis)
                .ThenInclude(x => x.NaigaiClassMaster);

            /*
             * Linq実行
             */
            this.KaikeiHeader=qty3.FirstOrDefault();

            return this.KaikeiHeader;
        }

        /// <summary>
        /// 会計ヘッダー＋実績のＤＢ更新
        /// </summary>
        /// <param name="argpostedKaikeiHeader">反映する会計ヘッダ＋実績</param>
        /// <returns>KaikeiHeader DB更新された会計ヘッダ＋実績</returns>
        public async Task<KaikeiHeader?> KaikeiUpdate(KaikeiHeader argpostedKaikeiHeader) {

            /*
             * 前準備　
             */
            var postedKaikeiHeader = argpostedKaikeiHeader;                     //Postされた会計実績ヘッダー
            var postedUriageDatetimeId = postedKaikeiHeader.UriageDatetimeId;   //売上日時コード
            var postedUriageDatetime = postedKaikeiHeader.UriageDatetime;       //売上日時

            /*
             * 会計ヘッダー＋実績問い合わせ
             */
            KaikeiHeader? queriedkaikeiHeader = default;
            if (postedUriageDatetimeId is null) {
                queriedkaikeiHeader = null; //新規の場合はnull
            }
            else {
                                            //問い合わせ
                queriedkaikeiHeader = await KaikeiToiawase(postedUriageDatetimeId,x => x.KaikeiSeq);
            }

            /*
             * 会計ヘッダー＋実績登録
             */
            if (queriedkaikeiHeader is null) {
                /*
                 * 新規の場合
                 * 売上日時コード発番
                 */
                postedKaikeiHeader.UriageDatetimeId = UriageDatetimeIdHatsuban(postedUriageDatetime);

                //会計実績に、売上日時ＩＤと反映＋ShohinMasterをNULL(しておかないと、add時、更新エラーになる）
                postedKaikeiHeader.KaikeiJissekis.ToList().ForEach(x => { x.ShohinMaster = null; x.UriageDatetimeId = postedKaikeiHeader.UriageDatetimeId; });


                /*
                 * 会計実績に対する処理
                 * 店頭在庫の登録
                 * 会計Seqの反映
                 */
                for (int index=0; index < postedKaikeiHeader.KaikeiJissekis.Count;index++) {

                    KaikeiJisseki postedItem = postedKaikeiHeader.KaikeiJissekis[index];
                    ShohinMaster ? shohinMaster=_context.ShohinMaster.AsNoTracking().Where(x => x.ShohinId == postedItem.ShohinId).FirstOrDefault();

                    postedItem.UriageKingaku = postedItem.UriageSu * shohinMaster?.ShohinTanka??0;
                    ShohizeiKeisan(postedItem, postedItem);   //消費税関連処理
                    postedItem.TentoZaiko = await ZaikoConnection(postedItem.ShohinId, postedItem.UriageDatetime, postedItem.UriageSu, null);
                    postedItem.KaikeiSeq = index + 1;
                }

                /*
                 * PostデータのDBへAdd準備（会計ヘッダー＋実績）
                 */
                _context.Add(postedKaikeiHeader);

                //プロパティに反映
                this.KaikeiHeader = postedKaikeiHeader; 
            }
            else {
                /*
                 * 既存データの場合
                 */
                for (int index = 0; index < postedKaikeiHeader.KaikeiJissekis.Count; index++) {
                    KaikeiJisseki postedItem = postedKaikeiHeader.KaikeiJissekis[index];
                    /*
                     * 売上金額を求めておく
                     */
                    postedItem.UriageKingaku=postedItem.UriageSu*postedItem.ShohinTanka;
                    /*
                     * 更新対象を抽出
                     */
                    var queriedKaikeiJisseki = queriedkaikeiHeader.KaikeiJissekis
                        .Where(x => x.UriageDatetimeId.Equals(postedItem.UriageDatetimeId) &&
                        x.ShohinId.Equals(postedItem.ShohinId) &&
                        x.UriageDatetime == postedItem.UriageDatetime
                        ).Single();
                    /*
                     * Postデータを問い合わせた結果にオーバーライドさせる
                     */
                    var tempUriageSu = queriedKaikeiJisseki.UriageSu;                       
                    queriedKaikeiJisseki.UriageSu += postedItem.UriageSu - tempUriageSu;                //売上数
                    queriedKaikeiJisseki.UriageKingaku = postedItem.UriageKingaku;                      //売上金額
                    //消費税関連処理
                    ShohizeiKeisan(postedItem, queriedKaikeiJisseki);                                   //内外区分
                                                                                                        //消費税率
                                                                                                        //税込み金額
                    queriedKaikeiJisseki.TentoZaiko =
                        await ZaikoConnection(postedItem.ShohinId, postedItem.UriageDatetime, postedItem.UriageSu - tempUriageSu, null);
                                                                                                        //店頭在庫への反映
                    postedItem.KaikeiSeq = index + 1;                                                   //会計Seq
                }
                this.KaikeiHeader = queriedkaikeiHeader;                                                //プロパティにセット
            }
            return this.KaikeiHeader;
        }
        /// <summary>
        /// 店頭在庫から売上分を差し引く
        /// </summary>
        /// <param name="argShohinId">商品コード</param>
        /// <param name="argUriageDateTime">売上日時</param>
        /// <param name="argDiffUriageSu">差し引く個数</param>
        /// <param name="argTentoZaiko">店頭在庫</param>
        /// <returns>店頭在庫</returns>
        private async Task<TentoZaiko> ZaikoConnection(string argShohinId, DateTime argUriageDateTime, decimal argDiffUriageSu, TentoZaiko? argTentoZaiko) {

            /*
             * 店頭在庫の抽出
             * （引数）店頭在庫がセットされていれば、それを適用
             */
            TentoZaiko? tentoZaiko = default;
            if (argTentoZaiko is null) {
                tentoZaiko = await _context.TentoZaiko.Where(x => x.ShohinId.Equals(argShohinId)).FirstOrDefaultAsync();
            }
            else {
                tentoZaiko = argTentoZaiko;
            }

            /*
             * 店頭在庫計算
             */
            if (tentoZaiko is not null) {
                tentoZaiko.ZaikoSu -= argDiffUriageSu;
                tentoZaiko.ZaikoSu = tentoZaiko.ZaikoSu > 0 ? tentoZaiko.ZaikoSu : 0;
                //直近売上日のセット
                if (argDiffUriageSu > 0) tentoZaiko.LastUriageDatetime = argUriageDateTime;
            }

            return tentoZaiko;
        }

        /// <summary>
        /// 消費税計算
        /// </summary>
        /// <param name="inKaikeiJisseki">会計実績</param>
        /// <param name="outKaikeiJisseki">消費税関連で計算されたものの反映先（会計実績）</param>
        /// <returns>消費税関連で計算されたものの反映先（会計実績）</returns>
        /// <exception cref="InvalidDataException"></exception>
        /// <remarks>（条件）売上金額が第一引数側にセットされていること</remarks>
        private KaikeiJisseki ShohizeiKeisan(IKaikeiJissekiForAdd inKaikeiJisseki, KaikeiJisseki outKaikeiJisseki) {
            decimal shohiZei = default;

            /*
             * 商品マスタから消費税率を求める
             */
            ShohinMaster? shohinmaster = default;
            if (inKaikeiJisseki.ShohinMaster?.ShohinId is null) {
                shohinmaster = _context.ShohinMaster.AsNoTracking()
                    .Where(x => x.ShohinId == inKaikeiJisseki.ShohinId)
                    .FirstOrDefault();
            }
            else {
                shohinmaster = inKaikeiJisseki.ShohinMaster;
            }
            /*
             * 内外区分により消費税率を切り分ける
             */
            switch (inKaikeiJisseki.NaigaiClass) {
                case "0":
                    shohiZei = shohinmaster.ShohiZeiritsu;
                    break;
                case "1":
                    shohiZei = shohinmaster.ShohiZeiritsuEatIn;
                    break;
                default:
                    throw new InvalidDataException("NaigaiClassの区分が不正です");
            }
            /*
             * 消費税込み計算
             */
            outKaikeiJisseki.NaigaiClass = inKaikeiJisseki.NaigaiClass; //内外区分
            outKaikeiJisseki.ShohiZeiritsu = shohiZei;                  //消費税率
            outKaikeiJisseki.ZeikomiKingaku = inKaikeiJisseki.UriageKingaku * (1.0m + outKaikeiJisseki.ShohiZeiritsu / 100.0m);
                                                                        //税込金額
            return outKaikeiJisseki;
        }
    }
}
