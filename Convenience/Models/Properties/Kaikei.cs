using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;
using AutoMapper;
using AutoMapper.Collection;
using Convenience.Migrations;
using AutoMapper.EquivalencyExpression;
using Microsoft.Build.Framework;

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
        /// AutoMapper
        /// </summary>
        private readonly IMapper _mapper;
        private readonly IMapper _mappershared;
        /// <summary>
        /// コンストラクタ（ASP用）
        /// </summary>
        /// <param name="context"></param>
        public Kaikei(ConvenienceContext context) {
            this._context = context;

            var config1 = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers(); // コレクションマッパーを追加
                cfg.AddProfile(new AutoMapperProfile(this));
            });

            var config2 = new MapperConfiguration(cfg => {
                cfg.AddProfile(new AutoMapperProfile(this));
            });

            this._mapper = config1.CreateMapper();
            this._mappershared = config2.CreateMapper();
        }

        /// <summary>
        /// 会計コード発番
        /// </summary>
        /// <param name="argCurrentDateTime">会計日時</param>
        /// <returns>会計コード(yyyyMMdd-HH-001～999、HHは店頭払出日時の時間部分</returns>
        private async Task<string> UriageDatetimeIdHatsuban(DateTime argCurrentDateTime) {
            string dateArea = argCurrentDateTime.ToString("yyyyMMdd-HH");

            string? maxUriageDatetimeId = await _context.KaikeiHeader
                .Where(x => x.UriageDatetimeId.StartsWith(dateArea)).MaxAsync(s => s.UriageDatetimeId);

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
        public KaikeiHeader KaikeiSakusei(DateTime argCurrentDateTime) {
            this.KaikeiHeader = new() {
                UriageDatetimeId = string.Empty,
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
            string shohinId = argKaikeiJisseki.ShohinId ?? throw new ArgumentException("商品マスタがセットされていません");

            //売上日時
            argKaikeiJisseki.UriageDatetime = this.KaikeiHeader?.UriageDatetime
                ?? throw new NoDataFoundException("プロパティに会計ヘッダーがセットされていません");

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
                .FirstOrDefault(x => x.ShohinId!.Equals(shohinId));


            /*
             * 関係する商品マスタを問い合わせる
             */
            argKaikeiJisseki.ShohinMaster = await _context.ShohinMaster.AsNoTracking().FirstOrDefaultAsync(x => x.ShohinId == shohinId)
                    ?? throw new ArgumentException("引数で渡された商品コードがＤＢに見つかりません");

            /*
             * 画面上で追加された項目を追加する
             */

            /*
             * 会計実績の各項目セット
             */

            var tmpSaveDatas = _mappershared.Map<List<KaikeiJisseki>>(this.KaikeiHeader.KaikeiJissekis);

            /*
             * 会計済みリスト作成
             */
            _mapper.Map(new List<IKaikeiJissekiForAdd>() { argKaikeiJisseki }, tmpSaveDatas);

            var tmpSaveData = tmpSaveDatas.FirstOrDefault() ?? throw new Exception("データエラー");

            var existingItem = this.KaikeiHeader.KaikeiJissekis.FirstOrDefault(x => x.ShohinId == shohinId);
            if (existingItem == null) this.KaikeiHeader.KaikeiJissekis.Insert(0, tmpSaveData);
            else this.KaikeiHeader.KaikeiJissekis[this.KaikeiHeader.KaikeiJissekis.IndexOf(existingItem)] = tmpSaveData;

            return (KaikeiHeader.KaikeiJissekis);
        }

        /// <summary>
        /// 会計問い合わせ(ソート指示なし）
        /// </summary>
        /// <param name="argTentoHaraidashiId">店頭払出コード</param>
        /// <returns>会計実績ヘッダー＋実績</returns>
        public async Task<KaikeiHeader?> KaikeiToiawase(string argTentoHaraidashiId) {
            return (await KaikeiToiawase<object>(argTentoHaraidashiId, null));
        }

        /// <summary>
        /// 会計問い合わせ
        /// </summary>
        /// <typeparam name="T">ソート指示された項目のタイプ</typeparam>
        /// <param name="argTentoHaraidashiId">店頭払出コード</param>
        /// <param name="orderexpression">ソート指示用ラムダ式</param>
        /// <returns>会計実績ヘッダー＋実績</returns>
        public async Task<KaikeiHeader?> KaikeiToiawase<T>(string argTentoHaraidashiId, Expression<Func<KaikeiJisseki, T>>? orderexpression) {

            /*
             * Linq組み立て
             */
            //Where部分
            IQueryable<KaikeiHeader> qty1 = _context.KaikeiHeader
                .Where(x => x.UriageDatetimeId.Equals(argTentoHaraidashiId))
                .Include(x => x.KaikeiJissekis)
                .ThenInclude(x => x.ShohinMaster)
                .Include(x => x.KaikeiJissekis)
                .ThenInclude(x => x.TentoZaiko)
                .Include(x => x.KaikeiJissekis)
                .ThenInclude(x => x.NaigaiClassMaster);

            //OrderBy指示するか、そのままか

            IQueryable<KaikeiHeader> qty2 = orderexpression is not null ?
                qty1.Include(x => x.KaikeiJissekis.AsQueryable().OrderBy(orderexpression)) : qty1;
            /*
             * Linq実行
             */
            return this.KaikeiHeader = await qty2.FirstOrDefaultAsync();
        }

        /// <summary>
        /// 会計ヘッダー＋実績のＤＢ更新
        /// </summary>
        /// <param name="argpostedKaikeiHeader">反映する会計ヘッダ＋実績</param>
        /// <returns>KaikeiHeader DB更新された会計ヘッダ＋実績</returns>
        public async Task<KaikeiHeader> KaikeiUpdate(KaikeiHeader argpostedKaikeiHeader) {

            _ = argpostedKaikeiHeader.KaikeiJissekis ?? throw new Exception("Postデータがありません");
            /*
             * 前準備　
             */
            KaikeiHeader postedKaikeiHeader = argpostedKaikeiHeader;                     //Postされた会計実績ヘッダー
            string postedUriageDatetimeId = postedKaikeiHeader.UriageDatetimeId;   //売上日時コード
            DateTime postedUriageDatetime = postedKaikeiHeader.UriageDatetime;       //売上日時

            /*
             * 会計ヘッダー＋実績問い合わせ
             */

            KaikeiHeader queriedkaikeiHeader;
            if (string.IsNullOrEmpty(postedUriageDatetimeId)) {
                // 新規の場合は 初期化
                queriedkaikeiHeader = new KaikeiHeader();
                string uriageDatetimeId = await UriageDatetimeIdHatsuban(postedUriageDatetime) ?? throw new Exception("");
                postedKaikeiHeader.UriageDatetimeId = uriageDatetimeId;
                postedKaikeiHeader.KaikeiJissekis.ToList().ForEach(x => {
                    x.UriageDatetimeId = uriageDatetimeId;
                    x.UriageDatetime = postedKaikeiHeader.UriageDatetime;
                    x.ShohinMaster = _context.ShohinMaster.AsNoTracking().FirstOrDefault(y => y.ShohinId == x.ShohinId);
                    x.TentoZaiko = _context.TentoZaiko.FirstOrDefault(y => y.ShohinId == x.ShohinId);
                });
            }
            else {
                //新規以外は問い合わせて、変更の準備
                queriedkaikeiHeader = await KaikeiToiawase(postedUriageDatetimeId, x => x.KaikeiSeq)??throw new Exception("会計問い合わせエラー");
                postedKaikeiHeader.KaikeiJissekis.ToList().ForEach(x =>
                    x.ShohinMaster = queriedkaikeiHeader.KaikeiJissekis.FirstOrDefault(y => x.UriageDatetimeId == y.UriageDatetimeId && x.ShohinId == y.ShohinId)?.ShohinMaster ?? throw new Exception("")
                );
            }
            /*
             * post側で金額を設定
             */
            postedKaikeiHeader.KaikeiJissekis.ToList().ForEach(x => {
                x.ShohinTanka = x.ShohinMaster!.ShohinTanka;
                x.UriageKingaku = x.UriageSu * x.ShohinTanka;
            });

            /*
             * 会計ヘッダー＋実績登録
             */
            _mapper.Map<KaikeiHeader,KaikeiHeader>(postedKaikeiHeader, queriedkaikeiHeader);

            if (_context.Entry(queriedkaikeiHeader).State == EntityState.Detached) _context.Add(queriedkaikeiHeader);

            return this.KaikeiHeader = queriedkaikeiHeader;
        }
        /// <summary>
        /// 店頭在庫から売上分を差し引く
        /// </summary>
        /// <param name="argShohinId">商品コード</param>
        /// <param name="argUriageDateTime">売上日時</param>
        /// <param name="argDiffUriageSu">差し引く個数</param>
        /// <param name="argTentoZaiko">店頭在庫</param>
        /// <returns>店頭在庫</returns>
        public TentoZaiko? ZaikoConnection(string argShohinId, DateTime argUriageDateTime, decimal argDiffUriageSu, TentoZaiko? argTentoZaiko) {

            /*
             * 店頭在庫の抽出
             * （引数）店頭在庫がセットされていれば、それを適用
             */
            TentoZaiko? tentoZaiko = argTentoZaiko ?? _context.TentoZaiko.FirstOrDefault(x => x.ShohinId.Equals(argShohinId));

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
        public KaikeiJisseki ShohizeiKeisan(IKaikeiJissekiForAdd inKaikeiJisseki, KaikeiJisseki outKaikeiJisseki) {
            /*
             * 商品マスタから消費税率を求める
             */
            ShohinMaster? shohinmaster = inKaikeiJisseki.ShohinMaster?.ShohinId is null
                ? _context.ShohinMaster.AsNoTracking()
                    .FirstOrDefault(x => x.ShohinId == inKaikeiJisseki.ShohinId)
                : inKaikeiJisseki.ShohinMaster;

            _ = shohinmaster ?? throw new ArgumentException("商品マスタデータなし");

            /*
             * 内外区分により消費税率を切り分ける
             */
            decimal shohiZei = inKaikeiJisseki.NaigaiClass switch {
                "0" => shohinmaster.ShohiZeiritsu,
                "1" => shohinmaster.ShohiZeiritsuEatIn,
                _ => throw new InvalidDataException("NaigaiClassの区分が不正です")
            };

            /*
             * 消費税込み計算
             */
            outKaikeiJisseki.NaigaiClass = inKaikeiJisseki.NaigaiClass; //内外区分
            outKaikeiJisseki.ShohiZeiritsu = shohiZei;                  //消費税率
            outKaikeiJisseki.ZeikomiKingaku = outKaikeiJisseki.UriageKingaku * (1.0m + outKaikeiJisseki.ShohiZeiritsu / 100.0m);
            //税込金額
            return outKaikeiJisseki;
        }
    }
}
