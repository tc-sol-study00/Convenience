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
    /// <summary>
    /// 注文サービスクラス
    /// </summary>
    public class ChumonService : IChumonService, IDbContext {

        /// <summary>
        /// DBコンテクスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// 注文オブジェクト用
        /// </summary>
        public IChumon chumon { get; set; }

        /// <summary>
        /// コンストラクタ 注文オブジェクト用記述
        /// </summary>
        private Func<ConvenienceContext, IChumon> CreateChumonInstance = context => new Chumon(context);

        /// <summary>
        /// コンストラクター　通常用
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        public ChumonService(ConvenienceContext context) {
            _context = context;
            chumon = CreateChumonInstance(_context);
        }
        /// <summary>
        /// デバッグ用
        /// </summary>
        public ChumonService() {
            _context = IDbContext.DbOpen();
            chumon = CreateChumonInstance(_context);
        }

        /// <summary>
        /// 注文セッティング
        /// </summary>
        /// <param name="inShiireSakiId">仕入先コード（画面より）</param>
        /// <param name="inChumonDate">注文日付（画面より）</param>
        /// <returns>注文viewモデル</returns>
        public async Task<ChumonViewModel> ChumonSetting(string inShiireSakiId, DateOnly inChumonDate) {
            
            //注文実績モデル変数定義
            ChumonJisseki createdChumonJisseki=default, existedChumonJisseki= default;
            //もし、引数の注文日付がない場合（画面入力の注文日付が入力なしだと、1年1月1日になる
            if (DateOnly.FromDateTime(new DateTime(1, 1, 1)) == inChumonDate) {
                //注文作成
                createdChumonJisseki = await chumon.ChumonSakusei(inShiireSakiId, DateOnly.FromDateTime(DateTime.Now));   //注文日付が指定なし→注文作成
            }
            else {
                //注文日付指定あり→注文問い合わせ
                existedChumonJisseki = await chumon.ChumonToiawase(inShiireSakiId, inChumonDate);

                if (existedChumonJisseki == null) {
                    //注文問い合わせでデータがない場合は、注文作成
                    createdChumonJisseki = await chumon.ChumonSakusei(inShiireSakiId, inChumonDate);
                }
            }

            //注文モデルを設定し戻り値とする
            return (new ChumonViewModel() {
                ChumonJisseki = createdChumonJisseki??existedChumonJisseki??throw new Exception("注文セッティングエラー")   //初期表示用の注文実績データ
            });
        }

        /// <summary>
        /// 注文データをDBに書き込む
        /// </summary>
        /// <param name="inChumonJisseki">Postされた注文実績</param>
        /// <returns></returns>
        /// <exception cref="Exception">排他制御の例外が起きたらスローする</exception>
        public async Task<(ChumonJisseki, int, bool, ErrDef)> ChumonCommit(ChumonJisseki inChumonJisseki) {

            //Postされたデータで注文実績と注文実績明細の更新
            var updatedChumonJisseki = await chumon.ChumonUpdate(inChumonJisseki);

            (bool IsValid, ErrDef errCd) = ChumonJissekiIsValid(updatedChumonJisseki);

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
                updatedChumonJisseki = await chumon.ChumonToiawase(inChumonJisseki.ShiireSakiId, inChumonJisseki.ChumonDate);
                return (updatedChumonJisseki, entities, IsValid, ErrDef.NormalUpdate);
                }
            else {
                return (updatedChumonJisseki, 0, IsValid, errCd);
            }
        }

        /// <summary>
        /// Postされた注文実績のデータチェック
        /// </summary>
        /// <param name="inChumonJisseki">postされた注文実績</param>
        /// <returns>正常=true、異常=false、エラーコード</returns>
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