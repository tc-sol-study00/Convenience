using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.ChumonJisseki;
using Convenience.Models.ViewModels.KaikeiJisseki;
using Convenience.Models.ViewModels.ShiireJisseki;
using Convenience.Models.ViewModels.TentoZaiko;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Convenience.Models.Properties {
    /// <summary>
    ///  （会計クラス用）会計ヘッダー＋実績をPostデータからDTOに反映
    /// </summary>
    public class KaikeiPostToDTOAutoMapperProfile : Profile {
        public KaikeiPostToDTOAutoMapperProfile(IKaikei kaikei) {

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
            .EqualityComparison((src, dest) =>
                src.UriageDatetimeId == dest.UriageDatetimeId &&
                src.ShohinId == dest.ShohinId
             )
            .ForMember(dest => dest.KaikeiSeq, opt => opt.MapFrom((src, dest) => indexCounter))
            //外部キーをnullにすると、親が消された判断で、マップされたデータがEFCoreで削除扱いされるのを防止
            .ForMember(dest => dest.KaikeiHeader, opt => opt.Ignore())
            .ForMember(dest => dest.NaigaiClassMaster, opt => opt.Ignore())
            .ForMember(dest => dest.ShohinMaster, opt => opt.Ignore())
            .ForMember(dest => dest.TentoZaiko, opt => opt.Ignore())
            .BeforeMap((src, dest) => difUriageSu = src.UriageSu - dest.UriageSu)
            .AfterMap((src, dest) => {
                indexCounter++;
                kaikei.ShohizeiKeisan(src, dest);       //内外区分による税込み金額の計算
                dest.TentoZaiko = kaikei.ZaikoConnection(dest.ShohinId!, dest.UriageDatetime, difUriageSu, dest.TentoZaiko);
            }
            );
        }
    }

    /// <summary>
    ///  （会計クラス用）会計実績(Post->一時領域：追加ボタン用)
    /// </summary>
    public class KaikeiAddLineToTempDataAutoMapperProfile : Profile {

        public KaikeiAddLineToTempDataAutoMapperProfile(IKaikei kaikei) {

            /*
             * 会計実績(Post->一時領域：追加ボタン用)
             */
            CreateMap<IKaikeiJissekiForAdd, KaikeiJisseki>()
                .EqualityComparison((src, dest) => src.ShohinId == dest.ShohinId)   //商品コードがすでに会計されていたら、以下の処理は加算
                .ForMember(dest => dest.UriageSu, opt => opt.MapFrom((src, dest) => dest.UriageSu + src.UriageSu))          //売上数
                .ForMember(dest => dest.UriageKingaku,
                    opt => opt.MapFrom((src, dest) => dest.UriageSu * src.ShohinMaster!.ShohinTanka)
                 )                                                                                                          //売上金額
                .ForMember(dest => dest.ShohinTanka, opt => opt.MapFrom((src, dest) => src.ShohinMaster!.ShohinTanka))      //商品単価
                .AfterMap((src, dest) => {
                    kaikei.ShohizeiKeisan(src, dest);                                                                   //内外区分による税込み金額の計算
                    dest.TentoZaiko
                        = kaikei.ZaikoConnection(dest.ShohinId!, dest.UriageDatetime, src.UriageSu, dest.TentoZaiko);   //店頭在庫への反映（売った分減る）
                }
                 );
        }
    }
    /// <summary>
    /// （会計クラス用） 会計実績（追加ボタン退避用）
    /// </summary>
    public class KaikeiJissekiforSaveAutoMapperProfile : Profile {
        public KaikeiJissekiforSaveAutoMapperProfile() {
            /*
             * 会計実績（追加ボタン退避用）
             */
            CreateMap<KaikeiJisseki, KaikeiJisseki>();
        }
    }
    /// <summary>
    /// （店頭払出クラス用）PostデータをDTOに反映
    /// </summary>
    public class TentoHaraidashiPostToDTOAutoMapperProfile : Profile {
        public TentoHaraidashiPostToDTOAutoMapperProfile() {

            decimal shiirePcsPerUnit = default;
            decimal defHaraidashiCaseSu = default;
            decimal beforeSokoZaikoSu = default;
            decimal beforeSokoZaikoCaseSu = default;
            decimal beforeTentoZaikoSu = default;

            CreateMap<TentoHaraidashiJisseki, TentoHaraidashiJisseki>()
            .EqualityComparison((odto, o) =>
                odto.TentoHaraidashiId == o.TentoHaraidashiId &&
                odto.ShiireSakiId == o.ShiireSakiId &&
                odto.ShiirePrdId == o.ShiirePrdId &&
                odto.ShohinId == o.ShohinId
            )
            .BeforeMap((src, dest) => {
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
            .ForPath(dest => dest.ShiireMaster!.SokoZaiko!.SokoZaikoCaseSu,
                opt => opt.MapFrom(src => beforeSokoZaikoCaseSu - defHaraidashiCaseSu)
             )
            .ForPath(dest => dest.ShiireMaster!.SokoZaiko!.SokoZaikoSu,
                opt => opt.MapFrom(src => beforeSokoZaikoSu - defHaraidashiCaseSu * shiirePcsPerUnit)
             )
            .ForPath(dest => dest.ShiireMaster!.SokoZaiko!.LastDeliveryDate,
                opt => { opt.MapFrom(src => DateOnly.FromDateTime(src.HaraidashiDate)); opt.Condition(x => defHaraidashiCaseSu > 0); }
             )
            .ForPath(dest => dest.ShiireMaster!.ShohinMaster!.TentoZaiko!.ZaikoSu,
                opt => opt.MapFrom(src => beforeTentoZaikoSu + defHaraidashiCaseSu * shiirePcsPerUnit)
             )
            ;
        }
    }
    /// <summary>
    /// （仕入クラス用）注文実績から仕入実績に自動反映するため用
    /// </summary>
    public class ShiireConvChumonJissekiToShiireJissekiAutoMapperProfile : Profile {
        public ShiireConvChumonJissekiToShiireJissekiAutoMapperProfile() {
            CreateMap<ChumonJissekiMeisai, ShiireJisseki>()
            .ForMember(dest => dest.ChumonId, opt => opt.MapFrom(src => src.ChumonId))
            .ForMember(dest => dest.ShiireDate, opt => opt.MapFrom((src, dest, destMember, context) => context.Items["ShiireDate"]))
            .ForMember(dest => dest.SeqByShiireDate, opt => opt.MapFrom((src, dest, destMember, context) => context.Items["SeqByShiireDate"]))
            .ForMember(dest => dest.ShiireDateTime, opt => opt.MapFrom((src, dest, destMember, context) => context.Items["ShiireDateTime"]))
            .ForMember(dest => dest.ShiireSakiId, opt => opt.MapFrom(src => src.ShiireSakiId))
            .ForMember(dest => dest.ShiirePrdId, opt => opt.MapFrom(src => src.ShiirePrdId))
            .ForMember(dest => dest.ShohinId, opt => opt.MapFrom(src => src.ShohinId))
            .ForMember(dest => dest.NonyuSu, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.ChumonJissekiMeisaii, opt => opt.MapFrom(src => src));
        }
    }
    /// <summary>
    /// （仕入クラス用）PostデータからDTO反映用
    /// </summary>
    public class ShiirePostToDTOAutoMapperProfile : Profile {
        public ShiirePostToDTOAutoMapperProfile(ConvenienceContext _context) {
            CreateMap<ShiireJisseki, ShiireJisseki>()
            .EqualityComparison((odto, o) =>
                odto.ShiireSakiId == o.ShiireSakiId &&
                odto.ShiirePrdId == o.ShiirePrdId
             )
            .ForMember(dest => dest.ShiireDateTime,
                opt => opt.MapFrom(src => DateTime.SpecifyKind(src.ShiireDateTime, DateTimeKind.Utc))
             )
            .ForMember(dest => dest.NonyuSu, opt => opt.MapFrom(src => src.NonyuSu))
            .BeforeMap((src, dest) => dest.NonyuSubalance = src.NonyuSu - dest.NonyuSu)
            .ForMember(dest => dest.NonyuSubalance, opt => opt.Ignore())
            .ForMember(dest => dest.ChumonJissekiMeisaii, opt => opt.Ignore())
            .AfterMap((src, dest) => {
                _context.Entry(dest).Property(v => v.Version).OriginalValue = src.Version;
                _context.Entry(dest.ChumonJissekiMeisaii).Property(v => v.Version).OriginalValue
                                                                        = src.ChumonJissekiMeisaii.Version;
            }
             );
        }
    }

    /// <summary>
    /// （注文クラス用）PostデータをDTOに反映
    /// </summary>
    public class ChumonChumonJissekiToDTOAutoMapperProfile : Profile {

        public ChumonChumonJissekiToDTOAutoMapperProfile() {
            CreateMap<ChumonJisseki, ChumonJisseki>()
            .EqualityComparison((odto, o) => odto.ChumonId == o.ChumonId);

            CreateMap<ChumonJissekiMeisai, ChumonJissekiMeisai>()
            .EqualityComparison((odto, o) =>
                odto.ChumonId == o.ChumonId &&
                odto.ShiireSakiId == o.ShiireSakiId &&
                odto.ShiirePrdId == o.ShiirePrdId &&
                odto.ShohinId == o.ShohinId
             )
            .BeforeMap((src, dest) => src.LastChumonSu = dest.ChumonSu)
            .ForMember(dest => dest.ChumonZan, opt => opt.MapFrom(src => src.ChumonZan + src.ChumonSu - src.LastChumonSu))
            .ForMember(dest => dest.ChumonJisseki, opt => opt.Ignore());
        }
    }
    /// <summary>
    /// （注文クラス用）仕入マスタから注文実績モデルへ自動反映用
    /// </summary>
    public class ChumonCreateChumonJissekiToDTOAutoMapperProfile : Profile {
        public ChumonCreateChumonJissekiToDTOAutoMapperProfile() {
            CreateMap<ShiireMaster, ChumonJissekiMeisai>()
            .ForMember(dest => dest.ChumonId, opt => opt.MapFrom((src, dest, destMember, context) => context.Items["ChumonId"]))
            .ForMember(dest => dest.ShiireSakiId, opt => opt.MapFrom((src, dest, destMember, context) => context.Items["ShiireSakiId"]))
            .ForMember(dest => dest.ShiirePrdId, opt => opt.MapFrom(src => src.ShiirePrdId))    //仕入商品コードのセット(bより）
            .ForMember(dest => dest.ShohinId, opt => opt.MapFrom(src => src.ShohinId))          //仕入マスタから商品コード（bより）
            .ForMember(dest => dest.ChumonSu, opt => opt.MapFrom(opt => 0))                     //初期値として注文数０をセット
            .ForMember(dest => dest.ChumonZan, opt => opt.MapFrom(opt => 0))                    //初期値として注文残０をセット
            .ForMember(dest => dest.ShiireMaster, opt => opt.MapFrom(src => src))               //仕入マスタに対するリレーション情報のセット
            ;
        }
    }
    /// <summary>
    /// <para>店頭在庫サービス用）店頭在庫検索結果をを画面に反映する</para>
    /// <para>Mapping クエリの結果 to　店頭在庫ビューモデル</para>
    /// </summary>
    public class TentoZaikoPostdataToTentoZaikoViewModel : Profile {
        public TentoZaikoPostdataToTentoZaikoViewModel() {
            CreateMap<TentoZaikoViewModel, TentoZaikoViewModel>();
            CreateMap<TentoZaikoViewModel.DataAreaClass, TentoZaikoViewModel.DataAreaClass>();
            CreateMap<TentoZaiko, TentoZaikoViewModel.DataAreaClass.TentoZaIkoLine>()
            .ForMember(dest => dest.ShohinName, opt => opt.MapFrom(src => src.ShohinMaster!.ShohinName))
            ;
        }
    }
    /// <summary>
    /// （会計実績サービス用）クエリの結果を画面に反映する
    /// </summary>
    public class KaikeiJissekiPostdataToKaikeiJissekiViewModel : Profile {
        public KaikeiJissekiPostdataToKaikeiJissekiViewModel() {
            CreateMap<KaikeiJissekiViewModel, KaikeiJissekiViewModel>();
            CreateMap<KaikeiJissekiViewModel.DataAreaClass, KaikeiJissekiViewModel.DataAreaClass>();
            CreateMap<KaikeiJisseki, KaikeiJissekiViewModel.DataAreaClass.KaikeiJissekiLineClass>()
            .ForMember(dest => dest.ShohinName, opt => opt.MapFrom(src => src.ShohinMaster!.ShohinName))
            .ForMember(dest => dest.NaigaiClassName, opt => opt.MapFrom(src => src.NaigaiClassMaster!.NaigaiClassName))
            ;
        }
    }
    /// <summary>
    /// （注文実績サービス用）クエリの結果を画面に反映する
    /// </summary>
    public class ChumonJissekiPostdataToChumonJissekiViewModel : Profile {

        public ChumonJissekiPostdataToChumonJissekiViewModel() {
            CreateMap<ChumonJissekiViewModel, ChumonJissekiViewModel>();
            CreateMap<ChumonJissekiViewModel.DataAreaClass, ChumonJissekiViewModel.DataAreaClass>();
            CreateMap<ChumonJissekiMeisai, ChumonJissekiViewModel.DataAreaClass.ChumonJissekiLineClass>()
            .ForMember(dest => dest.ChumonDate, opt => opt.MapFrom(src => src.ChumonJisseki!.ChumonDate))
            .ForMember(dest => dest.ShiireSakiKaisya, opt => opt.MapFrom(src => src.ShiireMaster!.ShiireSakiMaster!.ShiireSakiKaisya))
            .ForMember(dest => dest.ShiirePrdName, opt => opt.MapFrom(src => src.ShiireMaster!.ShiirePrdName))
            .ForMember(dest => dest.ShohinName, opt => opt.MapFrom(src => src.ShiireMaster!.ShohinMaster!.ShohinName))
            .ForMember(dest => dest.ChumonKingaku, opt => opt.MapFrom(src => src.ChumonSu * src.ShiireMaster!.ShireTanka))
            .ForMember(dest => dest.ChumonZanKingaku, opt => opt.MapFrom(src => src.ChumonZan * src.ShiireMaster!.ShireTanka))
            .ForMember(dest => dest.ShiireZumiSu, opt => opt.MapFrom(src => src.ChumonSu - src.ChumonZan))
            .ForMember(dest => dest.ShiireZumiKingaku, opt => opt.MapFrom(src => (src.ChumonSu - src.ChumonZan) * src.ShiireMaster!.ShireTanka))
            ;
        }
    }

    /// <summary>
    /// （仕入実績サービス用）クエリの結果を画面に反映する
    /// </summary>
    public class ShiireJissekiPostdataToShiireJissekiViewModel : Profile {
        public ShiireJissekiPostdataToShiireJissekiViewModel() {
            CreateMap<ShiireJissekiViewModel, ShiireJissekiViewModel>();
            CreateMap<ShiireJissekiViewModel.DataAreaClass, ShiireJissekiViewModel.DataAreaClass>();
            CreateMap<ShiireJisseki, ShiireJissekiViewModel.DataAreaClass.ShiireJissekiLineClass>()
            .ForMember(dest => dest.ShiireSakiKaisya, opt => opt.MapFrom(src => src.ChumonJissekiMeisaii.ShiireMaster!.ShiireSakiMaster!.ShiireSakiKaisya))
            .ForMember(dest => dest.ShiirePrdName, opt => opt.MapFrom(src => src.ChumonJissekiMeisaii.ShiireMaster!.ShiirePrdName))
            .ForMember(dest => dest.ShohinName, opt => opt.MapFrom(src => src.ChumonJissekiMeisaii.ShiireMaster!.ShohinMaster!.ShohinName))
            ;
        }
    }

}
