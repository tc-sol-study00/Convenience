using Convenience.Models.DataModels;
using SelfStudy.Interfaces;
using SelfStudy.Propaties;

namespace SelfStudy {
    internal class Program {
        static async Task Main(string[] args) {

            int x = 2;
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
            }
        }
    }
}
