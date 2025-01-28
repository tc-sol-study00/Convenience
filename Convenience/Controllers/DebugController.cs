using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace Convenience.Controllers {
    public class DebugController : Controller {

        private readonly ConvenienceContext _context;
        public DebugController(ConvenienceContext context) {
            _context = context;
        }
        public async Task<IActionResult> Index() {
            Chumon chumon = new Chumon();

            DateOnly thisday = DateOnly.FromDateTime(DateTime.Now);

            //元々、発番はprivateだったので、コメントアウト
            //string result=await chumon.ChumonIdHatsuban(thisday);

            return View();
        }

        public async Task<IActionResult> Index20241226() {
            //中村さんの質問
            //教科書36

            ChumonJisseki chumonJisseki = new ChumonJisseki() { ChumonId = "20241226-001" };

            ChumonJisseki cpyChumonJisseki = chumonJisseki;

            cpyChumonJisseki.ChumonId = "20241225-001";

            //chumonJissekiのChumonIdは？

            //藤原さんの質問
            ChumonJisseki? result1=await _context.ChumonJisseki
                .FirstOrDefaultAsync();

            List<int> intdatas = new List<int>();

            var result0 = intdatas.FirstOrDefault();

            List<ChumonJisseki> result10 = await _context.ChumonJisseki
                //.Where(x => x.ChumonId == "20241226-001")
                .ToListAsync();



            Response.ContentType = "text/html";
            return Content(string.Join("<br>",result10.Select(x=> $"{x.ChumonId}:{x.ChumonDate}")));
        }

        /*
        public async Task<ActionResult> Index20241226No01() {
            IEnumerable<ChumonJissekiMeisai> postedChumonJisseki 
                = await _context.ChumonJissekiMeisai.AsNoTracking()
                .OrderBy(t => t.ChumonId).ThenBy(t => t.ShiireSakiId).ThenBy(p => p.ShiirePrdId)
                .ToListAsync();

            IList<ChumonJissekiMeisai> existedChumonJisseki 
                = await _context.ChumonJissekiMeisai
                .OrderBy(o => o.ChumonId).ThenBy(t => t.ShiireSakiId).ThenBy(p => p.ShiirePrdId)
                .ToListAsync();

            _ = existedChumonJisseki.Zip(postedChumonJisseki,(ext,pst) => ext.ChumonSu = pst.ChumonSu);

        }

        */

    }
}
