using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Properties;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Services {

    public class ZaikoService {
        private readonly ConvenienceContext _context;

        public Zaiko Zaiko { get; set; }

        public ZaikoService(ConvenienceContext context) {
            _context = context;

            ZaikoClassCreate();
        }

        public ZaikoService() {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var contextOptions = new DbContextOptionsBuilder<ConvenienceContext>()
                .UseNpgsql(configuration["ConvenienceContext"])
                .Options;

            _context = new ConvenienceContext(contextOptions);

            ZaikoClassCreate();
        }

        private void ZaikoClassCreate() {
            Zaiko = new Zaiko(_context);
        }

        public IList<SokoZaiko> KeyInput(string key, bool inDescendig) {
            IOrderedEnumerable<SokoZaiko> sokoZaikos;

            switch (key) {
                case "ShohinId":
                    sokoZaikos = Zaiko.CreateSokoZaikoList(zaiko => zaiko.ShohinId, inDescendig);
                    break;

                case "ShiirePrdId":
                    sokoZaikos = Zaiko.CreateSokoZaikoList(zaiko => zaiko.ShiirePrdId, inDescendig);
                    break;

                case "ShiireSakiId":
                    sokoZaikos = Zaiko.CreateSokoZaikoList(zaiko => zaiko.ShiireSakiId, inDescendig);
                    break;

                case "SokoZaikoCaseSu":
                    sokoZaikos = Zaiko.CreateSokoZaikoList(zaiko => zaiko.SokoZaikoCaseSu, inDescendig);
                    break;

                case "SokoZaikoSu":
                    sokoZaikos = Zaiko.CreateSokoZaikoList(zaiko => zaiko.SokoZaikoSu, inDescendig);
                    break;

                case "ShohinName":
                    sokoZaikos = Zaiko.CreateSokoZaikoList(zaiko => zaiko.ShiireMaster.ShohinMaster.ShohinName, inDescendig);
                    break;

                case null:
                    sokoZaikos = Zaiko.CreateSokoZaikoList(zaiko => zaiko.ShohinId, inDescendig); //指定なしなら商品Idと同じ
                    break;

                default:
                    throw new Exception("ソート項目処理エラー");
            }

            return (sokoZaikos.ToList());
        }
    }
}