using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStudy.Propaties {
    public class Class1 : IDbContext{

        private readonly ConvenienceContext _context;
        public Class1() {
            _context = IDbContext.DbOpen();
        }

        //変数のスコープ
        private string p1;

        void method1() {
            p1 = "p1は、メンバーのp1と同じ";

            string p2;
            {
                p2 = "中かっこのp2と同じ";
                p2 = "入替可能";

                string p3 = "";

                p3 = "p3データ";
            }
        }

        void method2() {

            p1 = "これは共通のp1";

            {
                string p1 = "これは中かっこ内しか通用しないp1";
                p1 = "これも中かっこ内のp1に入る";
            }

            {
                string p1 = "これは中かっこ内しか通用しないp1";
                p1 = "これも中かっこ内のp1に入る";
            }

            IEnumerable<ChumonJissekiMeisai> chumonJissekiMeisais=  _context.ChumonJissekiMeisai;

            foreach( var aChumonJissekiMeisai in chumonJissekiMeisais) {

            }
        }

    }

    public class Class2 {
        Class2() {
            Class1 class1 = new Class1();
        }
    }
}
