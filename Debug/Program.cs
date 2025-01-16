using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Debug {
    public class Program {

        static async Task Main(string[] args) {
            await new Study20250108().Study01();

            new Study20250109().Valuestudy();

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

}