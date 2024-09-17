using Microsoft.EntityFrameworkCore;

namespace Convenience.Data {

    public class ConvenienceContext : DbContext {

        public ConvenienceContext(DbContextOptions<ConvenienceContext> options)
            : base(options) {
        }

        public DbSet<Convenience.Models.DataModels.ChumonJissekiMeisai> ChumonJissekiMeisai { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.ChumonJisseki> ChumonJisseki { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.ShiireJisseki> ShiireJisseki { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.ShiireMaster> ShireMaster { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.ShiireSakiMaster> ShiireSakiMaster { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.ShohinMaster> ShohinMaster { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.SokoZaiko> SokoZaiko { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.TentoZaiko> TentoZaiko { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.TentoHaraidashiJisseki> TentoHaraidashiJisseki { get; set; } = default!;
        public DbSet<Convenience.Models.DataModels.KaikeiJisseki> KaikeiJisseki { get; set; } = default!;
    }
}