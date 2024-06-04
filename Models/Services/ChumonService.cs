using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Chumon;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static Convenience.Models.Properties.Message;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Convenience.Models.Services {

    public class ChumonService : IChumonService, IDbContext {
        /*
         * 注文サービスクラス
         */

        /*
         * 共通変数
         */

        //DBコンテクスト
        private readonly ConvenienceContext _context;

        //注文オブジェクト用
        public IChumon chumon { get; set; }

        /*
         * コンストラクタ
         */

        //注文オブジェクト用記述
        private Func<ConvenienceContext, IChumon> CreateChumonInstance = context => new Chumon(context);

        //通常用
        public ChumonService(ConvenienceContext context) {
            _context = context;
            chumon = CreateChumonInstance(_context);
        }

        //デバッグ用
        public ChumonService() {
            _context = IDbContext.DbOpen();
            chumon = CreateChumonInstance(_context);
        }

        public async Task<IList<ChumonJisseki>> Test() {
            IList<ChumonJisseki> listChumonJissekis = await _context.ChumonJisseki.Include(i => i.ChumonJissekiMeisais).ToListAsync();
            return listChumonJissekis;
        }

        public async Task<ChumonViewModel> ChumonSetting(string inShiireSakiId, DateOnly inChumonDate) {
            /*
             * 注文セッティング
             * 引数：仕入先コード、注文日付
             * 戻り値：注文viewモデル
             */
            
            //注文実績モデル変数定義
            ChumonJisseki chumonJisseki;
            //もし、引数の注文日付がない場合（画面入力の注文日付が入力なしだと、1年1月1日になる
            if (DateOnly.FromDateTime(new DateTime(1, 1, 1)) == inChumonDate) {
                chumonJisseki = await chumon.ChumonSakusei(inShiireSakiId, DateOnly.FromDateTime(DateTime.Now));   //注文日付が指定なし→注文作成
            }
            else {
                //注文日付指定あり→注文問い合わせ
                chumonJisseki = await chumon.ChumonToiawase(inShiireSakiId, inChumonDate);

                if (chumonJisseki == null) {
                    //注文問い合わせでデータがない場合は、注文作成
                    chumonJisseki = await chumon.ChumonSakusei(inShiireSakiId, inChumonDate);
                }
            }
            //注文モデルを設定し戻り値とする
            return (new ChumonViewModel() {
                ChumonJisseki = chumonJisseki   //初期表示用の注文実績データ
            });
        }

        public async Task<(ChumonJisseki, int, bool, ErrDef)> ChumonCommit(ChumonJisseki inChumonJisseki) {

            var chumonJisseki = await chumon.ChumonUpdate(inChumonJisseki);

            (bool IsValid, ErrDef errCd) = ChumonJissekiIsValid(chumonJisseki);

            if (IsValid) {

                var entities = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity).Count();

                try {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException ex) {
                    throw new Exception(ex.Message);
                }
                chumonJisseki = await chumon.ChumonToiawase(inChumonJisseki.ShiireSakiId, inChumonJisseki.ChumonDate);
                return (chumonJisseki, entities, IsValid, ErrDef.NormalUpdate);
                }
            else {
                return (chumonJisseki, 0, IsValid, errCd);
            }
        }

        private (bool, ErrDef) ChumonJissekiIsValid(ChumonJisseki inChumonJisseki) {
            var chumonId = inChumonJisseki.ChumonId;
            var chumonDate = inChumonJisseki.ChumonDate;

            if (!Regex.IsMatch(chumonId, "^[0-9]{8}-[0-9]{3}$")) {
                return (false, ErrDef.ChumonIdError);
            }
            else if (chumonDate == null || chumonDate <= (new DateOnly(1, 1, 1))) {
                return (false, ErrDef.ChumonDateError);
            }

            foreach (var i in inChumonJisseki.ChumonJissekiMeisais) {
                if (i.ChumonId != chumonId) {
                    return (false, ErrDef.ChumonIdRelationError);
                }
                else if (i.ChumonSu == null) {
                    return (false, ErrDef.ChumonSuIsNull);
                }
                else if (i.ChumonSu < 0) {
                    return (false, ErrDef.ChumonSuBadRange);
                }
                else if (i.ChumonZan == null) {
                    return (false, ErrDef.ChumonZanIsNull);
                }
                else if (i.ChumonSu < i.ChumonZan) {
                    return (false, ErrDef.SuErrorBetChumonSuAndZan);
                }
            }
            return (true, ErrDef.DataValid);
        }

    }
}