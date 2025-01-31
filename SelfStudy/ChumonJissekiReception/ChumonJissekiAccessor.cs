using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using SelfStudy.ChumonJissekiReception.DTO;
using SelfStudy.ChumonJissekiReception.Interfaces;
using static Convenience.Models.Properties.Config.CSVMapping;
using static Convenience.Models.Properties.Shiire;

namespace SelfStudy.ChumonJissekiReception {
    /// <summary>
    /// 注文実績アクセサー
    /// </summary>
    public class ChumonJissekiAccessor : IChumonJissekiAccessor {
        /*
         * 領域定義
         */
        private readonly ConvenienceContext _context;

        public IEnumerable<ChumonListItem> ChumonZanList { get; set; }
        public ChumonJisseki? ChumonJisseki { get; set; }

        /*
         * コンストラクタ
         */
        public ChumonJissekiAccessor(ConvenienceContext context) {
            _context = context;
        }
        public ChumonJissekiAccessor() : this(IDbContext.DbOpen()) {
        }


        /// <summary>
        /// 注文実績取得
        /// </summary>
        /// <param name="inChumonJissekis">注文実績一覧</param>
        /// <param name="inShiireSakiId">仕入先コード</param>
        /// <param name="inChumonId">注文コード</param>
        /// <returns>注文実績</returns>
        public ChumonJisseki? GetaChumonJisseki(string inShiireSakiId, string inChumonId) {

            ChumonJisseki = _context.ChumonJisseki
                .Include(cj => cj.ChumonJissekiMeisais!)
                .ThenInclude(cm => cm.ShiireMaster)
                .ThenInclude(sm => sm.ShohinMaster)
                .Where(x => x.ShiireSakiId == inShiireSakiId && x.ChumonId == inChumonId)
                .FirstOrDefault();

            return ChumonJisseki;
        }

        /// <summary>
        /// 注文リスト作成
        /// </summary>
        /// <returns>注文リスト</returns>
        public IEnumerable<ChumonListItem> GetChumonZanList() {
                
            ChumonZanList = _context.ChumonJisseki
                .AsNoTracking()
                .Include(cj => cj.ChumonJissekiMeisais)
                .GroupBy(cj => new { cj.ShiireSakiId, cj.ChumonId })
                .Select(g => new ChumonListItem 
                    { 
                        ShiireSakiId = g.Key.ShiireSakiId, 
                        ChumonId = g.Key.ChumonId!,
                        ChumonZan = g.SelectMany(cj => cj.ChumonJissekiMeisais!).Sum(m => m.ChumonZan)
                    }
                ).Where(cl => cl.ChumonZan > 0)
                .OrderBy(cl => cl.ShiireSakiId)
                .ThenBy(cl => cl.ChumonId)
                .ToList();

            return ChumonZanList;
        }
    }
}
