using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debug {
    public class Study20250123withJoin : IDbContext {

        private readonly ConvenienceContext _context;
        public Study20250123withJoin() {
            _context = IDbContext.DbOpen();
        }

        public void LINQStudy() {
            //内部結合（注文実績→明細）
            var result1 = _context.ChumonJisseki
                .Join(_context.ChumonJissekiMeisai,
                      cj => new { cj.ChumonId, cj.ShiireSakiId },
                      cjm => new { cjm.ChumonId, cjm.ShiireSakiId },
                      (cj, cjm) => new {
                          cj.ChumonId,
                          cj.ShiireSakiId,
                          cj.ChumonDate,
                          cjm.ShiirePrdId,
                          cjm.ChumonSu
                      })
                .ToList();

            //外部結合（注文実績→明細）
                var results2 = _context.ChumonJisseki
                .GroupJoin(
                    _context.ChumonJissekiMeisai,
                        cj => new { cj.ChumonId, cj.ShiireSakiId },
                        cjm => new { cjm.ChumonId, cjm.ShiireSakiId },
                    (cj, cjm) => new { cj, cjm }
                )
                .SelectMany(
                    x => x.cjm.DefaultIfEmpty(),
                    (x, fr) => new {
                        x.cj,
                        fr
                    }
                )
                .ToList();


        }
    }
}
