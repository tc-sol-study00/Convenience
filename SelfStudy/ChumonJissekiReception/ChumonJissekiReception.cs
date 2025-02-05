using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.Extensions.Logging;
using SelfStudy.ChumonJissekiReception.DTO;
using SelfStudy.ChumonJissekiReception.Interfaces;

namespace SelfStudy.ChumonJissekiReception {
    public class ChumonJissekiReception : IChumonJissekiReception, IEnableLegacyTimestampBehavior, IDisposable {

        private readonly ConvenienceContext _context;
        private readonly IChumonJissekiAccessor _chumonJissekiAccessor;
        private readonly IShiireJissekiAccessor _shiireJissekiAccessor;
        private readonly ISokoZaikoAccesor _sokoZaikoAccesor;
        private readonly DisplayResult _displayResult;

        private const string _DisplayBeforeProcess = "Before Process";
        private const string _Result = "Result";

        private Action<ConvenienceContext> _saveChanges= (context) => context.SaveChanges();
        //private Action<ConvenienceContext> _saveChanges = (context) => Console.WriteLine("DB更新しません");


        public ChumonJissekiReception(ConvenienceContext context) {
            _context = context;
            _chumonJissekiAccessor = new ChumonJissekiAccessor(_context);
            _shiireJissekiAccessor = new ShiireJissekiAccessor(_context);
            _sokoZaikoAccesor = new SokoZaikoAccesor(_context);
            _displayResult = new DisplayResult();
            (this as IEnableLegacyTimestampBehavior).SetSwitch();
        }
        public ChumonJissekiReception() : this(IDbContext.DbOpen(LogLevel.Warning)) {
        }

        private bool _disposed = false;
        public void Dispose() {
            if (!_disposed) {
                //リソースの解放（例：Stream, DB Connection）
                _context?.Dispose();
                GC.SuppressFinalize(this);  //GCがメモリ開放の際に、ファイナライザを呼ばなくて良いと指示
                _disposed = true;
            }
        }
        ~ChumonJissekiReception(){ // ファイナライザー（デストラクタ）
            Dispose();
        }

        public int ChumonJissekiToShiireJisseki() {

            /*
             * 一覧表表示と入力
             */

            //注文残リスト
            IList<ChumonListItem> chumonZanList =
                (_chumonJissekiAccessor as ChumonJissekiAccessor).GetChumonZanList() as IList<ChumonListItem>;

            //注文一覧表示
            for (int i = 0; i < chumonZanList.Count; i++) {
                var aList = chumonZanList[i];
                Console.WriteLine($"{i:000}:{aList.ShiireSakiId}:{aList.ChumonId}");
            }

            //仕入先コードと注文コード入力

            Console.WriteLine(new string('-', 20));
            Console.Write("番号 = ");
            string? indata = Console.ReadLine();

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
            if (chumonJisseki == null) {
                throw new Exception("注文実績エラー");
            }
            else if (chumonJisseki.ChumonJissekiMeisais == null) {
                throw new Exception("注文実績明細エラー");
            }

            /*
             * 処理前の状態を表示
             */

            _displayResult.DisplayData(chumonJisseki, _DisplayBeforeProcess);
            _displayResult.DisplayData(chumonJisseki!.ChumonJissekiMeisais, _DisplayBeforeProcess);

            IList<SokoZaiko> sokoZaikos = new List<SokoZaiko>();
            foreach (var meisai in chumonJisseki.ChumonJissekiMeisais) {
                SokoZaiko? sokoZaiko = _sokoZaikoAccesor.GetSokoZaiko(chumonJisseki.ShiireSakiId, meisai.ShiirePrdId, meisai.ShohinId);
                if(sokoZaiko != null)sokoZaikos.Add(sokoZaiko);
            }
            _displayResult.DisplayData(sokoZaikos, _DisplayBeforeProcess);

            /*
             * 仕入実績処理・倉庫在庫処理
             */

            DateOnly shiireDate = DateOnly.FromDateTime(DateTime.Today);

            //仕入日（今日）と注文IDで、最大のSeqNoを求める

            uint seqByShiireDate = _shiireJissekiAccessor.GetMaxSeqByShiireDate(chumonJisseki!.ChumonId!, shiireDate);
            seqByShiireDate += 1; //次の仕入実績追加用のSeqNo

            //注文実績明細から仕入実績を作る

            foreach (var aMeisai in chumonJisseki.ChumonJissekiMeisais) {

                //仕入実績作成
                var shiireJisseki = _shiireJissekiAccessor.CreateShiireJissekiToDB(aMeisai, chumonJisseki.ShiireSakiId, seqByShiireDate);

                /*
                 * 倉庫在庫処理
                 */
                var sokoZaiko =
                        _sokoZaikoAccesor.GetSokoZaiko(chumonJisseki.ShiireSakiId, aMeisai.ShiirePrdId, aMeisai.ShohinId);
                if (sokoZaiko == null) {    //倉庫在庫がなければ、作成
                    sokoZaiko = _sokoZaikoAccesor.CreateSokoZaiko(chumonJisseki.ShiireSakiId, aMeisai.ShiirePrdId, aMeisai.ShohinId);
                }

                //倉庫在庫情報セット
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

                //SeqNoを一個ずらす
                seqByShiireDate++;

            }

            /*
             * DB更新
             */

            var check = _context.ChangeTracker.Entries();
            _saveChanges(_context);

            /*
             * 結果表示
             */
            ChumonJisseki chumonJissekiForAfterCheck = _chumonJissekiAccessor.GetaChumonJisseki(shiireSakiId, chumonId) ?? new ChumonJisseki();
            _displayResult.DisplayData(chumonJissekiForAfterCheck, _Result);
            if (chumonJissekiForAfterCheck?.ChumonJissekiMeisais != null) {
                _displayResult.DisplayData(chumonJissekiForAfterCheck.ChumonJissekiMeisais, _Result);
            }

            IList<SokoZaiko> sokoZaikoForAfterChecks = new List<SokoZaiko>();
            foreach (var meisai in chumonJisseki.ChumonJissekiMeisais) {
                SokoZaiko? sokoZaikoForAfterCheck = _sokoZaikoAccesor.GetSokoZaiko(chumonJissekiForAfterCheck!.ShiireSakiId, meisai.ShiirePrdId, meisai.ShohinId) ?? new SokoZaiko();
                if (sokoZaikoForAfterCheck != null) 
                    sokoZaikoForAfterChecks.Add(sokoZaikoForAfterCheck);
            }
            _displayResult.DisplayData(sokoZaikoForAfterChecks, _Result);

            IEnumerable<ShiireJisseki> shiireJissekiForAfterChecks = ((ShiireJissekiAccessor)_shiireJissekiAccessor).GetShiireJisseki(chumonId, shiireDate) ?? new List<ShiireJisseki>();
            _displayResult.DisplayData(shiireJissekiForAfterChecks, _Result);

            return 0;
        }

    }
}
