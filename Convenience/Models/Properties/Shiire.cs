using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Properties
{
    /// <summary>
    /// 仕入クラス
    /// </summary>
    public class Shiire : IShiire, IDbContext {

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// 仕入実績プロパティ
        /// Include ChuumonJissekiMeisai
        /// </summary>
        public IList<ShiireJisseki> Shiirejissekis { get; set; } 
        /// <summary>
        /// 倉庫在庫プロパティ
        /// </summary>
        public IList<SokoZaiko> SokoZaikos { get; set; }

        /// <summary>
        /// コンストラクタ
        /// 通常の場合はＤＢコンテキストを引き継ぐ
        /// </summary>
        /// <param name="context">ASPから引き継ぐDBコンテキスト</param>
        public Shiire(ConvenienceContext context) {
            _context = context;
        }
        /// <summary>
        /// 仕入クラスデバッグ用
        /// </summary>
        public Shiire() {
            _context = IDbContext.DbOpen();
        }

        /// <summary>
        /// 仕入データPost内容の反映 
        /// </summary>
        /// <param name="inShiireJissekis">Postされた仕入実績</param>
        /// <returns>Postされた注仕入実績がオーバライドされた仕入実績プロパティ</returns>
        public IList<ShiireJisseki> ShiireUpdate(IList<ShiireJisseki> inShiireJissekis) {
            // AutoMapperの初期設定
           
            var config = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers();
                cfg.CreateMap<ShiireJisseki, ShiireJisseki>()
                .EqualityComparison((odto, o) => odto.ShiireSakiId == o.ShiireSakiId && odto.ShiirePrdId == o.ShiirePrdId)
                .ForMember(dest => dest.ShiireDateTime, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.ShiireDateTime, DateTimeKind.Utc)))
                .ForMember(dest => dest.NonyuSu, opt => opt.MapFrom(src => src.NonyuSu))
                .BeforeMap((src, dest) => dest.NonyuSubalance = src.NonyuSu - dest.NonyuSu)
                .ForMember(dest => dest.NonyuSubalance, opt => opt.Ignore())
                .ForMember(dest => dest.ChumonJissekiMeisaii, opt => opt.Ignore())
                .AfterMap((src, dest) => _context.Entry(dest).Property(v => v.Version).OriginalValue = src.Version)
                .AfterMap((src, dest) => _context.Entry(dest.ChumonJissekiMeisaii).Property(v => v.Version).OriginalValue = src.ChumonJissekiMeisaii.Version);
            });

            var mapper = config.CreateMapper(); // AutoMapperのインスタンス作成
            mapper.Map<IList<ShiireJisseki>, IList<ShiireJisseki>>(inShiireJissekis, Shiirejissekis);

            return (Shiirejissekis);
        }

        /// <summary>
        /// 仕入実績に既に存在しているかチェック
        /// </summary>
        /// <param name="inChumonId">注文コード（仕入実績に対する検索キー）</param>
        /// <param name="inShiireDate">仕入日付（仕入実績に対する検索キー）</param>
        /// <param name="inSeqByShiireDate">仕入SEQ（仕入実績に対する検索キー）</param>
        /// <returns>データがあればtrueなければfalse/returns>
        public async Task<bool> ChuumonIdOnShiireJissekiExistingCheck(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate) {
            var result = await _context.ShiireJisseki
               .FirstOrDefaultAsync(
                   w => w.ChumonId == inChumonId
                   && w.ShiireDate == inShiireDate
                   && w.SeqByShiireDate == inSeqByShiireDate
                );

            return (result != null);
        }

        /// <summary>
        /// 注文残・倉庫在庫調整用モデル
        /// </summary>
        public class ShiireUkeireReturnSet {
            /// <summary>
            /// 仕入実績
            /// Include注文実績
            /// </summary>
            public IList<ShiireJisseki> ShiireJissekis { get; set; }
            /// <summary>
            /// 倉庫在庫
            /// </summary>
            public IList<SokoZaiko> SokoZaikos { get; set; }
        }

        /// <summary>
        /// 注文残・在庫数量調整
        /// </summary>
        /// <param name="inChumonId">注文コード</param>
        /// <param name="inShiireJissekis">仕入実績（注文実績がインクルードされていること）</param>
        /// <returns>注文残・倉庫在庫が調整された注文残・倉庫在庫調整用モデル</returns>
        public async Task<ShiireUkeireReturnSet> ChuumonZanZaikoSuChousei(string inChumonId, IList<ShiireJisseki> inShiireJissekis) {

            //注文残を設定・注文実績明細にセット

            foreach (var item in inShiireJissekis) {
                item.ChumonJissekiMeisaii.ChumonZan -= item.NonyuSubalance ?? 0;
                if (item.ChumonJissekiMeisaii.ChumonZan < 0) {
                    item.ChumonJissekiMeisaii.ChumonZan = 0;
                }
            }

            //仕入実績を元に倉庫在庫セット

            var sokoZaikos = await ZaikoSet(inShiireJissekis);

            return (new ShiireUkeireReturnSet {
                ShiireJissekis = inShiireJissekis,
                SokoZaikos = sokoZaikos
            });
        }
        /// <summary>
        /// 仕入実績から仕入実績プロパティに反映
        /// </summary>
        /// <param name="inChumonId">注文コード</param>
        /// <param name="inShiireDate">仕入日付</param>
        /// <param name="inSeqByShiireDate">仕入SEQ</param>
        /// <returns></returns>
        public async Task<IList<ShiireJisseki>> ShiireToShiireJisseki(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate) {
            //仕入実績のセット
            IList<ShiireJisseki> queriedShiireJissekis = await _context.ShiireJisseki
                .Where(c => c.ChumonId == inChumonId && c.ShiireDate == inShiireDate && c.SeqByShiireDate == inSeqByShiireDate)
                .Include(cjm => cjm.ChumonJissekiMeisaii)
                .ThenInclude(cj => cj.ChumonJisseki)
                .ThenInclude(ss => ss.ShiireSakiMaster)
                .Include(cjm => cjm.ChumonJissekiMeisaii)
                .ThenInclude(a => a.ShiireMaster)
                .ThenInclude(s => s.ShohinMaster)
                .ToListAsync();
            //プロパティに設定
            Shiirejissekis = queriedShiireJissekis;

            return (Shiirejissekis);
        }

        /// <summary>
        /// 仕入実績作成
        /// </summary>
        /// <param name="inChumonId">注文コード（注文実績問い合わせキー）</param>
        /// <param name="inShiireDate">仕入日付（仕入実績にセットされる）</param>
        /// <param name="inSeqByShiireDate">仕入日付内のシーケンス（仕入実績にセットされる）</param>
        /// <returns>注文実績から新規作成された仕入実績</returns>
        public async Task<IList<ShiireJisseki>> ChumonToShiireJisseki(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate) {

            //注文明細取得（キー：注文コード）複数のレコード
            IList<ChumonJissekiMeisai> queriedChumonJissekiMeisais = await _context.ChumonJissekiMeisai
                .Where(c => c.ChumonId == inChumonId)
                .Include(cj => cj.ChumonJisseki)
                .ThenInclude(ss => ss.ShiireSakiMaster)
                .Include(sm => sm.ShiireMaster)
                .ThenInclude(s => s.ShohinMaster)
                .ToListAsync();

            //現在時間
            DateTime nowTime = DateTime.Now;

            //注文明細 to 仕入実績（Ａ）　複数のレコード
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ChumonJissekiMeisai, ShiireJisseki>()
                    .ForMember(dest => dest.ChumonId, opt => opt.MapFrom(src => src.ChumonId))
                    .ForMember(dest => dest.ShiireDate, opt => opt.MapFrom(src => inShiireDate))
                    .ForMember(dest => dest.SeqByShiireDate, opt => opt.MapFrom(src => inSeqByShiireDate))
                    .ForMember(dest => dest.ShiireDateTime, opt => opt.MapFrom(src => DateTime.SpecifyKind(nowTime, DateTimeKind.Utc)))
                    .ForMember(dest => dest.ShiireSakiId, opt => opt.MapFrom(src => src.ShiireSakiId))
                    .ForMember(dest => dest.ShiirePrdId, opt => opt.MapFrom(src => src.ShiirePrdId))
                    .ForMember(dest => dest.ShohinId, opt => opt.MapFrom(src => src.ShohinId))
                    .ForMember(dest => dest.NonyuSu, opt => opt.MapFrom(src => 0))
                    //                .ForMember(dest => dest.NonyuSu, opt => opt.MapFrom(src => src.ChumonZan))  //注文残
                    .ForMember(dest => dest.ChumonJissekiMeisaii, opt => opt.MapFrom(src => src)); //ChuumonJissekiMeisai Set
            });

            //AutoMapperの処理
            var mapper = new Mapper(config);
            IList<ShiireJisseki> createdShiireJissekis = new List<ShiireJisseki>();
            mapper.Map<IList<ChumonJissekiMeisai>, IList<ShiireJisseki>>(queriedChumonJissekiMeisais, createdShiireJissekis);

            //仕入実績に対し、EFに新規（Add）の指示
            await _context.ShiireJisseki.AddRangeAsync(createdShiireJissekis);
            //仕入実績プロパティに反映
            Shiirejissekis = createdShiireJissekis;
            //注文実績から新規作成された仕入実績
            return (Shiirejissekis);
        }
        /// <summary>
        /// 倉庫在庫登録
        /// </summary>
        /// <param name="shiireJissekis">Postされたデータでオーバーライドされた仕入実績</param>
        /// <returns>仕入実績から仕入差を使って在庫数を調整された倉庫在庫</returns>

        private async Task<IList<SokoZaiko>> ZaikoSet(IEnumerable<ShiireJisseki> shiireJissekis) {

            //仕入実績より倉庫在庫主キー単位にレコードを起こす（倉庫在庫と粒度をあわせるため）
            //主キー   ：仕入先、仕入商品コード、商品コード
            //集計      :納品数（ケース）、実数量（納品ケース数×ケースあたりの数量）
            var shiireJissekiGrp = shiireJissekis
                .GroupBy(g => new {
                    g.ShiireSakiId,
                    g.ShiirePrdId,
                    g.ShohinId
                })
                .Select(gr => new {
                    ShireSakiId = gr.Key.ShiireSakiId,
                    ShiirePrdId = gr.Key.ShiirePrdId,
                    ShohinId = gr.Key.ShohinId,
                    NonyuSubalance = gr.Sum(i => i.NonyuSubalance),
                    SokoZaikoSu = gr.Sum(j => j.ChumonJissekiMeisaii.ShiireMaster.ShiirePcsPerUnit * j.NonyuSubalance ?? 0),
                    ShireDateTime = gr.Max(k => k.ShiireDateTime)
                }).ToList()
                ;

            //現在の倉庫在庫を読み込む準備

            IList<SokoZaiko> sokoZaikos = new List<SokoZaiko>();
            var shiireSakiIds = shiireJissekiGrp.Select(s => s.ShireSakiId).ToList();
            var shiirePrdIds = shiireJissekiGrp.Select(s => s.ShiirePrdId).ToList();
            var shohinIds = shiireJissekiGrp.Select(s => s.ShohinId).ToList();

            // 倉庫在庫を一括で取得
            sokoZaikos = await _context.SokoZaiko
                .Where(s => shiireSakiIds.Contains(s.ShiireSakiId) &&
                            shiirePrdIds.Contains(s.ShiirePrdId) &&
                            shohinIds.Contains(s.ShohinId))
                .ToListAsync();

            var result = shiireJissekiGrp.GroupJoin(
            sokoZaikos,
            sjg => new { sjg.ShireSakiId, sjg.ShiirePrdId, sjg.ShohinId },
            sz => new { ShireSakiId = sz.ShiireSakiId, ShiirePrdId = sz.ShiirePrdId, ShohinId = sz.ShohinId },
            (sjg, sz) => new {
                ShiireSakiId = sjg.ShireSakiId,
                ShiirePrdId = sjg.ShiirePrdId,
                ShohinId = sjg.ShohinId,
                SokoZaikoCaseSu = sjg.NonyuSubalance + sz.Sum(sz => sz.SokoZaikoCaseSu),
                SokoZaikoSu = sjg.SokoZaikoSu + sz.Sum(sz => sz.SokoZaikoSu),
                LastShiireDate = DateOnly.FromDateTime(sjg.ShireDateTime),
                SokoZaiko = sz.FirstOrDefault()
            }).Select(s => new SokoZaiko {
                ShiireSakiId = s.ShiireSakiId,
                ShiirePrdId = s.ShiirePrdId,
                ShohinId = s.ShohinId,
                SokoZaikoCaseSu = s.SokoZaikoCaseSu ?? 0,
                SokoZaikoSu = s.SokoZaikoSu,
                LastShiireDate = s.LastShiireDate
            }
              ).ToList();

            //倉庫在庫更新
            //倉庫在庫に項目名をあわせているため、単純コピーで行けるはず

            var config2 = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers();
                cfg.CreateMap<SokoZaiko, SokoZaiko>()
                .EqualityComparison((s, t)
                    => s.ShiireSakiId == t.ShiireSakiId &&
                       s.ShiirePrdId == t.ShiirePrdId &&
                         s.ShohinId == t.ShohinId
                    )
                .ForMember(t => t.ShiireMaster, opt => opt.Ignore())
                .ForMember(t => t.ShiireJissekis, opt => opt.Ignore())
                .ForMember(t => t.Version, opt => opt.Ignore()) //TimeStampを上書きしない
                ;
            });

            var mapper2 = new Mapper(config2);

            if (sokoZaikos.Count() == 0) {
                //新規倉庫在庫登録
                await _context.SokoZaiko.AddRangeAsync(result);
            }
            else {
                //既に倉庫在庫がある場合は上書き
                mapper2.Map<IList<SokoZaiko>, IList<SokoZaiko>>(result, sokoZaikos);
            }

            SokoZaikos = sokoZaikos;

            return (SokoZaikos);
        }


        /// <summary>
        /// 注文コード、仕入日を元に、次の仕入SEQを求める
        /// 仕入実績の主キーは注文コード、仕入日、仕入SEQなので、仕入日に数回仕入れる場合は、
        /// 仕入SEQをインクリメントして利用する
        /// </summary>
        /// <param name="inChumonId">仕入実績検索キー：注文コード</param>
        /// <param name="inShiireDate">仕入実績検索キー：仕入日</param>
        /// <returns>次の仕入SEQ（次に仕入実績を登録する仕入SEQ）</returns>
        public async Task<uint> NextSeq(string inChumonId, DateOnly inShiireDate) {

            //仕入実績を注文コードと仕入日を元に検索、もしあれば最大の仕入SEQを求める
            //注文コードと仕入日でレコードが起きてなければ、仕入SEQは1とする

            uint? seq = await _context.ShiireJisseki
                .Where(d => d.ChumonId == inChumonId && d.ShiireDate == inShiireDate)
                    .OrderByDescending(s => s.SeqByShiireDate)
                    .Select(x => x.SeqByShiireDate)
                    .FirstOrDefaultAsync();

            return ((seq ?? 0) + 1);
        }

        /// <summary>
        /// 仕入画面のキー入力の注文コード一覧用
        /// </summary>
        public class ChumonList {
            public string ChumonId { get; set; }
            public decimal ChumonZan { get; set; }
        }
        /// <summary>
        /// 注文残がある注文のリスト化
        /// </summary>
        /// <returns>注文残のある注文コード一覧</returns>
        public async Task<IList<ChumonList>> ZanAriChumonList() {
            IList<ChumonList> chumonIdList = await _context.ChumonJissekiMeisai
                    .Where(c => c.ChumonZan > 0).GroupBy(c => c.ChumonId).Select(group => new ChumonList {
                        ChumonId = group.Key,
                        ChumonZan = group.Sum(c => c.ChumonZan)
                    }).OrderBy(o => o.ChumonId).ToListAsync();
            //return (chumonIdList.Count() > 0 ? chumonIdList : null);
            return (chumonIdList);
        }

        /// <summary>
        /// 倉庫在庫を仕入データに接続する（表示前に利用する）　
        /// NotMappedは外部キーが使えないから、includeできないため
        /// </summary>
        /// <param name="inShiireJissekis">仕入実績</param>
        /// <param name="indata">仕入実績に結合する倉庫在庫</param>
        /// <return>倉庫在庫が接続された仕入実績</return>
        public IList<ShiireJisseki> ShiireSokoConnection(IList<ShiireJisseki> inShiireJissekis, IEnumerable<SokoZaiko> inSokoZaiko ){
            //引数で渡された仕入実績を一行づつ取り出す
            foreach (var item in inShiireJissekis) {
                //仕入実績とマッチする倉庫在庫を取得
                SokoZaiko? sokoZaiko = inSokoZaiko
                    .Where(z =>
                        z.ShiireSakiId == item.ShiireSakiId &&
                        z.ShiirePrdId == item.ShiirePrdId &&
                        z.ShohinId == item.ShohinId)
                    .FirstOrDefault();
                //マッチする倉庫在庫の有無チェック
                if (sokoZaiko != null) {    
                    //ありの場合、取得した倉庫在庫を仕入実績に接続
                    item.SokoZaiko = sokoZaiko;
                }
                else {
                    //なしの場合、倉庫在庫を新規に作成し仕入実績に接続
                    item.SokoZaiko = new SokoZaiko {
                        ShiireSakiId = item.ShiireSakiId,
                        ShiirePrdId = item.ShiirePrdId,
                        ShohinId = item.ShohinId,
                        SokoZaikoCaseSu = 0,
                        SokoZaikoSu = 0
                    };
                }
            }
            //倉庫在庫が接続された仕入実績
            return inShiireJissekis;
        }
    }
}