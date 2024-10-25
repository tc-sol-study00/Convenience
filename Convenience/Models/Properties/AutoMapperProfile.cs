using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Properties
{
    /// <summary>
    /// AutoMapper用プロファイル
    /// </summary>
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile(IKaikei kaikei)
        {

            int indexCounter = 1;
            decimal difUriageSu = default;
            /*
             * 会計ヘッダー(Post->Dto)
             */
            CreateMap<KaikeiHeader, KaikeiHeader>()
            .EqualityComparison((src, dest) => src.UriageDatetimeId == dest.UriageDatetimeId);
            /*
             * 会計実績(Post->Dto)
             */
            CreateMap<KaikeiJisseki, KaikeiJisseki>()
            .EqualityComparison((src, dest) => src.UriageDatetimeId == dest.UriageDatetimeId && src.ShohinId == dest.ShohinId)
            .ForMember(dest => dest.KaikeiSeq, opt => opt.MapFrom((src, dest) => indexCounter))
            //外部キーをnullにすると、親が消された判断で、マップされたデータがEFCoreで削除扱いされるのを防止
            .ForMember(dest => dest.KaikeiHeader, opt => opt.Ignore())
            .ForMember(dest => dest.NaigaiClassMaster, opt => opt.Ignore())
            .ForMember(dest => dest.ShohinMaster, opt => opt.Ignore())
            .ForMember(dest => dest.TentoZaiko, opt => opt.Ignore())
            .BeforeMap((src, dest) => difUriageSu = src.UriageSu - dest.UriageSu)
            .AfterMap((src, dest) =>
            {
                indexCounter++;
                kaikei.ShohizeiKeisan(src, dest);       //内外区分による税込み金額の計算
                dest.TentoZaiko = kaikei.ZaikoConnection(dest.ShohinId!, dest.UriageDatetime, difUriageSu, dest.TentoZaiko);
            });

            /*
             * 会計実績(Post->一時領域：追加ボタン用)
             */
            CreateMap<IKaikeiJissekiForAdd, KaikeiJisseki>()
            .EqualityComparison((src, dest) => src.ShohinId == dest.ShohinId)   //商品コードがすでに会計されていたら、以下の処理は加算
            .ForMember(dest => dest.UriageSu, opt => opt.MapFrom((src, dest) => dest.UriageSu + src.UriageSu))                  //売上数
            .ForMember(dest => dest.UriageKingaku, opt => opt.MapFrom((src, dest) => dest.UriageSu * src.ShohinMaster!.ShohinTanka))   //売上金額
            .ForMember(dest => dest.ShohinTanka, opt => opt.MapFrom((src, dest) => src.ShohinMaster!.ShohinTanka))                   //商品単価
            .AfterMap((src, dest) =>
            {
                kaikei.ShohizeiKeisan(src, dest);                                                                                      //内外区分による税込み金額の計算
                dest.TentoZaiko
                    = kaikei.ZaikoConnection(dest.ShohinId!, dest.UriageDatetime, src.UriageSu, dest.TentoZaiko);                      //店頭在庫への反映（売った分減る）
            });

        }
        public AutoMapperProfile(ITentoHaraidashi tentoHaraidashi)
        {

            decimal shiirePcsPerUnit = default;
            decimal defHaraidashiCaseSu = default;
            decimal beforeSokoZaikoSu = default;
            decimal beforeSokoZaikoCaseSu = default;
            decimal beforeTentoZaikoSu = default;

            CreateMap<TentoHaraidashiJisseki, TentoHaraidashiJisseki>()
            .EqualityComparison((odto, o) => odto.TentoHaraidashiId == o.TentoHaraidashiId && odto.ShiireSakiId == o.ShiireSakiId && odto.ShiirePrdId == o.ShiirePrdId && odto.ShohinId == o.ShohinId)
            .BeforeMap((src, dest) =>
            {
                defHaraidashiCaseSu = src.HaraidashiCaseSu - dest.HaraidashiCaseSu;
                shiirePcsPerUnit = dest.ShiireMaster!.ShiirePcsPerUnit;
                beforeSokoZaikoCaseSu = dest.ShiireMaster!.SokoZaiko!.SokoZaikoCaseSu;
                beforeSokoZaikoSu = dest.ShiireMaster!.SokoZaiko!.SokoZaikoSu;
                beforeTentoZaikoSu = dest.ShiireMaster!.ShohinMaster!.TentoZaiko!.ZaikoSu;
            })
            .ForMember(dest => dest.HaraidashiCaseSu, opt => opt.MapFrom(src => src.HaraidashiCaseSu))
            .ForMember(dest => dest.HaraidashiSu, opt => opt.MapFrom(src => src.HaraidashiCaseSu * shiirePcsPerUnit))
            .ForMember(dest => dest.ShiireMaster, opt => opt.Ignore())
            .ForMember(dest => dest.TentoHaraidashiHeader, opt => opt.Ignore())
            .ForPath(dest => dest.ShiireMaster!.SokoZaiko!.SokoZaikoCaseSu, opt => opt.MapFrom(src => beforeSokoZaikoCaseSu - defHaraidashiCaseSu))
            .ForPath(dest => dest.ShiireMaster!.SokoZaiko!.SokoZaikoSu, opt => opt.MapFrom(src => beforeSokoZaikoSu - defHaraidashiCaseSu * shiirePcsPerUnit))
            .ForPath(dest => dest.ShiireMaster!.SokoZaiko!.LastDeliveryDate, opt => { opt.MapFrom(src => DateOnly.FromDateTime(src.HaraidashiDate)); opt.Condition(x => defHaraidashiCaseSu > 0); })
            .ForPath(dest => dest.ShiireMaster!.ShohinMaster!.TentoZaiko!.ZaikoSu, opt => opt.MapFrom(src => beforeTentoZaikoSu + defHaraidashiCaseSu * shiirePcsPerUnit))
            ;
        }
        public AutoMapperProfile(IShiire shiire, DateOnly inShiireDate, uint inSeqByShiireDate, DateTime nowTime)
        {
            CreateMap<ChumonJissekiMeisai, ShiireJisseki>()
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
        }

        public AutoMapperProfile(IShiire shiire, ConvenienceContext _context )
        {
            var config = new MapperConfiguration(cfg =>
            {
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
        }

        public AutoMapperProfile(IChumon chumon)
        {
                CreateMap<ChumonJisseki, ChumonJisseki>()
                .EqualityComparison((odto, o) => odto.ChumonId == o.ChumonId);
                
                CreateMap<ChumonJissekiMeisai, ChumonJissekiMeisai>()
                .EqualityComparison((odto, o) => odto.ChumonId == o.ChumonId && odto.ShiireSakiId == o.ShiireSakiId && odto.ShiirePrdId == o.ShiirePrdId && odto.ShohinId == o.ShohinId)
                .BeforeMap((src, dest) => src.LastChumonSu = dest.ChumonSu)
                .ForMember(dest => dest.ChumonZan, opt => opt.MapFrom(src => src.ChumonZan + src.ChumonSu - src.LastChumonSu))
                .ForMember(dest => dest.ChumonJisseki, opt => opt.Ignore());
        }

        public class AutoMapperSharedProfile : Profile
        {
            public AutoMapperSharedProfile()
            {
                /*
                 * 会計実績（追加ボタン退避用）
                 */
                CreateMap<KaikeiJisseki, KaikeiJisseki>();
            }
        }
    }
}
