using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Convenience.Migrations;
using Convenience.Models.ViewModels.Kaikei;
using System.Net.Http.Headers;

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

            string shohinId = argKaikeiJisseki.ShohinId;

            var uriageDatetime = this.KaikeiHeader.UriageDatetime;

            KaikeiJisseki? kaikeiJisseki = default;

            if (this.KaikeiHeader.KaikeiJissekis is null) {
                this.KaikeiHeader.KaikeiJissekis = new List<KaikeiJisseki>();
            }

            kaikeiJisseki = this.KaikeiHeader.KaikeiJissekis
                .Where(x => x.ShohinId.Equals(shohinId))
                .FirstOrDefault();

            var shohinMaster = _context.ShohinMaster.Where(x => x.ShohinId == shohinId).FirstOrDefault();
            if (shohinMaster is null) {
                throw new ArgumentException("引数で渡された商品コードがＤＢに見つかりません");
            }

            if (kaikeiJisseki is null) {
                kaikeiJisseki = new KaikeiJisseki();
                kaikeiJisseki.UriageSu = argKaikeiJisseki.UriageSu;
                kaikeiJisseki.ShohinId = shohinId;
                kaikeiJisseki.NaigaiClass = argKaikeiJisseki.NaigaiClass;

                ShohizeiKeisan(argKaikeiJisseki, kaikeiJisseki);

                kaikeiJisseki.UriageKingaku = argKaikeiJisseki.UriageSu * shohinMaster.ShohinTanka;
                kaikeiJisseki.ZeikomiKingaku = kaikeiJisseki.UriageKingaku * (1.0m + shohinMaster.ShohiZeiritsu / 100.0m);
                kaikeiJisseki.UriageDatetimeId = this.KaikeiHeader.UriageDatetimeId;
                kaikeiJisseki.ShohinTanka = shohinMaster.ShohinTanka;
                kaikeiJisseki.UriageDatetime = uriageDatetime;
                kaikeiJisseki.ShohinMaster = shohinMaster;
                kaikeiJisseki.TentoZaiko =
                    await ZaikoConnection(kaikeiJisseki.ShohinId, kaikeiJisseki.UriageDatetime, kaikeiJisseki.UriageSu, null);
                kaikeiJisseki.KaikeiHeader = this.KaikeiHeader;
                this.KaikeiHeader.KaikeiJissekis.Insert(0, kaikeiJisseki);
            }
            else {
                var tempUriageSu = kaikeiJisseki.UriageSu;
                kaikeiJisseki.UriageSu += argKaikeiJisseki.UriageSu;
                kaikeiJisseki.UriageKingaku = kaikeiJisseki.UriageSu * kaikeiJisseki.ShohinTanka;

                ShohizeiKeisan(argKaikeiJisseki, kaikeiJisseki);

                kaikeiJisseki.ZeikomiKingaku = kaikeiJisseki.UriageKingaku * (1.0m + kaikeiJisseki.ShohiZeiritsu / 100.0m);
                kaikeiJisseki.TentoZaiko =
                    await ZaikoConnection(kaikeiJisseki.ShohinId, kaikeiJisseki.UriageDatetime, kaikeiJisseki.UriageSu - tempUriageSu, kaikeiJisseki.TentoZaiko);
            }
            return (KaikeiHeader.KaikeiJissekis);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argTentoHaraidashiId"></param>
        /// <returns></returns>
        public async Task<KaikeiHeader?> KaikeiToiawase(string argTentoHaraidashiId) {
            this.KaikeiHeader = _context.KaikeiHeader
                .Where(x => x.UriageDatetimeId.Equals(argTentoHaraidashiId))
                .Include(x => x.KaikeiJissekis.OrderBy(y=> y.KaikeiSeq))
                .ThenInclude(x => x.ShohinMaster)
                .Include(x => x.KaikeiJissekis)
                .ThenInclude(x => x.TentoZaiko)
                .Include(x => x.KaikeiJissekis)
                .ThenInclude(x => x.NaigaiClassMaster)
                .FirstOrDefault();

            return this.KaikeiHeader;
        }

        /// <summary>
        /// 会計ヘッダー＋実績のＤＢ更新
        /// </summary>
        /// <param name="argpostedKaikeiHeader">反映する会計ヘッダ＋実績</param>
        /// <returns>KaikeiHeader DB更新された会計ヘッダ＋実績</returns>
        public async Task<KaikeiHeader?> KaikeiUpdate(KaikeiHeader argpostedKaikeiHeader) {

            var postedKaikeiHeader = argpostedKaikeiHeader;
            var postedUriageDatetimeId = postedKaikeiHeader.UriageDatetimeId;
            var postedUriageDatetime = postedKaikeiHeader.UriageDatetime;

            KaikeiHeader queriedkaikeiHeader = default;
            if (postedUriageDatetimeId is null) {
                queriedkaikeiHeader = null;
            }
            else {
                queriedkaikeiHeader = await KaikeiToiawase(postedUriageDatetimeId);
            }

            if (queriedkaikeiHeader is null) {
                postedKaikeiHeader.UriageDatetimeId = UriageDatetimeIdHatsuban(postedUriageDatetime);
                postedKaikeiHeader.KaikeiJissekis.ToList().ForEach(x => { x.ShohinMaster = null; x.UriageDatetimeId = postedKaikeiHeader.UriageDatetimeId; });

                for (int index=0; index < postedKaikeiHeader.KaikeiJissekis.Count;index++) {
                    KaikeiJisseki postedItem = postedKaikeiHeader.KaikeiJissekis[index];
                    postedItem.TentoZaiko = await ZaikoConnection(postedItem.ShohinId, postedItem.UriageDatetime, postedItem.UriageSu, null);
                    postedItem.KaikeiSeq = index + 1;
                }

                _context.Add(postedKaikeiHeader);
                this.KaikeiHeader = postedKaikeiHeader;
            }
            else {
                for (int index = 0; index < postedKaikeiHeader.KaikeiJissekis.Count; index++) {
                    KaikeiJisseki postedItem = postedKaikeiHeader.KaikeiJissekis[index];
                    var queriedKaikeiJisseki = queriedkaikeiHeader.KaikeiJissekis
                        .Where(x => x.UriageDatetimeId.Equals(postedItem.UriageDatetimeId) &&
                        x.ShohinId.Equals(postedItem.ShohinId) &&
                        x.UriageDatetime == postedItem.UriageDatetime
                        ).Single();

                    var tempUriageSu = queriedKaikeiJisseki.UriageSu;
                    queriedKaikeiJisseki.UriageSu += postedItem.UriageSu - tempUriageSu;
                    queriedKaikeiJisseki.UriageKingaku = postedItem.UriageSu * postedItem.ShohinTanka;
                    ShohizeiKeisan(postedItem, queriedKaikeiJisseki);
                    queriedKaikeiJisseki.ZeikomiKingaku = queriedKaikeiJisseki.UriageKingaku * (1.0m + queriedKaikeiJisseki.ShohiZeiritsu / 100.0m);

                    queriedKaikeiJisseki.TentoZaiko =
                        await ZaikoConnection(postedItem.ShohinId, postedItem.UriageDatetime, postedItem.UriageSu - tempUriageSu, null);

                     postedItem.KaikeiSeq = index + 1;
                }
                this.KaikeiHeader = queriedkaikeiHeader;
            }
            return this.KaikeiHeader;
        }

        public async Task<TentoZaiko> ZaikoConnection(string argShohinId, DateTime argUriageDateTime, decimal argDiffUriageSu, TentoZaiko? argTentoZaiko) {

            TentoZaiko? tentoZaiko = default;
            if (argTentoZaiko is null) {
                tentoZaiko = await _context.TentoZaiko.Where(x => x.ShohinId.Equals(argShohinId)).FirstOrDefaultAsync();
            }
            else {
                tentoZaiko = argTentoZaiko;
            }

            if (tentoZaiko is not null) {
                tentoZaiko.ZaikoSu -= argDiffUriageSu;
                tentoZaiko.ZaikoSu = tentoZaiko.ZaikoSu > 0 ? tentoZaiko.ZaikoSu : 0;
                if (argDiffUriageSu > 0) tentoZaiko.LastUriageDatetime = argUriageDateTime;
            }

            return tentoZaiko;
        }

        private KaikeiJisseki ShohizeiKeisan(IKaikeiJissekiForAdd inKaikeiJisseki, KaikeiJisseki outKaikeiJisseki) {
            decimal shohiZei = default;

            ShohinMaster? shohinmaster = default;
            if (inKaikeiJisseki.ShohinMaster?.ShohinId is null) {
                shohinmaster = _context.ShohinMaster.AsNoTracking()
                    .Where(x => x.ShohinId == inKaikeiJisseki.ShohinId)
                    .FirstOrDefault();
            }
            else {
                shohinmaster = inKaikeiJisseki.ShohinMaster;
            }
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
            outKaikeiJisseki.NaigaiClass = inKaikeiJisseki.NaigaiClass;
            outKaikeiJisseki.ShohiZeiritsu = shohiZei;
            outKaikeiJisseki.ZeikomiKingaku = inKaikeiJisseki.UriageKingaku * (1.0m + outKaikeiJisseki.ShohiZeiritsu / 100.0m);

            return outKaikeiJisseki;
        }
    }
}
