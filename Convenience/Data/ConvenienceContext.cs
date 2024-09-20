using Microsoft.EntityFrameworkCore;

namespace Convenience.Data {
    /// <summary>
    /// DBコンテキスト
    /// </summary>
    public class ConvenienceContext : DbContext {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="options"></param>
        public ConvenienceContext(DbContextOptions<ConvenienceContext> options)
            : base(options) {
        }
        /// <summary>
        /// 注文実績明細
        /// </summary>
        public DbSet<Convenience.Models.DataModels.ChumonJissekiMeisai> ChumonJissekiMeisai { get; set; } = default!;
        /// <summary>
        /// 注文実績
        /// </summary>
        public DbSet<Convenience.Models.DataModels.ChumonJisseki> ChumonJisseki { get; set; } = default!;
        /// <summary>
        /// 仕入実績
        /// </summary>
        public DbSet<Convenience.Models.DataModels.ShiireJisseki> ShiireJisseki { get; set; } = default!;
        /// <summary>
        /// 仕入マスタ
        /// </summary>
        public DbSet<Convenience.Models.DataModels.ShiireMaster> ShireMaster { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.ShiireSakiMaster> ShiireSakiMaster { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.ShohinMaster> ShohinMaster { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.SokoZaiko> SokoZaiko { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.TentoZaiko> TentoZaiko { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.TentoHaraidashiJisseki> TentoHaraidashiJisseki { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.KaikeiJisseki> KaikeiJisseki { get; set; } = default!;
    }
}