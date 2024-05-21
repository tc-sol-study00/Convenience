using Convenience.Data;
using Convenience.Models.DataModels;
using Microsoft.EntityFrameworkCore;

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

        public IOrderedEnumerable<SokoZaiko> CreateSokoZaikoList(Func<SokoZaiko, object> sortKey, bool descending) {
            IOrderedEnumerable<SokoZaiko> soko;

            var sokodata = _context.SokoZaiko.Include(i => i.ShiireMaster).ThenInclude(j => j.ShohinMaster);

            if (descending) {
                soko = sokodata.OrderByDescending(sortKey);
            }
            else {
                soko = sokodata.OrderBy(sortKey);
            }
            return (soko);
        }
    }
}