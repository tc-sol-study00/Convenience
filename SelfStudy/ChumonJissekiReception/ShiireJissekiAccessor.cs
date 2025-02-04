using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using SelfStudy.ChumonJissekiReception.Interfaces;

namespace SelfStudy.ChumonJissekiReception {
    /// <summary>
    /// 仕入実績アクセサー
    /// </summary>
    public class ShiireJissekiAccessor : IShiireJissekiAccessor {
        private readonly ConvenienceContext _context;

        public ShiireJisseki? ShiireJisseki { get; set; }
        public ShiireJissekiAccessor(ConvenienceContext context) {
            _context = context;
        }
        public ShiireJissekiAccessor() : this(IDbContext.DbOpen()) {
        }
        /// <summary>
        /// 最大の仕入SEQを求める
        /// </summary>
        /// <param name="inChumonId">注文コード</param>
        /// <param name="inShiireDate">仕入日付</param>
        /// <returns></returns>
        public uint GetMaxSeqByShiireDate(string inChumonId, DateOnly inShiireDate) {

            uint maxSeq = _context.ShiireJisseki
                .Where(sj => sj.ChumonId == inChumonId && sj.ShiireDate == inShiireDate)
                .Select(x => (uint?)x.SeqByShiireDate)
                .Max() ?? 0;
            //もしDBにデータがなかったら0を返却

            return maxSeq;
        }

        public IEnumerable<ShiireJisseki> GetShiireJisseki(string inChumonId, DateOnly inShiireDate) {
            return _context.ShiireJisseki.AsNoTracking()
                    .Where(sj => sj.ChumonId == inChumonId && sj.ShiireDate == inShiireDate)
                    .ToList();
        }
        /// <summary>
        /// 仕入実績作成
        /// </summary>
        /// <param name="inChumonJissekiMeisai">注文実績明細</param>
        /// <param name="inShiireSakiId">仕入先コード</param>
        /// <param name="inSeqByShiireDate">SeqNo</param>
        /// <returns>仕入実績</returns>
        public ShiireJisseki CreateShiireJissekiToDB(ChumonJissekiMeisai inChumonJissekiMeisai, string inShiireSakiId, uint inSeqByShiireDate) {
            DateTime currentTime = DateTime.Now;
            DateOnly today = DateOnly.FromDateTime(currentTime);

            decimal shiireCaseSu = inChumonJissekiMeisai.ChumonZan;

            this.ShiireJisseki = new ShiireJisseki() {
                ChumonId = inChumonJissekiMeisai.ChumonId,
                ShiireDate = today,
                SeqByShiireDate = inSeqByShiireDate,
                ShiireDateTime = currentTime,
                ShiireSakiId = inShiireSakiId,
                ShiirePrdId = inChumonJissekiMeisai.ShiirePrdId,
                ShohinId = inChumonJissekiMeisai.ShohinId,
                NonyuSu = shiireCaseSu
            };

            //DBに追加準備
            _context.Add(ShiireJisseki);

            return ShiireJisseki;
        }
    }
}
