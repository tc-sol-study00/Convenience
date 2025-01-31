using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using SelfStudy.ChumonJissekiReception.DTO;
using SelfStudy.ChumonJissekiReception.Interfaces;

namespace SelfStudy.ChumonJissekiReception {
    public class ChumonJissekiReception : IChumonJissekiReception, IEnableLegacyTimestampBehavior,IDisposable {

        private readonly ConvenienceContext _context;
        private readonly IChumonJissekiAccessor _chumonJissekiAccessor;
        private readonly IShiireJissekiAccessor _shiireJissekiAccessor;
        private readonly ISokoZaikoAccesor _sokoZaikoAccesor;

        public ChumonJissekiReception(ConvenienceContext context) {
            _context = context;
            _chumonJissekiAccessor = new ChumonJissekiAccessor(_context);
            _shiireJissekiAccessor = new ShiireJissekiAccessor(_context);
            _sokoZaikoAccesor = new SokoZaikoAccesor(_context);
            (this as IEnableLegacyTimestampBehavior).SetSwitch();
        }
        public ChumonJissekiReception() : this(IDbContext.DbOpen()) {

        }

        public void Dispose() {
            _context?.Dispose();
        }

        public int ChumonJissekiToShiireJisseki() {

            /*
             * 一覧表表示と入力
             */

            //注文残リスト
            IList<ChumonListItem> chumonZanList = 
                _chumonJissekiAccessor.GetChumonZanList() as IList<ChumonListItem>;


            //注文一覧表示
            for (int i = 0; i < chumonZanList.Count; i++) {
                var aList = chumonZanList[i];
                Console.WriteLine($"{i:000}:{aList.ShiireSakiId}:{aList.ChumonId}");
            }

            //仕入先コードと注文コード入力

            Console.WriteLine(new string('-',20));
            Console.Write("番号 = ");
            string indata = Console.ReadLine();

            if (string.IsNullOrEmpty(indata)) {
                return 0;
            }
            int index = int.Parse(indata);
            (string shiireSakiId, string chumonId) = 
                (chumonZanList[index].ShiireSakiId, chumonZanList[index].ChumonId);

            /*
             * 注文実績処理
             */

            //注文実績取得
            ChumonJisseki? chumonJisseki = _chumonJissekiAccessor.GetaChumonJisseki(shiireSakiId, chumonId);

            /*
             * 仕入実績処理・倉庫在庫処理
             */

            DateOnly shiireDate = DateOnly.FromDateTime(DateTime.Today);
            
            //仕入日（今日）と注文IDで、最大のSeqNoを求める

            uint seqByShiireDate = _shiireJissekiAccessor.GetMaxSeqByShiireDate(chumonJisseki.ChumonId, shiireDate) + 1;

            //注文実績明細から仕入実績を作る

            foreach (var aMeisai in chumonJisseki.ChumonJissekiMeisais) {

                //仕入実績作成
                var shiireJisseki = _shiireJissekiAccessor.CreateShiireJissekiToDB(aMeisai, chumonJisseki.ShiireSakiId, seqByShiireDate);

                /*
                 * 倉庫在庫処理
                 */
                var sokoZaiko =
                        _sokoZaikoAccesor.GetSokoZaiko(chumonJisseki.ShiireSakiId, aMeisai.ShiirePrdId, aMeisai.ShohinId);
                if (sokoZaiko == null) {
                    sokoZaiko = _sokoZaikoAccesor.CreateSokoZaiko(chumonJisseki.ShiireSakiId, aMeisai.ShiirePrdId, aMeisai.ShohinId);
                }

                var ShiirePcsPerUnit = aMeisai.ShiireMaster.ShiirePcsPerUnit;
                var ShiireCaseSu = aMeisai.ChumonZan;
                var ShiireSu = ShiireCaseSu * ShiirePcsPerUnit;

                sokoZaiko.SokoZaikoCaseSu += ShiireCaseSu;
                sokoZaiko.SokoZaikoSu += ShiireSu;
                sokoZaiko.LastShiireDate = shiireDate;

                /*
                 * 注文残の処理
                 */
                aMeisai.ChumonZan -= ShiireCaseSu;

            }

            /*
             * DB更新
             */

            var check = _context.ChangeTracker.Entries();
            _context.SaveChanges();

            return 0;
        }
    }
}
