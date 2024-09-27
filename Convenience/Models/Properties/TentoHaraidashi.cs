using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convenience.Models.Properties {
    public class TentoHaraidashi : ITentoHaraidashi {

        private readonly ConvenienceContext _context;
        public TentoHaraidashiJisseki TentoHaraidashiJisseki { get; set; }

        public TentoHaraidashi(ConvenienceContext context) {
            this._context = context;
        }

        public async Task<IEnumerable<TentoHaraidashiJisseki>> TentoHaraidashiSakusei(DateTime argCurrentDateTime) {

            /*
             * 在庫のある倉庫在庫一覧を作り、店頭払出実績を作る
             */

            IEnumerable<TentoHaraidashiJissekiForView> tentoHaraidashiJissekiForView = await _context.SokoZaiko.AsNoTracking()
                .Where(x => x.SokoZaikoCaseSu > 0 && x.SokoZaikoSu > 0)
                .GroupJoin(
                    _context.ShiireMaster,
                    sokoZaiko => new { sokoZaiko.ShiireSakiId, sokoZaiko.ShiirePrdId, sokoZaiko.ShohinId },
                    shiireMaster => new { shiireMaster.ShiireSakiId, shiireMaster.ShiirePrdId, shiireMaster.ShohinId },
                    (sokoZaiko, shiireMaster) => new { SokoZaiko = sokoZaiko, ShiireMaster = shiireMaster.FirstOrDefault() }
                )
                .GroupJoin(
                    _context.TentoZaiko,
                    src => new { src.SokoZaiko.ShohinId },
                    tentoZaiko => new { tentoZaiko.ShohinId },
                    (src, tentoZaiko) => new { SokoZaiko = src.SokoZaiko, ShiireMaster = src.ShiireMaster, TenToZaiko = tentoZaiko.FirstOrDefault() }
                )
                .Select(x => new TentoHaraidashiJissekiForView {
                    ShiireSakiId = x.SokoZaiko.ShiireSakiId,
                    ShiirePrdId = x.SokoZaiko.ShiirePrdId,
                    ShohinId = x.SokoZaiko.ShohinId,
                    ShireDateTime = x.SokoZaiko.LastShiireDate,
                    HaraidashiDate = argCurrentDateTime,
                    HaraidashiCaseSu = 0,
                    HaraidashiSu = 0,
                    ShiireMaster = x.ShiireMaster,
                    SokoZaiko = x.SokoZaiko
                }).ToListAsync();

            return (tentoHaraidashiJissekiForView);
        }
    }
}
