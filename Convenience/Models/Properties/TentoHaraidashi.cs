﻿using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Convenience.Models.ViewModels.TentoHaraidashi;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Microsoft.Build.Framework;

namespace Convenience.Models.Properties {
    /// <summary>
    /// 店頭払出クラス
    /// </summary>
    public class TentoHaraidashi : ITentoHaraidashi {

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// <para>プロパティ</para>
        /// <para>店頭払出ヘッダー</para>
        /// </summary>
        public TentoHaraidashiHeader? TentoHaraidashiHeader { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context"></param>
        public TentoHaraidashi(ConvenienceContext context) {
            this._context = context;
        }

        /// <summary>
        /// 店頭払出コード発番
        /// </summary>
        /// <param name="argCurrentDateTime">店頭払出日時</param>
        /// <returns>店頭払出コード(yyyyMMdd-HH-001～999、HHは店頭払出日時の時間部分</returns>
        private string TentoHaraidashiHatsuban(DateTime argCurrentDateTime) {
            string dateArea = argCurrentDateTime.ToString("yyyyMMdd-HH");

            string? maxTentoHaraidashiId = _context.TentoHaraidashiHearder
                .Where(x => x.TentoHaraidashiId.StartsWith(dateArea)).Max(s => s.TentoHaraidashiId);

            uint seq = 0;
            if (maxTentoHaraidashiId is null) {
                seq = 1;
            }
            else {
                seq = uint.Parse(maxTentoHaraidashiId.Substring(12, 3)) + 1;
            }
            return $"{dateArea}-{seq:000}";
        }

        /// <summary>
        /// <para>倉庫在庫から店頭払出実績（ヘッダー＋実績）を作成する</para>
        /// <para>①店頭払出ヘッダーを作成する</para>
        /// <para>②倉庫在庫より、店頭払出実績（ヘッダー＋実績）を作成する</para>
        /// <para>③データ表示用に＋倉庫在庫＋仕入マスタもリンク接続する</para>
        /// <para>4️⃣店頭在庫をリンク接続する</para>
        /// </summary>
        /// <param name="argCurrentDateTime"></param>
        /// <returns>TentoHaraidashiHeader 店頭払出ヘッダー＋店頭払出実績</returns>
        public async Task<TentoHaraidashiHeader> TentoHaraidashiSakusei(DateTime argCurrentDateTime) {

            /*
             * 店頭払出ヘッダーを作成する
             */
            this.TentoHaraidashiHeader = new TentoHaraidashiHeader {
                TentoHaraidashiId = TentoHaraidashiHatsuban(argCurrentDateTime),
                HaraidashiDateTime = argCurrentDateTime,
            };
            
            /*
             * 倉庫在庫より、店頭払出実績（ヘッダー＋実績）を作成する
             * データ表示用に＋倉庫在庫＋仕入マスタもリンク接続する
             */
            this.TentoHaraidashiHeader.TentoHaraidashiJissekis = await _context.SokoZaiko
                .Where(sokozaiko => sokozaiko.SokoZaikoCaseSu > 0 && sokozaiko.SokoZaikoSu > 0)
                .Include(sokozaiko => sokozaiko.ShiireMaster)
                .ThenInclude(shiiremaster => shiiremaster!.ShohinMaster)
                .ThenInclude(shohinmaster => shohinmaster!.TentoZaiko)
                .Include(sokozaiko => sokozaiko.ShiireMaster)
                .ThenInclude(shiiremaster => shiiremaster!.SokoZaiko)
                .Select(x => new TentoHaraidashiJisseki {
                    TentoHaraidashiId = this.TentoHaraidashiHeader.TentoHaraidashiId,
                    ShiireSakiId = x.ShiireSakiId,
                    ShiirePrdId = x.ShiirePrdId,
                    ShohinId = x.ShohinId,
                    ShireDateTime = x.LastShiireDate,
                    HaraidashiDate = argCurrentDateTime,
                    HaraidashiCaseSu = 0,
                    HaraidashiSu = 0,
                    ShiireMaster = x.ShiireMaster,
                })
                .ToListAsync();

            /*
             * 店頭在庫をリンク接続する
             */
            foreach (TentoHaraidashiJisseki tentoHaraidashiJisseki in this.TentoHaraidashiHeader.TentoHaraidashiJissekis) {
                
                _ = tentoHaraidashiJisseki?.ShiireMaster?.ShohinMaster ?? throw new InvalidDataException("店頭在庫リンクエラー");
                _ = tentoHaraidashiJisseki.ShiireMaster.SokoZaiko ?? throw new InvalidDataException("倉庫在庫リンクエラー");

                if (tentoHaraidashiJisseki.ShiireMaster.ShohinMaster.TentoZaiko is null) {
                    tentoHaraidashiJisseki.ShiireMaster.ShohinMaster.TentoZaiko
                        = new TentoZaiko {
                            ShohinId = tentoHaraidashiJisseki.ShohinId,
                            ZaikoSu = 0,
                            LastShireDateTime = tentoHaraidashiJisseki.ShiireMaster.SokoZaiko.LastShiireDate,
                            LastHaraidashiDate = argCurrentDateTime,
                            LastUriageDatetime = null,
                        };
                }
            }

            //DBへ追加指示
            _context.Add(this.TentoHaraidashiHeader);

            return (this.TentoHaraidashiHeader);
        }

        /// <summary>
        /// <para>店頭払出問い合わせ</para>
        /// <para>①店頭払出ヘッダー＋実績を問い合わせる</para>
        /// <para>②実績に倉庫在庫をくっつける</para>
        /// <para>②実績に仕入マスタ＋商品マスタをくっつける</para>
        /// <para>③実績に店頭在庫をくっつける</para>
        /// </summary>
        /// <param name="argTentoHaraidashiId">店頭払出コード</param>
        /// <returns>TentoHaraidashiHeader 店頭払出ヘッダ</returns>
        public async Task<TentoHaraidashiHeader?> TentoHaraidashiToiawase(string argTentoHaraidashiId) {
            /*
             * 店頭払出コードより店頭払出ヘッダー以下を検索
             */
            this.TentoHaraidashiHeader =
                await _context.TentoHaraidashiHearder
                .Where(tentoheader => tentoheader.TentoHaraidashiId == argTentoHaraidashiId)
                //仕入マスタ→倉庫在庫
                .Include(tentoheader => tentoheader.TentoHaraidashiJissekis)
                .ThenInclude(harajisseki => harajisseki.ShiireMaster)
                .ThenInclude(shiiremaster => shiiremaster!.SokoZaiko)
                //仕入マスタ→商品マスタ→店頭在庫
                .Include(tentoheader => tentoheader.TentoHaraidashiJissekis)
                .ThenInclude(harajisseki => harajisseki.ShiireMaster)
                .ThenInclude(shiiremaster => shiiremaster!.ShohinMaster)
                .ThenInclude(x => x!.TentoZaiko)
                .FirstOrDefaultAsync();

            return this.TentoHaraidashiHeader;
        }

        /// <summary>
        /// Postデータを上書きしてＤＢ更新準備をする
        /// </summary>
        /// <param name="argTentoHaraidashiJissekis">Postされた店頭払出実績</param>
        /// <returns>IList<TentoHaraidashiJisseki> Postデータが上書きされた店頭払出実績</returns>
        /// <exception cref="Exception"></exception>
        public IList<TentoHaraidashiJisseki> TentoHaraidashiUpdate(IEnumerable<TentoHaraidashiJisseki> argTentoHaraidashiJissekis) {

            _ = TentoHaraidashiHeader ?? throw new Exception("引数エラー");
            IList<TentoHaraidashiJisseki> settingTentoHaraidashiJissekis = this.TentoHaraidashiHeader.TentoHaraidashiJissekis;

            decimal shiirePcsPerUnit = default;
            decimal defHaraidashiCaseSu = default;
            decimal beforeSokoZaikoSu = default;
            decimal beforeSokoZaikoCaseSu = default;
            decimal beforeTentoZaikoSu = default;

            //上乗せ前の事前チェック
            if (settingTentoHaraidashiJissekis.Any(th => th.ShiireMaster?.ShohinMaster?.TentoZaiko == null) == true) throw new Exception("仕入マスタor商品マスタor店頭在庫にnullのデータがあります");
            if (settingTentoHaraidashiJissekis.Any(th => th.ShiireMaster?.SokoZaiko == null) == true) throw new Exception("倉庫在庫にnullのデータがあります");

            /*
             * Postデータを上乗せする
             */
            var config = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers();
                cfg.CreateMap<TentoHaraidashiJisseki, TentoHaraidashiJisseki>()
                .EqualityComparison((odto, o) => odto.TentoHaraidashiId == o.TentoHaraidashiId && odto.ShiireSakiId == o.ShiireSakiId && odto.ShiirePrdId == o.ShiirePrdId && odto.ShohinId == o.ShohinId)
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
                .ForPath(dest => dest.ShiireMaster!.SokoZaiko!.SokoZaikoCaseSu, opt => opt.MapFrom(src => beforeSokoZaikoCaseSu - defHaraidashiCaseSu))
                .ForPath(dest => dest.ShiireMaster!.SokoZaiko!.SokoZaikoSu, opt => opt.MapFrom(src => beforeSokoZaikoSu - defHaraidashiCaseSu * shiirePcsPerUnit))
                .ForPath(dest => dest.ShiireMaster!.SokoZaiko!.LastDeliveryDate, opt => { opt.MapFrom(src => DateOnly.FromDateTime(src.HaraidashiDate)); opt.Condition(x => defHaraidashiCaseSu > 0); })
                .ForPath(dest => dest.ShiireMaster!.ShohinMaster!.TentoZaiko!.ZaikoSu, opt => opt.MapFrom(src => beforeTentoZaikoSu + defHaraidashiCaseSu * shiirePcsPerUnit))
                ;
            });
            //引数で渡された注文実績をDBから読み込んだ注文実績に上書きする
            var mapper = new Mapper(config);

            mapper.Map(argTentoHaraidashiJissekis, settingTentoHaraidashiJissekis);

            this.TentoHaraidashiHeader.TentoHaraidashiJissekis = settingTentoHaraidashiJissekis;

            return this.TentoHaraidashiHeader.TentoHaraidashiJissekis;
        }


            /// <summary>
            /// 店頭払出ヘッダーのリストを条件より作成
            /// </summary>
            /// <param name="whereExpression">条件式</param>
            /// <returns>IQueryable<TentoHaraidashiHeader> 店頭払出ヘッダーリスト（遅延実行）</returns>
            public IQueryable<TentoHaraidashiHeader> TentoHaraidashiHeaderList(Expression<Func<TentoHaraidashiHeader,bool>> whereExpression) =>
            whereExpression is null ? _context.TentoHaraidashiHearder:_context.TentoHaraidashiHearder.Where(whereExpression);
    }
}
