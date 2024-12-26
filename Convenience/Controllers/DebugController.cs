using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Controllers {
    public class DebugController : Controller {

        private readonly ConvenienceContext _context;
        public DebugController(ConvenienceContext context) {
            _context = context;
        }
        public async Task<IActionResult> Index2() {
            Chumon chumon = new Chumon();

            DateOnly thisday = DateOnly.FromDateTime(DateTime.Now);

            string result=await chumon.ChumonIdHatsuban(thisday);

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
            ChumonJisseki result1=await _context.ChumonJisseki
                .FirstOrDefaultAsync();

            int i;

            ChumonJisseki chumonJisseki2;

            List<int> intdatas = new List<int>();

            var result0 = intdatas.FirstOrDefault();

            List<ChumonJisseki> result10 = await _context.ChumonJisseki.Where(x => x.ChumonId == "20241226-001")
                .ToListAsync();

            return View();
        }

    }
}
