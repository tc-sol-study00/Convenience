using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Properties {
    public class Chumon2 : IChumon {

        private readonly ConvenienceContext _context;
        public ChumonJisseki ChumonJisseki { get; set; }

        public Chumon2() {
            //DBコンテクスト作成
            var contextOptions = new DbContextOptionsBuilder<ConvenienceContext>()
                .UseNpgsql("Server=localhost;Port=5432;Username=xxx;Password=xxx;Database=postgres;")
                .Options;
            _context = new ConvenienceContext(contextOptions));
        }

        public ChumonJisseki ChumonSakusei(string inShireSakiId) { 
        }

        public ChumonJisseki ChumonToiawase(string inShireSakiId, DateOnly inChumonDate) { }

        public ChumonJisseki ChumonUpdate(ChumonJisseki inChumonJisseki) { }

        public string ChumonIdHatsuban() { }


        }
}
