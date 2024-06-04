using Convenience.Data;
using Convenience.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Convenience.Models.Properties {

    public class Zaiko {
        private readonly ConvenienceContext _context;
        public IList<SokoZaiko> SokoZaikos { get; set; }

        public Zaiko() {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var contextOptions = new DbContextOptionsBuilder<ConvenienceContext>()
                .UseNpgsql(configuration["ConvenienceContext"])
                .Options;

            _context = new ConvenienceContext(contextOptions);
        }

        public Zaiko(ConvenienceContext context) {
            _context = context;
        }

        public async Task<IList<SokoZaiko>> CreateSokoZaikoList<TKey>(Expression<Func<SokoZaiko, TKey>> sortKey, bool descending) {

            IQueryable<SokoZaiko> sokodata = _context.SokoZaiko.Include(i => i.ShiireMaster).ThenInclude(j => j.ShohinMaster);
            if (descending) {
                sokodata = sokodata.OrderByDescending(sortKey);
            }
            else {
                sokodata = sokodata.OrderBy(sortKey);
            }
            return (await sokodata.ToListAsync());
        }
    }
}