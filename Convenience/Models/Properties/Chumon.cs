using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties.Config;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Convenience.Models.Properties {

    /// <summary>
    /// 注文クラス
    /// </summary>
    public class Chumon : IChumon,ISharedTools,IDisposable {

        /// <summary>
        /// 注文実績プロパティ
        /// </summary>
        public ChumonJisseki? ChumonJisseki { get; set; }

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// 自分でDbcontextを作ったかどうか
        /// </summary>
        private readonly bool _selfCreateContextFlg=false;

        /// <summary>
        /// Disposeを一回でも実施されたかどうか
        /// </summary>
        private bool _disposed = false;

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
            _selfCreateContextFlg = true;
        }

        /// <summary>
        ///デトラクタ（アンマネージドリソース開放用）
        /// </summary>
        ~Chumon() {
            Dispose(false);
        }

        /// <summary>
        /// ファイナライザ
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// ファイナライザ（オーバーライド可）
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if ((!_disposed) && _selfCreateContextFlg) {
                if (disposing) {
                    //マネージドリソース解放を書く
                    _context?.Dispose();
                }
                //アンマネージドリソース解放を書く

                //複数回実行しないように
                _disposed = true;
            }
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

            //引数チェック
            
            if (IsNotExistCheck(inShireSakiId)) {
                throw new ArgumentException("仕入先コード引数エラー");
            } else if(inChumonDate <= new DateOnly(1, 1, 1)) {
                throw new ArgumentException("注文日引数エラー");
            }

            //仕入先より注文実績データ（親）を生成する(a)

            string chumonId = await ChumonIdHatsuban(inChumonDate) ?? throw new OrderCodeGenerationException("注文コード発番時");

            if (IsExistCheck(chumonId)) {
                ChumonJisseki = new ChumonJisseki {
                    ChumonId = chumonId,                                //注文コード発番
                    ShiireSakiId = inShireSakiId,                       //仕入先コード（引数より）
                    ChumonDate = inChumonDate                           //注文日付
                };
            }
            else {
                throw new ArgumentException("注文コード発番エラー");
            }

            //注文実績明細データ（子）を作るために仕入マスタを読み込む(b)

            IEnumerable<ShiireMaster> shiireMasters = await _context.ShiireMaster.AsNoTracking()
                .Where(s => s.ShiireSakiId == inShireSakiId)
                .Include(s => s.ShiireSakiMaster)
                .Include(s => s.ShohinMaster)
                .OrderBy(s => s.ShiirePrdId).ToListAsync();

            if (!shiireMasters.Any()) {   //仕入マスタがない場合は例外
                throw new NoDataFoundException(nameof(ShiireMaster));
            }

            //商品マスタチェック
            if (shiireMasters!.Any(sm => IsExistCheck(sm.ShohinMaster))) {

                //(b)のデータから注文実績明細を作成する

                IMapper mapper = new MapperConfiguration(cfg => {
                    cfg.AddProfile(new ChumonCreateChumonJissekiToDTOAutoMapperProfile());
                }).CreateMapper();

                ChumonJisseki.ChumonJissekiMeisais  //注文実績明細
                    = mapper.Map<IEnumerable<ShiireMaster>, IList<ChumonJissekiMeisai>>(shiireMasters, opt => {
                        opt.Items["ChumonId"] = ChumonJisseki.ChumonId;
                        opt.Items["ShiireSakiId"] = ChumonJisseki.ShiireSakiId;
                    });
            }
            else {
                //商品マスタがないと以降の処理で困るから例外ではじく
                throw new NoDataFoundException(nameof(ShohinMaster));
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

        public async Task<ChumonJisseki?> ChumonToiawase(string inShireSakiId, DateOnly inChumonDate, bool includeShiireAndShohinMaster = true) {
            /*
             * ①注文実績＋注文実績明細＋仕入マスタ＋商品マスタ検索 
             */
            //注文実績＋注文実績明細(Trackingモードで取得）
            ChumonJisseki? chumonJisseki = await _context.ChumonJisseki
                .Where(c => c.ShiireSakiId == inShireSakiId && c.ChumonDate == inChumonDate)
                .Include(cj => cj.ChumonJissekiMeisais)
                .FirstOrDefaultAsync();

            var yyy = _context.ChangeTracker.Entries();

            //注文実績＋注文実績明細にプラスして、仕入マスタ＋商品マスタ
            // 仕入マスタと商品マスタを NoTracking モードで取得
            if (IsExistCheck(chumonJisseki)) {
                if (IsExistCheck(chumonJisseki!.ChumonJissekiMeisais)) {
                    foreach (ChumonJissekiMeisai meisai in chumonJisseki.ChumonJissekiMeisais!) {
                        ShiireMaster? shiireMaster = await _context.ShiireMaster
                            .AsNoTracking()
                            .Where(sm => sm.ShiireSakiId == meisai.ShiireSakiId &&
                                    sm.ShiirePrdId == meisai.ShiirePrdId &&
                                    sm.ShohinId == meisai.ShohinId)
                            .Include(sm => sm.ShohinMaster)
                            .FirstOrDefaultAsync();

                        // 明示的に ShiireMaster を関連付け（データがなければ null が入る）

                        _context.Entry(meisai).State = EntityState.Detached;
                        meisai.ShiireMaster = shiireMaster;                     //仕入＋商品マスタをセット、NotrackingなのでDetach状態でセット
                        _context.Entry(meisai).State = EntityState.Unchanged;
                    }
                }
            }

            var xxx = _context.ChangeTracker.Entries();

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

            string? chumonid = await _context.ChumonJisseki
                .Where(x => x.ChumonId!.StartsWith(dateArea))
                .MaxAsync(x => x.ChumonId);

            // 上記以外の場合、 //注文コードの右３桁の数値を求め＋１にする

            seqNumber = string.IsNullOrEmpty(chumonid) ? 1      //今日、注文コード起こすのが初めての場合
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
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers();
                cfg.AddProfile(new ChumonChumonJissekiToDTOAutoMapperProfile());
            }).CreateMapper();

            mapper!.Map<ChumonJisseki, ChumonJisseki>(postedChumonJisseki, existedChumonJisseki);
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

            _ = postedChumonJisseki?.ChumonJissekiMeisais ?? throw new ArgumentException("注文実績Postデータエラー");
            _ = existedChumonJisseki?.ChumonJissekiMeisais ?? throw new ArgumentException("注文実績ベースデータエラー");

            foreach (ChumonJissekiMeisai posted in postedChumonJisseki.ChumonJissekiMeisais) {
                ChumonJissekiMeisai target = existedChumonJisseki.ChumonJissekiMeisais
                    .Where(x => x.ChumonId == posted.ChumonId &&
                                x.ShiireSakiId == posted.ShiireSakiId &&
                                x.ShiirePrdId == posted.ShiirePrdId)
                    .Single();

                decimal lastChumonSu = target.ChumonSu;
                target.ChumonZan += posted.ChumonSu - target.ChumonSu;
                target.ChumonSu = posted.ChumonSu;
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
            IList<ChumonJissekiMeisai> postedMeisais =
                postedChumonJisseki?.ChumonJissekiMeisais ?? throw new ArgumentException("注文実績Postデータエラー");
            IList<ChumonJissekiMeisai> existedMeisais =
                existedChumonJisseki?.ChumonJissekiMeisais ?? throw new ArgumentException("注文実績ベースデータエラー");

            if (postedMeisais.Count != existedMeisais.Count)
                throw new DataCountMismatchException("注文明細画面");

            for (int i = 0; i < postedMeisais.Count; i++) {
                ChumonJissekiMeisai src = postedMeisais[i];
                ChumonJissekiMeisai dest = existedMeisais[i];

                if ((src.ChumonId, src.ShiireSakiId, src.ShiirePrdId) !=
                    (dest.ChumonId, dest.ShiireSakiId, dest.ShiirePrdId)) {
                    throw new DataPositionMismatchException("注文明細画面");
                }

                decimal lastChumonSu = dest.ChumonSu;
                dest.ChumonSu = src.ChumonSu;
                dest.ChumonZan = src.ChumonZan + src.ChumonSu - lastChumonSu;
            }

            return existedChumonJisseki;
        }

        /// <summary>
        /// PostデータをDTOに反映するときに、AutoMappewrなのか、手作りなのか、インデックス使った方法なのか切り分け用
        /// </summary>
        /// <param name="postedChumonJisseki">Postデータ</param>
        /// <param name="existedChumonJisseki">DTO</param>
        /// <returns></returns>
        private delegate ChumonJisseki DelegateOverrideProc(ChumonJisseki postedChumonJisseki, ChumonJisseki existedChumonJisseki);
        private readonly DelegateOverrideProc OverrideProc = ChumonUpdateWithAutoMapper;
        //private readonly DelegateOverrideProc OverrideProc = ChumonUpdateWithHandMade;
        //private readonly DelegateOverrideProc OverrideProc = ChumonUpdateWithIndex;

        /// <summary>
        /// 注文実績＋注文明細更新
        /// </summary>
        /// <param name="postedChumonJisseki">postされた注文実績</param>
        /// <returns>postされた注文実績を上書きされた注文実績</returns>
        /// 

        /// <summary>
        /// 注文実績＋注文明細更新
        /// </summary>
        /// <param name="postedChumonJisseki">postされた注文実績</param
        /// <param name="existedChumonJisseki">DBに登録された注文実績(null可)</param>
        /// <returns>postされた注文実績を上書きされた注文実績</returns>
        public async Task<ChumonJisseki> ChumonUpdate(ChumonJisseki postedChumonJisseki, ChumonJisseki? existedChumonJisseki=null) {

            _ = postedChumonJisseki?.ChumonJissekiMeisais ?? throw new ArgumentException("注文実績Postデータエラー");
            _ = postedChumonJisseki?.ChumonId ?? throw new ArgumentException("注文実績Postデータの注文コードエラー");

            if (IsExistCheck(existedChumonJisseki)) {
                //注文実績がある場合
                //AutoMapper利用か、ハンドメイドなのか選択されている
                var a = _context.ChangeTracker.Entries();
                existedChumonJisseki = OverrideProc(postedChumonJisseki, existedChumonJisseki!);
                var b = _context.ChangeTracker.Entries();
                ChumonJisseki = existedChumonJisseki;
            }
            else {
                //注文実績がない場合
                //注文実績がない場合、引数で渡された注文実績をDBにレコード追加する
                foreach (ChumonJissekiMeisai item in postedChumonJisseki.ChumonJissekiMeisais) {
                    item.ChumonZan = item.ChumonSu;
                }
                await _context.ChumonJisseki.AddAsync(postedChumonJisseki);
                ChumonJisseki = postedChumonJisseki;
            }
            return ChumonJisseki;
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <returns></returns>
        /// <remarks>注文実績＋注文実績明細を更新する</remarks>
        /// <exception cref="DbUpdateConcurrencyException"></exception>
        public async Task<int> ChumonSaveChanges() {

            var xxx = _context.ChangeTracker.Entries();

            //更新対象のエンティティ数を求める
            int entities = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity).Count();

            try {
                //DB更新
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException ex) {
                //排他制御エラー
                throw new DbUpdateConcurrencyException(ex.Message);
            }

            return entities;
        }

        static private bool IsExistCheck<T>(T? checkdata) {
            if (checkdata == null) {
                return false; // null の場合は false を返す
            }

            // T が IEnumerable かどうかを確認
            if (checkdata is IEnumerable<object> enumerable) {
                return enumerable.Any(); // リストの場合は要素があるかどうかを確認
            }

            return true; // リストでない場合は true を返す（存在している）
        }
        static private bool IsNotExistCheck<T>(T? checkdata) {
            return !IsExistCheck(checkdata); // リストでない場合は true を返す（存在していない）
        }

        /// <summary>
        /// 注文可能な仕入先一覧を作成する
        /// </summary>
        /// <returns>IEnumerable<ShiireSakiMaster>注文可能な仕入先マスタのリスト（実行前）</returns>
        //public IQueryable<ShiireSakiMaster> ShiireSakiListold<T>(Expression<Func<ShiireSakiMaster, T>> orderExpression) =>
        //        IsExistCheck(orderExpression) ? _context.ShiireSakiMaster.OrderBy(orderExpression) : _context.ShiireSakiMaster;

        public IQueryable<ShiireSakiMaster> ShiireSakiList<T>(Expression<Func<ShiireSakiMaster, T>> orderExpression) => 
            new SelectList(_context).GenerateList(orderExpression);

    }
}

