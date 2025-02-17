using Convenience.Models.DataModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SelfStudy.ChumonJissekiReception;
using SelfStudy.ChumonJissekiReception.Interfaces;
using SelfStudy.Interfaces;
using SelfStudy.Propaties;

namespace SelfStudy {
    internal class Program {
        static async Task Main(string[] args) {

            int x = 5;
            switch (x) {
                case 1:
                    IObjectCopy objectCopy = new ObjectCopy(1);
                    IObjectCopy objectCopy2 = new ObjectCopy(2);

                    var task1 = objectCopy.ObjectCopyController();
                    var task2 = objectCopy2.ObjectCopyController();

                    IList<ChumonJisseki> chumonJissekis = await task1;
                    IList<ChumonJisseki> chumonJissekis2 = await task2;
                    break;
                case 2:
                    using (var chumonSummary = new ChumonSummary()) {
                        chumonSummary.MakeChumonSummary();
                    }
                    break;
                case 3:
                    using (IChumonJissekiReception chumonJissekiReception
                        = new ChumonJissekiReception.ChumonJissekiReception()) {
                        try {
                            chumonJissekiReception.ChumonJissekiToShiireJisseki();
                        }
                        catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                            throw;
                        }
                    }
                    GC.Collect(); // ガベージコレクションを強制実行
                    //GC.WaitForPendingFinalizers(); // 保留中のデストラクタを実行
                    break;

                case 4: {
                        using IChumonJissekiReception chumonJissekiReception
                            = new ChumonJissekiReception.ChumonJissekiReception();
                        try {
                            chumonJissekiReception.ChumonJissekiToShiireJisseki();
                        }
                        catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                            throw;
                        }
                    }
                    break;

                case 5:
                    IChumonJissekiReception chumonJissekiReception2
                        = new ChumonJissekiReception.ChumonJissekiReception();
                    try {
                        chumonJissekiReception2.ChumonJissekiToShiireJisseki();
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    chumonJissekiReception2.Dispose();
                    break;
                case 6:
                    var chumonJisseki = new ChumonJisseki() { ChumonId = "00000000-001" };
                    break;
            }
        }
    }
}
