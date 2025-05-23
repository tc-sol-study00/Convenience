using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Data;
using System.Linq.Expressions;


namespace Debug 
    {
    public class Program {

        private readonly ListTypeStudy listTypeStudy;
        static async Task Main(string[] args) {

            //await new Study20250108().Study01();
            //new Study20250109().Valuestudy();
            //new Study20250123withJoin().LINQStudy();

            ListTypeStudy listTypeStudy = new ListTypeStudy();
            
            IEnumerable<ChumonJissekiMeisai> data= listTypeStudy.QueryChumonJissekiMeisai();
            IList<DistictChumon> distictChumons=listTypeStudy.SummaryGroupByChumonId(data);
        }


        public class Study20250108 : IDbContext {
            void w(string data) {
                Console.WriteLine($"{data}");
            }

            int converToInt(string strdata) {
                return int.Parse(strdata);
            }

            public async Task Study01() {

                //デリゲート（アクション型）
                Action<string> w = data => Console.WriteLine($"{data}");
                //処理①
                w("なにか");

                w = data2 => Console.WriteLine($"結果={data2}");
                //処理②
                w("なにか");

                //デリゲート（関数型）
                Func<string, int> converToInt = strdata => int.Parse(strdata);
                Func<decimal, string, decimal> shohiZei = (price, naigai)
                    => naigai == "Nai" ? price * 1.1m : naigai == "Gai" ? price * 1.08m : price;

                Func<decimal, string, decimal> shohiZei2 = (price, naigai)
                    => {
                        if (naigai == "Nai") {
                            price = price * 1.1m;
                        }
                        else {
                            price = price * 0.8m;
                        }
                        return price;
                    };


                w("スタート");
                Console.WriteLine("スタート");


                var a = shohiZei(100, "Nai");
                w("エンド");

                //サービス・プロパティのデバッグ

                Chumon chumon = new Chumon();
                ChumonJisseki? data = await chumon.ChumonToiawase("A000000001", DateOnly.Parse("2024-10-17"));

                ChumonJisseki? postChumonJisseki = await new Chumon().ChumonToiawase("A000000001", DateOnly.Parse("2024-10-17"));
                postChumonJisseki?.ChumonJissekiMeisais?.ToList().ForEach(x => x.ChumonSu = 200);

                ChumonJisseki updatedChumonJisseki = await chumon.ChumonUpdate(postChumonJisseki);

                //var context=((IDbContext2)this).DbOpen();

            }


        }


    }

    public class Study20250109 {

        public void Valuestudy() {

            //double decimal
            double minDouble = double.MinValue;

            double maxDouble = double.MaxValue;

            decimal minDecimal = decimal.MinValue;

            decimal maxDecimal = decimal.MaxValue;

            string data = "123";

            int w = StringExtensions.ConvertToInt(data);

            //拡張メソッド
            int result = data.ConvertToInt();

        }
    }

    public static class StringExtensions {
        // string を int に変換する拡張メソッド
        public static int ConvertToInt(this string data) {
            return int.Parse(data);
        }
    }

    public class Study20250110 {
    }


    public class cls1 {
        void Hashiru() {
            Console.WriteLine("cls1が走る");
        }
    }

    public class Study20250120 {

        Func<int, int> Tashizan = x => x + 3;
        Func<int, int> Tashizan2 = y => y + 3;

        int method1(int a, int b) {
            return a + b;
        }

    }

    public class Study20250121 : IDbContext {

        private readonly ConvenienceContext _context;
        public Study20250121() {
            _context = IDbContext.DbOpen();
        }

        public void MainProc() {
            Console.WriteLine(DataConv<int, string>(1));
            Console.WriteLine(DataConv<string, int>("1"));
        }

        private T2 DataConv<T1, T2>(T1 indata) {
            int result = default;

            if (typeof(T1) == typeof(string)) {
                if (int.TryParse(indata as string, out int value)) {
                    result = value + 10;
                }
                else {
                    return default(T2);
                }
            }
            else if (typeof(T1) == typeof(int)) {
                result = (int)(object)indata + 10;
            }
            else {
                return default(T2);
            }

            if (typeof(T2) == typeof(string)) {
                return (T2)(object)result.ToString();
            }

            return (T2)(object)result;
        }

        private IQueryable<T> GetData<T>(string propertyName, object value) where T : class, ISelectList {

            var result = _context.Set<T>().OrderBy(d => d.Value);
            return result;
        }


    }

    public class Study20250123 : IDbContext {

        private readonly ConvenienceContext _context;  
        public Study20250123() {
            _context = IDbContext.DbOpen();
        }

        public IEnumerable<ChumonJisseki> ChumonList() {

            IEnumerable<ChumonJisseki> chumonJissekis = 
                _context.ChumonJisseki.AsNoTracking()
                .Include( x => x.ChumonJissekiMeisais)
                .ThenInclude( x => x.ShiireMaster)
                .ThenInclude(x => x.ShohinMaster)
                .Include(x => x.ShiireSakiMaster)
                .ToList();

            //複写

            IList<ChumonJisseki> cpiedChumonJissekis = new List<ChumonJisseki>();

            foreach(var aChumonJisseki in chumonJissekis) {

                //注文実績編集

                ChumonJisseki editChumonJisseki = new ChumonJisseki();
                editChumonJisseki.ShiireSakiId = aChumonJisseki.ShiireSakiId;
                editChumonJisseki.ChumonId = aChumonJisseki.ChumonId;
                editChumonJisseki.ChumonDate = aChumonJisseki.ChumonDate;
                editChumonJisseki.ShiireSakiMaster = aChumonJisseki.ShiireSakiMaster;

                IList<ChumonJissekiMeisai> cpiedChumonJissekiMeisais = new List<ChumonJissekiMeisai>();

                foreach ( var aChumonJissekimeisai in aChumonJisseki.ChumonJissekiMeisais) {

                }

            }


            return chumonJissekis;




        }

    }

}