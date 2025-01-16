using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.Chumon;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static Convenience.Models.Properties.Config.Message;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Convenience.Models.Services {
    /// <summary>
    /// 注文サービスクラス
    /// </summary>
    public partial class ChumonService : IChumonService {

        /// <summary>
        /// 注文オブジェクト用
        /// </summary>
        public IChumon chumon { get; set; }

        /// <summary>
        /// コンストラクタ 注文オブジェクト用記述
        /// </summary>
        private readonly Func<ConvenienceContext, IChumon> CreateChumonInstance = context => new Chumon(context);

        /// <summary>
        /// 注文キービューモデル（１枚目の画面用）
        /// </summary>
        public ChumonKeysViewModel ChumonKeysViewModel { get; set; } = new ChumonKeysViewModel();

        /// <summary>
        /// 注文明細ビューモデル（２枚目の画面用） 
        /// </summary>
        public ChumonViewModel ChumonViewModel { get; set; } = new ChumonViewModel(); 

        /// <summary>
        /// コンストラクター　通常用
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumon">注文クラスＤＩ注入用</param>
        public ChumonService(IChumon chumon) {
            this.chumon = chumon;
            //chumon = CreateChumonInstance(_context);
        }
        /// <summary>
        /// デバッグ用
        /// </summary>
        public ChumonService() {

            chumon = CreateChumonInstance(((IDbContext)this).DbOpen());
        }

        /// <summary>
        /// 注文キービューモデル初期設定
        /// </summary>
        /// <returns>ChumonKeysViewModel 注文キービューモデル</returns>
        public async Task<ChumonKeysViewModel> SetChumonKeysViewModel() {
            var list = await chumon.ShiireSakiList(s => s.ShiireSakiId)
                .Select(s => new SelectListItem { Value = s.ShiireSakiId, Text = s.ShiireSakiId + " " + s.ShiireSakiKaisya }).ToListAsync();

            return ChumonKeysViewModel = new ChumonKeysViewModel() {
                ShiireSakiId = null,
                ChumonDate = DateOnly.FromDateTime(DateTime.Today),
                ShiireSakiList = list
            };
        }

        /// <summary>
        /// 注文セッティング
        /// </summary>
        /// <param name="inChumonKeysViewModel">注文キー入力画面仕入先コード（画面より）</param>
        /// <returns>注文明細ビューモデル</returns>
        public async Task<ChumonViewModel> ChumonSetting(ChumonKeysViewModel inChumonKeysViewModel) {

            //仕入先コード抽出
            string shiireSakiId = inChumonKeysViewModel?.ShiireSakiId??throw new ArgumentException("仕入先がセットされていません");
            //注文日付抽出
            DateOnly chumonDate = 
                inChumonKeysViewModel.ChumonDate == DateOnly.FromDateTime(new DateTime(1, 1, 1))
                    ?DateOnly.FromDateTime(DateTime.Now)
                    :inChumonKeysViewModel.ChumonDate;

            //注文実績モデル変数定義
            ChumonJisseki? createdChumonJisseki = default, existedChumonJisseki = default;
            //もし、引数の注文日付がない場合（画面入力の注文日付が入力なしだと、1年1月1日になる
            if (DateOnly.FromDateTime(new DateTime(1, 1, 1)) == chumonDate) {
                //注文作成
                createdChumonJisseki = 
                    await chumon.ChumonSakusei(shiireSakiId, DateOnly.FromDateTime(DateTime.Now));   //注文日付が指定なし→注文作成
            }
            else {
                //注文日付指定あり→注文問い合わせ
                existedChumonJisseki = await chumon.ChumonToiawase(shiireSakiId, chumonDate);

                if (existedChumonJisseki == null) {
                    //注文問い合わせでデータがない場合は、注文作成
                    createdChumonJisseki = await chumon.ChumonSakusei(shiireSakiId, chumonDate);
                }
            }

            //注文明細ビューモデルを設定し戻り値とする
            return ChumonViewModel = new ChumonViewModel() {
                ChumonJisseki = createdChumonJisseki 
                ?? existedChumonJisseki 
                ?? throw new ChumonJissekiSetupException("注文セッティングエラー")   //初期表示用の注文実績データ
            };
        }

        /// <summary>
        /// 注文データをDBに書き込む
        /// </summary>
        /// <param name="inChumonJisseki">Postされた注文実績</param>
        /// <returns>ChumonViewModel 注文明細ビューモデル</returns>
        /// <exception cref="Exception">排他制御の例外が起きたらスローする</exception>
        public async Task<ChumonViewModel> ChumonCommit(ChumonViewModel inChumonViewModel) {

            //注文実績抽出
            ChumonJisseki postedchumonJisseki = inChumonViewModel.ChumonJisseki;
            //Postされたデータで注文実績と注文実績明細の更新
            ChumonJisseki updatedChumonJisseki = await chumon.ChumonUpdate(postedchumonJisseki);

            //Postされた注文実績のデータチェック
            (bool IsValid, ErrDef errCd) = ChumonJissekiIsValid(updatedChumonJisseki);

            if (IsValid) {

                //DB更新

                int entities = await chumon.ChumonSaveChanges();

                //再表示用データセット
                updatedChumonJisseki = await chumon.ChumonToiawase(postedchumonJisseki.ShiireSakiId, postedchumonJisseki.ChumonDate)
                    ?? throw new NoDataFoundException("DB更新後のデータがありません");


                //注文ビューモデルセット(正常時）
                ChumonViewModel = new ChumonViewModel {
                    ChumonJisseki = updatedChumonJisseki,
                    IsNormal = IsValid,
                    Remark = errCd == ErrDef.DataValid && entities > 0 || errCd != ErrDef.DataValid 
                    ? new Message().SetMessage(ErrDef.NormalUpdate)?.MessageText 
                    : null
                };
            }
            else {
                //注文ビューモデルセット(チェックエラー時）
                ChumonViewModel = new ChumonViewModel {
                    ChumonJisseki = updatedChumonJisseki,
                    IsNormal = IsValid,
                    Remark = new Message().SetMessage(errCd)?.MessageText
                };
            }
            //注文明細ビューモデルを返却
            return ChumonViewModel;

        }

        /// <summary>
        /// Postされた注文実績のデータチェック
        /// </summary>
        /// <param name="inChumonJisseki">postされた注文実績</param>
        /// <returns>正常=true、異常=false、エラーコード</returns>
        private static (bool, ErrDef) ChumonJissekiIsValid(ChumonJisseki inChumonJisseki) {
            var chumonId = inChumonJisseki.ChumonId;
            var chumonDate = inChumonJisseki.ChumonDate;

            if (!ChumonRegex().IsMatch(chumonId ??string.Empty)) {
                return (false, ErrDef.ChumonIdError);
            }
            else if (chumonDate == DateOnly.MinValue || chumonDate <= new DateOnly(1, 1, 1)) {
                return (false, ErrDef.ChumonDateError);
            }

            if(inChumonJisseki?.ChumonJissekiMeisais is null) {
                return (false, ErrDef.NothingChumonJisseki);
            }

            foreach (var i in inChumonJisseki.ChumonJissekiMeisais) {
                if (i.ChumonId != chumonId) {
                    return (false, ErrDef.ChumonIdRelationError);
                }
                else if (i.ChumonSu < 0) {
                    return (false, ErrDef.ChumonSuBadRange);
                }
                else if (i.ChumonSu < i.ChumonZan) {
                    return (false, ErrDef.SuErrorBetChumonSuAndZan);
                }
            }
            return (true, ErrDef.DataValid);
        }

        [GeneratedRegex("^[0-9]{8}-[0-9]{3}$")]
        private static partial Regex ChumonRegex();
    }
}