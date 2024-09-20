using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Properties {

    /// <summary>
    /// * 注文クラス
    /// </summary>
    public class Chumon : IChumon, IDbContext {

        /// <summary>
        /// 注文実績プロパティ
        /// </summary>
        public ChumonJisseki ChumonJisseki { get; set; }

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
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
        /// <para>①仕入先より注文実績データ（親）を生成する</para>
        /// <para>②注文実績明細データ（子）を仕入マスタを元に作成する</para>
        /// <para>③注文実績データ（親）と注文実績明細データ（子）を連結する</para>
        /// <para>④注文実績（プラス注文実績明細）を戻り値とする</para>
        /// </remarks>
        /// <param name="inShireSakiId">仕入先コード</param>
        /// <param name="inChumonDate">注文日</param>
        /// <returns>新規作成された注文実績</returns>
        /// <exception cref="Exception"></exception>
        public async Task<ChumonJisseki> ChumonSakusei(string inShireSakiId, DateOnly inChumonDate) {

            //引数チェック（仕入先コード有無）
            if (await _context.ShiireSakiMaster.FindAsync(inShireSakiId) == null) {
                throw new Exception("仕入先ＩＤエラー");
            }
            //仕入先より注文実績データ（親）を生成する(a)

            ChumonJisseki = new ChumonJisseki {
                ChumonId = await ChumonIdHatsuban(inChumonDate),              //注文コード発番
                ShiireSakiId = inShireSakiId,                       //仕入先コード（引数より）
                ChumonDate = inChumonDate                           //注文日付
            };

            //注文実績明細データ（子）を作るために仕入マスタを読み込む(b)
            var shiireMasters = _context.ShireMaster
                .Where(shiire => shiire.ShiireSakiId == inShireSakiId)
                .Include(shiire => shiire.ShiireSakiMaster)
                .Include(shiire => shiire.ShohinMaster)
                .OrderBy(shiire => shiire.ShohinId);


            ChumonJisseki.ChumonJissekiMeisais = new List<ChumonJissekiMeisai>() { };

            //(b)のデータから注文実績明細を作成する
            foreach (var shiire in await shiireMasters.ToListAsync()) {

                if (shiire == null || shiire.ShohinMaster == null) continue;

                shiire.ShohinMaster.ShiireMasters = null;

                var meisai = new ChumonJissekiMeisai {
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
        /// <para>①注文実績＋注文実績＋仕入マスタ＋商品マスタ検索</para>
        /// <para>②戻り値を注文実績＋注文実績明細とする</para>
        /// </remarks>
        /// <param name="inShireSakiId">仕入先コード</param>
        /// <param name="inChumonDate">注文日</param>
        /// <returns>既存の注文実績</returns>
        public async Task<ChumonJisseki?> ChumonToiawase(string inShireSakiId, DateOnly inChumonDate) {
            //①注文実績＋注文実績＋仕入マスタ＋商品マスタ検索

            var chumonJisseki = await _context.ChumonJisseki
                        .Where(c => c.ShiireSakiId == inShireSakiId && c.ChumonDate == inChumonDate)
                        .Include(cm => cm.ChumonJissekiMeisais)
                        .ThenInclude(shi => shi.ShiireMaster)
                        .ThenInclude(sho => sho.ShohinMaster)
                        .FirstOrDefaultAsync();

            //②戻り値を注文実績＋注文実績明細とする
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
        private async Task<string> ChumonIdHatsuban(DateOnly InTheDate) {
            uint seqNumber;
            string dateArea;
            //今日の日付
            dateArea = InTheDate.ToString("yyyyMMdd");

            //今日の日付からすでに今日の分の注文コードがないか調べる
            var chumonid = await _context.ChumonJisseki
                .Where(x => x.ChumonId.StartsWith(dateArea))
                .MaxAsync(x => x.ChumonId);

            // 上記以外の場合、 //注文コードの右３桁の数値を求め＋１にする
            seqNumber = string.IsNullOrEmpty(chumonid) ? 1 //今日、注文コード起こすのが初めての場合
                      : uint.Parse(chumonid.Substring(9, 3)) + 1;

            ////３桁の数値が999以内（ＯＫ） それを超過するとnull

            return seqNumber <= 999 ? $"{dateArea}-{seqNumber:000}" : null;  // 999以上はNULLセット
        }

        /// <summary>
        /// 注文実績＋注文明細更新
        /// </summary>
        /// <param name="postedChumonJisseki">postされた注文実績</param>
        /// <returns>postされた注文実績を上書きされた注文実績</returns>
        public async Task<ChumonJisseki> ChumonUpdate(ChumonJisseki postedChumonJisseki) {

            ChumonJisseki? existedChumonJisseki; //DBにすでに登録されている場合の移送先

            //注文実績を読む
            existedChumonJisseki = await _context.ChumonJisseki.AsNoTracking()
                .Include(e => e.ChumonJissekiMeisais.OrderBy(x => x.ShiirePrdId))
                .FirstOrDefaultAsync(e => e.ChumonId == postedChumonJisseki.ChumonId);

            if (existedChumonJisseki != null) {  //注文実績がある場合

                //引数で渡された注文実績データを現プロパティに反映する
                var config = new MapperConfiguration(cfg => {
                    cfg.AddCollectionMappers();
                    cfg.CreateMap<ChumonJisseki, ChumonJisseki>()
                    .EqualityComparison((odto, o) => odto.ChumonId == o.ChumonId);
                    cfg.CreateMap<ChumonJissekiMeisai, ChumonJissekiMeisai>()
                    .EqualityComparison((odto, o) => odto.ChumonId == o.ChumonId && odto.ShiireSakiId == o.ShiireSakiId && odto.ShiirePrdId == o.ShiirePrdId && odto.ShohinId == o.ShohinId)
                    .BeforeMap((src, dest) => src.LastChumonSu = dest.ChumonSu)
                    .ForMember(dest => dest.ChumonZan, opt => opt.MapFrom(src => src.ChumonZan + src.ChumonSu - src.LastChumonSu))
                    .ForMember(dest => dest.ChumonJisseki, opt => opt.Ignore());
                });
                //引数で渡された注文実績をDBから読み込んだ注文実績に上書きする
                var mapper = new Mapper(config);
                mapper.Map(postedChumonJisseki, existedChumonJisseki);

                _context.Update(existedChumonJisseki);
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
    }
}