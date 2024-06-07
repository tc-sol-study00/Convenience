using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Microsoft.EntityFrameworkCore;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;

namespace Convenience.Models.Services {

    public class ZaikoService : IZaikoService {
        private readonly ConvenienceContext _context;

        public IZaiko Zaiko { get; set; }

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

        public async Task<IList<ZaikoListLine>> KeyInput(string key, bool inDescendig) {
            
            IList<ZaikoListLine> sokoZaikos;

            switch (key) {
                case "ShohinId":
                    sokoZaikos = await Zaiko.CreateSokoZaikoList<SokoZaiko,string>(z => z.ShohinId, inDescendig);
                    break;

                case "ShiirePrdId":
                    sokoZaikos = await Zaiko.CreateSokoZaikoList<SokoZaiko, string>(z => z.ShiirePrdId, inDescendig);
                    break;

                case "ShiireSakiId":
                    sokoZaikos = await Zaiko.CreateSokoZaikoList<SokoZaiko, string>(z => z.ShiireSakiId, inDescendig);
                    break;

                case "SokoZaikoCaseSu":
                    sokoZaikos = await Zaiko.CreateSokoZaikoList<SokoZaiko, decimal>(z => z.SokoZaikoCaseSu, inDescendig);
                    break;

                case "SokoZaikoSu":
                    sokoZaikos = await Zaiko.CreateSokoZaikoList<SokoZaiko, decimal>(z => z.SokoZaikoSu, inDescendig);
                    break;

                case "ShohinName":
                    sokoZaikos = await Zaiko.CreateSokoZaikoList<SokoZaiko, string>(z => z.ShiireMaster.ShohinMaster.ShohinName, inDescendig);
                    break;
                case "ChumonZan":
                    sokoZaikos = await Zaiko.CreateSokoZaikoList<ZaikoListLine, decimal>(z => z.ChumonJissekiMeisai.ChumonZan, inDescendig);
                    break;
                case null:
                    sokoZaikos = await Zaiko.CreateSokoZaikoList<SokoZaiko, string>(z => z.ShohinId, inDescendig); //指定なしなら商品Idと同じ
                    break;

                default:
                    throw new Exception("ソート項目処理エラー");
            }

            return (sokoZaikos);
        }
    }
}