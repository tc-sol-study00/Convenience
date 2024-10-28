using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Convenience.Models.Properties {

    /// <summary>
    /// * 注文クラス
    /// </summary>
    public class Chumon : IChumon, IDbContext {

        /// <summary>
        /// 注文実績プロパティ
        /// </summary>
        public ChumonJisseki? ChumonJisseki { get; set; }

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// 
        public Chumon(ConvenienceContext context) {
            _context = context;
        }

        /// <summary>
        /// 注文クラスデバッグ用
        /// </summary>
        public Chumon() {
            _context = IDbContext.DbOpen();
        }

        /// <summary>
        /// 注文作成
        /// </summary>
        /// <remarks>
        /// 仕入先より注文実績データ（親）を生成する
        /// 注文実績明細データ（子）を仕入マスタを元に作成する
        /// 注文実績データ（親）と注文実績明細データ（子）を連結する
        /// 注文実績（プラス注文実績明細）を戻り値とする
        /// </remarks>
        /// <param name="inShireSakiId">仕入先コード</param>
        /// <param name="inChumonDate">注文日</param>
        /// <returns>新規作成された注文実績</returns>
        /// <exception cref="Exception"></exception>
        public async Task<ChumonJisseki> ChumonSakusei(string inShireSakiId, DateOnly inChumonDate) {

            //仕入先より注文実績データ（親）を生成する(a)

            Task<string?> taskChumonId = ChumonIdHatsuban(inChumonDate);      //非同期実行（あまり意味ないけど）

            ChumonJisseki = new ChumonJisseki {
                ChumonId = null,                                    //注文コード発番（非同期終わるまでnull)
                ShiireSakiId = inShireSakiId,                       //仕入先コード（引数より）
                ChumonDate = inChumonDate                           //注文日付
            };

            //注文実績明細データ（子）を作るために仕入マスタを読み込む(b)
            IEnumerable<ShiireMaster> shiireMasters = await _context.ShiireMaster.AsNoTracking()
                .Where(s => s.ShiireSakiId == inShireSakiId)
                .Include(s => s.ShiireSakiMaster)
                .Include(s => s.ShohinMaster)
                .OrderBy(s => s.ShiirePrdId).ToListAsync();

            if (!shiireMasters.Any()) {   //仕入マスタがない場合は例外
                throw new NoDataFoundException(nameof(ShiireMaster));
            }

            ChumonJisseki.ChumonJissekiMeisais = new List<ChumonJissekiMeisai>();

            //注文コード発番の結果を得る
            var chumonId = await taskChumonId ?? throw new OrderCodeGenerationException("注文コード発番でnullが設定されています");                      //非同期処理を待つ
            ChumonJisseki.ChumonId = chumonId;                      //問題なければ発番された注文コードをプロパティにセット

            //(b)のデータから注文実績明細を作成する
            foreach (ShiireMaster shiire in shiireMasters) {

                if (shiire == null || shiire.ShohinMaster == null) continue;

                //エンティティ連結ループ対策だったが、上記AsNoTracking対応により不要となった
                //shiire.ShohinMaster.ShiireMasters = null; 

                ChumonJissekiMeisai meisai = new (){
                    ChumonId = ChumonJisseki.ChumonId,
                    ShiireSakiId = ChumonJisseki.ShiireSakiId,  //仕入先コードを注文実績からセット(aより)
                    ShiirePrdId = shiire.ShiirePrdId,           //仕入商品コードのセット(bより）
                    ShohinId = shiire.ShohinId,                 //仕入マスタから商品コード（bより）
                    ChumonSu = 0,                               //初期値として注文数０をセット
                    ChumonZan = 0,                              //初期値として注文残０をセット
                    ShiireMaster = shiire                       //仕入マスタに対するリレーション情報のセット
                };
                ChumonJisseki.ChumonJissekiMeisais.Add(meisai);
            }

            //注文実績（プラス注文実績明細）を戻り値とする
            return ChumonJisseki;
        }

        /// <summary>
        /// 注文更新用問い合わせ
        /// </summary>
        /// <remarks>
        /// <para>①注文実績＋注文実績明細＋仕入マスタ＋商品マスタ検索</para>
        /// <para>②戻り値を注文実績＋注文実績明細とする</para>
        /// </remarks>
        /// <param name="inShireSakiId">仕入先コード</param>
        /// <param name="inChumonDate">注文日</param>
        /// <returns>既存の注文実績</returns>
        public async Task<ChumonJisseki?> ChumonToiawase(string inShireSakiId, DateOnly inChumonDate) {
            /*
             * ①注文実績＋注文実績明細＋仕入マスタ＋商品マスタ検索 
             */

            //注文実績＋注文実績明細
            ChumonJisseki? chumonJisseki = await _context.ChumonJisseki
                        .Where(c => c.ShiireSakiId == inShireSakiId && c.ChumonDate == inChumonDate)
                        .Include(cm => cm.ChumonJissekiMeisais)
                        .FirstOrDefaultAsync();

            //注文実績＋注文実績明細にプラスして、仕入マスタ＋商品マスタ
            if (chumonJisseki != null) {
                if (chumonJisseki.ChumonJissekiMeisais != null) {
                    // ShiireMaster と ShohinMaster を AsNoTracking() で取得
                    foreach(var meisai in chumonJisseki.ChumonJissekiMeisais){
                        var shiireMaster = await _context.ShiireMaster
                            .AsNoTracking()
                            .Where(sm => sm.ShiireSakiId == meisai.ShiireSakiId && sm.ShiirePrdId == meisai.ShiirePrdId && sm.ShohinId == meisai.ShohinId)
                            .Include(sm => sm.ShohinMaster)
                            .FirstOrDefaultAsync();

                        // 明示的に ShiireMaster を関連付け（データがなければ null が入る）
                        meisai.ShiireMaster = shiireMaster;
                    }
                }
            }


            //②戻り値を注文実績＋注文実績明細とする
            //データがない場合はnullで返す
            ChumonJisseki = chumonJisseki;
            return (ChumonJisseki);
        }

        /// <summary>
        /// 注文コード発番
        /// </summary>
        /// <remarks>
        ///  注文コード書式例）：20240129-001(yyyyMMdd-001～999）
        /// </remarks>
        /// <param name="InTheDate">注文日付</param>
        /// <param name="_context">ＤＢコンテキスト</param>
        /// <returns>発番された注文コード</returns>
        private async Task<string?> ChumonIdHatsuban(DateOnly InTheDate) {
            uint seqNumber;
            string dateArea;
            //今日の日付
            dateArea = InTheDate.ToString("yyyyMMdd");

            //今日の日付からすでに今日の分の注文コードがないか調べる
            var chumonid = await _context.ChumonJisseki
                .Where(x => x.ChumonId!.StartsWith(dateArea))
                .MaxAsync(x => x.ChumonId);

            // 上記以外の場合、 //注文コードの右３桁の数値を求め＋１にする
            seqNumber = string.IsNullOrEmpty(chumonid) ? 1 //今日、注文コード起こすのが初めての場合
                      : uint.Parse(chumonid.Substring(9, 3)) + 1;

            ////３桁の数値が999以内（ＯＫ） それを超過するとnull

            return seqNumber <= 999 ? $"{dateArea}-{seqNumber:000}" : null;  // 999以上はNULLセット
        }

        /// <summary>
        /// Postデータの上乗せ処理（AutoMapper利用)
        /// </summary>
        /// <remarks>
        /// private readonly DelegateOverrideProc OverrideProc = ChumonUpdateWithAutoMapper;の設定の時にコールされる
        /// ３つの回答例の説明用の為、研修生は気にしなくて良い
        /// </remarks>
        /// <param name="postedChumonJisseki">注文実績＋明細のPostデータ</param>
        /// <param name="existedChumonJisseki">注文実績＋明細のDBデータ</param>
        /// <returns>上乗せされた注文実績＋明細データ</returns>
        private static ChumonJisseki ChumonUpdateWithAutoMapper(ChumonJisseki postedChumonJisseki, ChumonJisseki existedChumonJisseki) {
            //引数で渡された注文実績データを現プロパティに反映する
            var config = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers();
                cfg.AddProfile(new ChumonChumonJissekiToDTOAutoMapperProfile());
            });

            IMapper mapper = config.CreateMapper();
            
            mapper!.Map(postedChumonJisseki, existedChumonJisseki);

            return existedChumonJisseki;
        }

        /// <summary>
        /// Postデータの上乗せ処理（Linq使った手書き対応)
        /// </summary>
        /// <remarks>
        /// private readonly DelegateOverrideProc OverrideProc = ChumonUpdateWithHandMade;の設定の時にコールされる
        /// ３つの回答例の説明用の為、研修生は気にしなくて良い
        /// </remarks>
        /// <param name="postedChumonJisseki">注文実績＋明細のPostデータ</param>
        /// <param name="existedChumonJisseki">注文実績＋明細のDBデータ</param>
        /// <returns>上乗せされた注文実績＋明細データ</returns>
        private static ChumonJisseki ChumonUpdateWithHandMade(ChumonJisseki postedChumonJisseki, ChumonJisseki existedChumonJisseki) {

            _ = postedChumonJisseki?.ChumonJissekiMeisais??throw new ArgumentException("注文実績Postデータエラー");
            _ = existedChumonJisseki?.ChumonJissekiMeisais?? throw new ArgumentException("注文実績ベースデータエラー");
            foreach (ChumonJissekiMeisai postedChumonJissekiMeisai in postedChumonJisseki.ChumonJissekiMeisais) {
                ChumonJissekiMeisai targetChumonJissekiMeisai = existedChumonJisseki.ChumonJissekiMeisais
                    .Where(x => x.ChumonId == postedChumonJissekiMeisai.ChumonId &&
                                x.ShiireSakiId == postedChumonJissekiMeisai.ShiireSakiId &&
                                x.ShiirePrdId == postedChumonJissekiMeisai.ShiirePrdId)
                    .Single();

                var lastChumonSu = targetChumonJissekiMeisai.ChumonSu;
                targetChumonJissekiMeisai.ChumonSu = postedChumonJissekiMeisai.ChumonSu;
                targetChumonJissekiMeisai.ChumonZan = postedChumonJissekiMeisai.ChumonZan + postedChumonJissekiMeisai.ChumonSu - lastChumonSu;
            }
            return existedChumonJisseki;
        }

        /// <summary>
        /// Postデータの上乗せ処理（For+Index使った手書き対応)
        /// </summary>
        /// <remarks>
        /// private readonly DelegateOverrideProc OverrideProc = ChumonUpdateWithIndex;の設定の時にコールされる
        /// ３つの回答例の説明用の為、研修生は気にしなくて良い
        /// </remarks>
        /// <param name="postedChumonJisseki">注文実績＋明細のPostデータ</param>
        /// <param name="existedChumonJisseki">注文実績＋明細のDBデータ</param>
        /// <returns>上乗せされた注文実績＋明細データ</returns>
        private static ChumonJisseki ChumonUpdateWithIndex(ChumonJisseki postedChumonJisseki, ChumonJisseki existedChumonJisseki) {
            _ = postedChumonJisseki?.ChumonJissekiMeisais ?? throw new ArgumentException("注文実績Postデータエラー");
            _ = existedChumonJisseki?.ChumonJissekiMeisais ?? throw new ArgumentException("注文実績ベースデータエラー");

            for (int i = 0; i < postedChumonJisseki.ChumonJissekiMeisais.Count; i++) {
                if (i < existedChumonJisseki.ChumonJissekiMeisais.Count) {
                    ChumonJissekiMeisai src = postedChumonJisseki.ChumonJissekiMeisais[i];
                    ChumonJissekiMeisai dest = existedChumonJisseki.ChumonJissekiMeisais[i];

                    if ((src.ChumonId, src.ShiireSakiId, src.ShiirePrdId) == (dest.ChumonId, dest.ShiireSakiId, dest.ShiirePrdId)) {
                        var lastChumonSu = dest.ChumonSu;
                        dest.ChumonSu = src.ChumonSu;
                        dest.ChumonZan = src.ChumonZan + src.ChumonSu - lastChumonSu;
                    }
                    else throw new DataPositionMismatchException("PostデータエラーとDB側データの位置エラー(ソートされていない可能性）");
                }
                else throw new DataCountMismatchException("PostデータエラーとDB側データの件数アンマッチ");
            }
            return existedChumonJisseki;
        }

        private delegate ChumonJisseki DelegateOverrideProc(ChumonJisseki postedChumonJisseki, ChumonJisseki existedChumonJisseki);
        private readonly DelegateOverrideProc OverrideProc = ChumonUpdateWithAutoMapper;
        //private readonly DelegateOverrideProc OverrideProc = ChumonUpdateWithHandMade;
        //private readonly DelegateOverrideProc OverrideProc = ChumonUpdateWithIndex;

        /// <summary>
        /// 注文実績＋注文明細更新
        /// </summary>
        /// <param name="postedChumonJisseki">postされた注文実績</param>
        /// <returns>postされた注文実績を上書きされた注文実績</returns>
        public async Task<ChumonJisseki> ChumonUpdate(ChumonJisseki postedChumonJisseki) {

            _=postedChumonJisseki?.ChumonJissekiMeisais?? throw new ArgumentException("注文実績Postデータエラー");

            ChumonJisseki? existedChumonJisseki; //DBにすでに登録されている場合の移送先

            //注文実績を読む
            existedChumonJisseki = await _context.ChumonJisseki
                .Include(e => e.ChumonJissekiMeisais!.OrderBy(x => x.ShiirePrdId))
                .FirstOrDefaultAsync(e => e.ChumonId == postedChumonJisseki.ChumonId);

            if (existedChumonJisseki != null) {  //注文実績がある場合

                //AutoMapper利用か、ハンドメイドなのか選択されている
                existedChumonJisseki = OverrideProc(postedChumonJisseki, existedChumonJisseki);
                //_context.Update(existedChumonJisseki); トレースされているからUpdateしなくて良い
                ChumonJisseki = existedChumonJisseki;
            }
            else {   //注文実績がない場合、引数で渡された注文実績をDBにレコード追加する
                foreach (var item in postedChumonJisseki.ChumonJissekiMeisais) {
                    item.ChumonZan = item.ChumonSu;
                }
                await _context.ChumonJisseki.AddAsync(postedChumonJisseki);
                ChumonJisseki = postedChumonJisseki;
            }
            //注文実績＋注文実績明細を戻り値とする
            return ChumonJisseki;
        }

        /// <summary>
        /// 注文可能な仕入先一覧を作成する
        /// </summary>
        /// <returns>IEnumerable<ShiireSakiMaster>注文可能な仕入先マスタのリスト（実行前）</returns>
        public IQueryable<ShiireSakiMaster> ShiireSakiList<T>(Expression<Func<ShiireSakiMaster, T>> orderExpression) =>
            orderExpression != null ? _context.ShiireSakiMaster.OrderBy(orderExpression) : _context.ShiireSakiMaster;
    }
}