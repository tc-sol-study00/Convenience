using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Convenience.Models.Properties {
    public class Shiire : IShiire, IDbContext {
        /*
         * 仕入クラス
         */

        //DBコンテキスト
        private readonly ConvenienceContext _context;

        /*
         * プロパティ
         */
        //仕入実績
        public IList<ShiireJisseki> Shiirejissekis { get; set; } //Include ChuumonJissekiMeisai
        //倉庫在庫     
        public IList<SokoZaiko> SokoZaikos { get; set; }

        //コンストラクタ
        //通常の場合はＤＢコンテキストを引き継ぐ
        public Shiire(ConvenienceContext context) {
            _context = context;
        }
        //仕入クラスデバッグ用
        public Shiire() {
            _context = IDbContext.DbOpen();
        }

        // 仕入データPost内容の反映

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

        public class ShiireUkeireReturnSet {
            public IList<ShiireJisseki> ShiireJissekis { get; set; }
            public IList<SokoZaiko> SokoZaikos { get; set; }
        }

        //仕入実績に既に存在しているかチェック

        public async Task<bool> ChuumonIdOnShiireJissekiExistingCheck(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate) {
            var result = await _context.ShiireJisseki
               .FirstOrDefaultAsync(
                   w => w.ChumonId == inChumonId
                   && w.ShiireDate == inShiireDate
                   && w.SeqByShiireDate == inSeqByShiireDate
                );

            return (result != null);
        }

        public async Task<ShiireUkeireReturnSet> ChuumonZanZaikoSuChousei(string inChumonId, IList<ShiireJisseki> inShiireJissekis) {
            /*
                * 注文残・在庫数量調整
            */

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

        public async Task<IList<ShiireJisseki>> ShiireToShiireJisseki(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate) {
            IList<ShiireJisseki> shiireJissekis = await _context.ShiireJisseki
                .Where(c => c.ChumonId == inChumonId && c.ShiireDate == inShiireDate && c.SeqByShiireDate == inSeqByShiireDate)
                .Include(cjm => cjm.ChumonJissekiMeisaii)
                .ThenInclude(cj => cj.ChumonJisseki)
                .ThenInclude(ss => ss.ShiireSakiMaster)
                .Include(cjm => cjm.ChumonJissekiMeisaii)
                .ThenInclude(a => a.ShiireMaster)
                .ThenInclude(s => s.ShohinMaster)
                .ToListAsync();

            Shiirejissekis = shiireJissekis;

            return (Shiirejissekis);
        }

        public async Task<IList<ShiireJisseki>> ChumonToShiireJisseki(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate) {
            /*
            * 仕入実績作成
            */
            //注文明細取得（キー：注文コード）複数のレコード
            IList<ChumonJissekiMeisai> chumonJissekiMeisais = await _context.ChumonJissekiMeisai
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

            var mapper = new Mapper(config);

            IList<ShiireJisseki> shiireJissekis = new List<ShiireJisseki>();

            mapper.Map<IList<ChumonJissekiMeisai>, IList<ShiireJisseki>>(chumonJissekiMeisais, shiireJissekis);

            await _context.ShiireJisseki.AddRangeAsync(shiireJissekis);

            Shiirejissekis = shiireJissekis;

            return (Shiirejissekis);
        }

        private async Task<IList<SokoZaiko>> ZaikoSet(IList<ShiireJisseki> shiireJissekis) {
            /*
             * 倉庫在庫登録
             */

            //仕入実績（Ａ）　より倉庫在庫主キー単位にレコードを起こす（倉庫在庫と粒度をあわせるため）
            //主キー   ：仕入先、仕入商品コード、商品コード
            //集計         :納品数（ケース）、実数量（納品ケース数×ケースあたりの数量）

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
                    CaseSu = gr.Sum(i => i.NonyuSubalance),
                    Su = gr.Sum(j => j.ChumonJissekiMeisaii.ShiireMaster.ShiirePcsPerUnit * j.NonyuSubalance ?? 0),
                    ShireDateTime = gr.Max(k => k.ShiireDateTime)
                }).ToList()
                ;

            //現在の倉庫在庫を読み込む

            IList<SokoZaiko> sokoZaikos = new List<SokoZaiko>();

            foreach (var sokoZaikokey in shiireJissekiGrp) {
                SokoZaiko? sokoZaiko = await _context.SokoZaiko
               .Where(s => s.ShiireSakiId == sokoZaikokey.ShireSakiId && s.ShiirePrdId == sokoZaikokey.ShiirePrdId && s.ShohinId == sokoZaikokey.ShohinId)
               .FirstOrDefaultAsync();

                if (sokoZaiko != null) sokoZaikos.Add(sokoZaiko);
            }



            /* 以下は、.AsEnumerable()を使っていて、全件メモリに貼り付けて危険なので、上記の書き方に変更
            sokoZaikos = _context.SokoZaiko
                .AsEnumerable() //これ入れないとposgreでエラー
                .Where(s => shiireJissekiGrp
                  .Any(j => s.ShiireSakiId == j.ShireSakiId && s.ShiirePrdId == j.ShiirePrdId && s.ShohinId == j.ShohinId))
                .ToList();
            */

            var result = shiireJissekiGrp.GroupJoin(
            sokoZaikos,
            sjg => new { sjg.ShireSakiId, sjg.ShiirePrdId, sjg.ShohinId },
            sz => new { ShireSakiId = sz.ShiireSakiId, ShiirePrdId = sz.ShiirePrdId, ShohinId = sz.ShohinId },
            (sjg, sz) => new {
                ShiireSakiId = sjg.ShireSakiId,
                ShiirePrdId = sjg.ShiirePrdId,
                ShohinId = sjg.ShohinId,
                SokoZaikoCaseSu = sjg.CaseSu + sz.Sum(sz => sz.SokoZaikoCaseSu),
                SokoZaikoSu = sjg.Su + sz.Sum(sz => sz.SokoZaikoSu),
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
                //cfg.SetGeneratePropertyMaps<GenerateEntityFrameworkPrimaryKeyPropertyMaps<ConvenienceContext>>();
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
                await _context.SokoZaiko.AddRangeAsync(result);
            }
            else {
                mapper2.Map<IList<SokoZaiko>, IList<SokoZaiko>>(result, sokoZaikos);
            }

            SokoZaikos = sokoZaikos;

            return (SokoZaikos);
        }

        public class ChumonList {
            public string ChumonId { get; set; }
            public decimal ChumonZan { get; set; }
        }

        public async Task<uint> NextSeq(string inChumonId, DateOnly inShiireDate) {

            uint? seq = await _context.ShiireJisseki
                .Where(d => d.ChumonId == inChumonId && d.ShiireDate == inShiireDate)
                    .OrderByDescending(s => s.SeqByShiireDate)
                    .Select(x => x.SeqByShiireDate)
                    .FirstOrDefaultAsync();

            /*
                uint? seq = await _context.ShiireJisseki
                .Where(d => d.ChumonId == inChumonId && d.ShiireDate == inShiireDate)
                .MaxAsync(d => d.SeqByShiireDate);
            */
            uint rseq = (seq ?? 0) + 1;

            //uint seq = shiirej != null ? shiirej.SeqByShiireDate : 0;
            return (rseq);
        }

        //注文残がある注文のリスト化
        public async Task<IList<ChumonList>> ZanAriChumonList() {
            IList<ChumonList> chumonIdList = await _context.ChumonJissekiMeisai
                    .Where(c => c.ChumonZan > 0).GroupBy(c => c.ChumonId).Select(group => new ChumonList {
                        ChumonId = group.Key,
                        ChumonZan = group.Sum(c => c.ChumonZan)
                    }).OrderBy(o => o.ChumonId).ToListAsync();
            //return (chumonIdList.Count() > 0 ? chumonIdList : null);
            return (chumonIdList);
        }

        //倉庫在庫を仕入データに接続する（表示前に利用する）　NotMappedは外部キーが使えないから、includeできないため
        public void ShiireSokoConnection(IList<ShiireJisseki> inShiireJissekis, IEnumerable<SokoZaiko> indata) {
            //
            foreach (var item in inShiireJissekis) {
                SokoZaiko? sokoZaiko = indata
                    .Where(z =>
                        z.ShiireSakiId == item.ShiireSakiId &&
                        z.ShiirePrdId == item.ShiirePrdId &&
                        z.ShohinId == item.ShohinId)
                    .FirstOrDefault();
                //
                if (sokoZaiko != null) {
                    //
                    item.SokoZaiko = sokoZaiko;
                }
                else {
                    //
                    item.SokoZaiko = new SokoZaiko {
                        ShiireSakiId = item.ShiireSakiId,
                        ShiirePrdId = item.ShiirePrdId,
                        ShohinId = item.ShohinId,
                        SokoZaikoCaseSu = 0,
                        SokoZaikoSu = 0
                    };

                }
            }
        }
    }
}