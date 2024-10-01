using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Convenience.Models.ViewModels.TentoHaraidashi;

namespace Convenience.Models.Properties {
    public class TentoHaraidashi : ITentoHaraidashi {

        private readonly ConvenienceContext _context;
        public TentoHaraidashiJisseki TentoHaraidashiJisseki { get; set; }

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

            string? maxTentoHaraidashiId = _context.TentoHaraidashiHearder.Where(x => x.TentoHaraidashiId.StartsWith(dateArea)).Max(s => s.TentoHaraidashiId);

            uint seq = 0;
            if (maxTentoHaraidashiId is null) {
                seq = 1;
            }
            else {
                seq = uint.Parse(maxTentoHaraidashiId.Substring(11, 3)) + 1;
            }
            return $"{dateArea}-{seq:000}";
        }

        /// <summary>
        /// <para>倉庫在庫から店頭払出実績（ヘッダー＋実績）を作成する</para>
        /// <para>①店頭払出ヘッダーを作成する</para>
        /// <para>②倉庫在庫より、店頭払出実績（ヘッダー＋実績）を作成する</para>
        /// <para>③データ表示用に＋倉庫在庫＋仕入マスタもリンク接続する</para>
        /// </summary>
        /// <param name="argCurrentDateTime"></param>
        /// <returns>TentoHaraidashiHeader 店頭払出ヘッダー＋店頭払出実績</returns>
        public async Task<TentoHaraidashiHeader> TentoHaraidashiSakusei(DateTime argCurrentDateTime) {

            /*
             * 店頭払出ヘッダーを作成する
             */
            TentoHaraidashiHeader tentoHaraidashiHeader = new TentoHaraidashiHeader {
                TentoHaraidashiId = TentoHaraidashiHatsuban(argCurrentDateTime),
                HaraidashiDateTime = argCurrentDateTime.ToUniversalTime(),
            };
            /*
             * 倉庫在庫より、店頭払出実績（ヘッダー＋実績）を作成する
             * データ表示用に＋倉庫在庫＋仕入マスタもリンク接続する
             */

            tentoHaraidashiHeader.TentoHaraidashiJissekis = _context.SokoZaiko.AsNoTracking()
                .Where(sokozaiko => sokozaiko.SokoZaikoCaseSu > 0 && sokozaiko.SokoZaikoSu > 0)
                .Include(sokozaiko => sokozaiko.ShiireMaster)
                .ThenInclude(shiiremaster => shiiremaster.ShohinMaster)
                .ThenInclude(shohinmaster => shohinmaster.TentoZaiko)
                .Select(x => new TentoHaraidashiJisseki {
                    ShiireSakiId = x.ShiireSakiId,
                    ShiirePrdId = x.ShiirePrdId,
                    ShohinId = x.ShohinId,
                    ShireDateTime = x.LastShiireDate,
                    HaraidashiDate = argCurrentDateTime.ToUniversalTime(),
                    HaraidashiCaseSu = 0,
                    HaraidashiSu = 0,
                    ShiireMaster = x.ShiireMaster,
                }).ToList();

            _context.Add(tentoHaraidashiHeader);

            return (tentoHaraidashiHeader);
        }

        /// <summary>
        /// <para>店頭払出問い合わせ</para>
        /// <para>①店頭払出ヘッダー＋実績を問い合わせる</para>
        /// <para>②実績に倉庫在庫をくっつける</para>
        /// <para>②実績に仕入マスタ＋商品マスタをくっつける</para>
        /// <para>③実績に店頭在庫をくっつける</para>
        /// </summary>
        /// <param name="argTentoHaraidashiId">店頭払出コード</param>
        /// <returns>店頭払出ヘッダ</returns>
        public async Task<TentoHaraidashiHeader> TentoHaraidashiToiawase(string argTentoHaraidashiId) {
            TentoHaraidashiHeader? tentoHaraidashiHeader =
                _context.TentoHaraidashiHearder
                .Where(tentoheader => tentoheader.TentoHaraidashiId == argTentoHaraidashiId)
                //仕入マスタ→倉庫在庫
                .Include(tentoheader => tentoheader.TentoHaraidashiJissekis)
                .ThenInclude(harajisseki => harajisseki.ShiireMaster)
                .ThenInclude(shiiremaster => shiiremaster.SokoZaiko)
                //仕入マスタ→商品マスタ→店頭在庫
                .Include(tentoheader => tentoheader.TentoHaraidashiJissekis)
                .ThenInclude(harajisseki => harajisseki.ShiireMaster)
                .ThenInclude(shiiremaster => shiiremaster.ShohinMaster)
                .ThenInclude(x => x.TentoZaiko)
                .FirstOrDefault();

            return tentoHaraidashiHeader;
        }
        /// <summary>
        /// 引数（マイナス値）を使用して、その引数分の日数をさかのぼり、店頭払出日付と店頭払出コード一覧を作る
        /// </summary>
        /// <param name="argReverseDaysWithMinus">さかのぼる日数（マイナスでいれる）</param>
        /// <returns>店頭払出日時・店頭払出コード</returns>
        public async Task<List<dynamic>> CreateListWithTentoHaraidashiId(int argReverseDaysWithMinus) {
            var listData = await _context.TentoHaraidashiHearder
               .Where(x => x.HaraidashiDateTime >= DateTime.Now.AddDays(argReverseDaysWithMinus).Date.ToUniversalTime())
               .OrderBy(x => x.HaraidashiDateTime)
               .Select(x => (dynamic)new { HaraidashiDateTime=x.HaraidashiDateTime, TentoHaraidashiId=x.TentoHaraidashiId})
               .ToListAsync();
            return listData;
        }
    }
}
