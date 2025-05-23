using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debug {


    public class DistictChumon {
        public string ChumonId { get; set; }

        public decimal ChumonSu { get; set; }
    }
    internal class ListTypeStudy {

        private readonly ConvenienceContext _context;

        public ListTypeStudy() {
            _context = IDbContext.DbOpen();
        }

        /// <summary>
        /// 遅延実行①
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ChumonJissekiMeisai> QueryChumonJissekiMeisai() {
            //ToListやFirst/FirstOrDefaultなどを実行せずに、IEnumerableで受けると実際のSQL文は発行されない
            IEnumerable<ChumonJissekiMeisai> result=_context.ChumonJissekiMeisai;

            return result;
        }
        /// <summary>
        /// 遅延実行②
        /// </summary>
        /// <param name="argdata"></param>
        /// <returns></returns>
        /// 
        /// IEnumerableは、配列型のすべての継承元となる
        public IList<DistictChumon> SummaryGroupByChumonId(IEnumerable<ChumonJissekiMeisai> argdata) {

            //注文ID毎に注文数を集計しているが、まだ、SQL文が実行される
            IEnumerable<DistictChumon> enumresultData =
                argdata
                .GroupBy(x => x.ChumonId)
                .Select(g => new DistictChumon {
                    ChumonId = g.Key,
                    ChumonSu = g.Sum(x => x.ChumonSu)
                });

            //SQL文実行
            IList<DistictChumon> resultData =enumresultData.ToList();  //ここで初めて実行

            //IEnumerablehは件数をリアルに管理していない。Count()で実際にデータ件数を数えている
            Console.WriteLine(argdata.Count());

            //IListは件数をリアルに管理している。
            Console.WriteLine(resultData.Count);
            return resultData;

        }

        public void SamaZamaNaType() {
            List<ChumonJissekiMeisai> chumonJissekiMeisais = _context.ChumonJissekiMeisai.ToList();

            IEnumerable<ChumonJissekiMeisai> enumerableChumonJissekiMeisais = chumonJissekiMeisais;

            ICollection<ChumonJissekiMeisai> collectionChumonJissekiMeisais = chumonJissekiMeisais;

            IList<ChumonJissekiMeisai> listChumonJissekiMeisais = chumonJissekiMeisais;

            /*
             * IEnumerable
             */

            //Foreachができる
            foreach (ChumonJissekiMeisai aChumonJissekiMeisai in enumerableChumonJissekiMeisais) {

            }

            /*
             * ICollecttion
             */

            //IEnumerableを継承しているので、foreach可能

            foreach (ChumonJissekiMeisai aChumonJissekiMeisai in collectionChumonJissekiMeisais) {

            }

            //ICollecttionで追加になった機能

            //AddやRemoveができる
            collectionChumonJissekiMeisais.Add(new ChumonJissekiMeisai() { ChumonId = "20250510-001"});
            collectionChumonJissekiMeisais.Remove(new ChumonJissekiMeisai() { ChumonId = "20250510-001" });

            //Countプロパティがある
            var count=collectionChumonJissekiMeisais.Count;

            /*
             * IList
             */
            //IEnumerableを継承しているので、foreach可能

            foreach (ChumonJissekiMeisai aChumonJissekiMeisai in listChumonJissekiMeisais) {

            }

            //ICollecttionで追加になった機能

            //AddやRemoveができる
            listChumonJissekiMeisais.Add(new ChumonJissekiMeisai() { ChumonId = "20250510-001" });
            listChumonJissekiMeisais.Remove(new ChumonJissekiMeisai() { ChumonId = "20250510-001" });

            //Countプロパティがある
            var count2 = listChumonJissekiMeisais.Count;

            //IListで追加になった機能

            //インデクサが使える
            var chumonJissekiMeisai=listChumonJissekiMeisais[0];
        }

        /*
         * 引数で、やる範囲を宣言する
         */

        public void Method1(IEnumerable<ChumonJissekiMeisai> argChumonJissekiMeisais) {
            //この引数で、Icollection、Ilistも受けれる
            //add、removeしない宣言
        }

    }
}
