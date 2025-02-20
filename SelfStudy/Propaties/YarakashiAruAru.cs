using Convenience.Data;
using Convenience.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStudy.Propaties {
    public  class YarakashiAruAru {

        private readonly ConvenienceContext _context;
        public YarakashiAruAru(ConvenienceContext context ) {
            _context = context;
        }

        public void CaseStudy01() {
            //ダメダメ系

            //Whereで、主キー指しているわけだからListじゃないのに、リストにしている
            var chumonJisseki=_context.ChumonJisseki.Where(x => x.ShiireSakiId == "xxx" && x.ChumonId == "xxxx").ToList();

            //変数宣言のために、var使っている
            var cmon= chumonJisseki[0].ChumonId;

            //一件しかこないのに、リストだから定番のforeachを使っているけど、変数はひとつだけ。意味なし
            foreach (var data in chumonJisseki) {
                cmon=data.ChumonId;
            }

            //ここはまあ許せるけど
            var cmeisai = _context.ChumonJissekiMeisai.Where(x => x.ChumonId == cmon).ToList();
            
            //これは許せない。０件データだと絶対アベンド
            var cmeisai2 = _context.ChumonJissekiMeisai.Where(x => x.ChumonId == chumonJisseki[0].ChumonId).ToList();

        }
        public void CaseStudy02() {
            //良い子系
            ChumonJisseki? chumonJisseki 
                = _context.ChumonJisseki
                    .Where(x => x.ShiireSakiId == "xxx" && x.ChumonId == "xxxx")
                    .FirstOrDefault();

            if (chumonJisseki != null) {
                string? cmon = chumonJisseki.ChumonId;

                IEnumerable<ChumonJissekiMeisai> cmeisai = 
                    _context.ChumonJissekiMeisai.Where(x => x.ChumonId == cmon).ToList();
            }
            else {
                //例外処置
            }
        }
    }

}
