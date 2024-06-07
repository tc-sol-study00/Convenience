using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;

namespace Convenience.Models.Properties {

    public class Zaiko : IZaiko {
        private readonly ConvenienceContext _context;
        public IList<SokoZaiko> SokoZaikos { get; set; }

        public Zaiko() {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var contextOptions = new DbContextOptionsBuilder<ConvenienceContext>()
                .UseNpgsql(configuration["ConvenienceContext"])
                .Options;

            _context = new ConvenienceContext(contextOptions);
        }

        public Zaiko(ConvenienceContext context) {
            _context = context;
        }

        public async Task<IList<ZaikoListLine>> CreateSokoZaikoList<TSource, TKey>(Expression<Func<TSource, TKey>> sortKey, bool descending) {

            //倉庫在庫検索
            IQueryable<SokoZaiko> sokodata = _context.SokoZaiko.AsNoTracking().Include(i => i.ShiireMaster).ThenInclude(j => j.ShohinMaster);

            //ソートキーのラムダ式の変換（倉庫在庫用）
            var convertedSortKey = sortKey as Expression<Func<SokoZaiko, TKey>>;

            //ソートキーの昇順・降順設定（倉庫在庫用）
            if (typeof(TSource) == typeof(SokoZaiko)) {
                if (descending) {
                    sokodata = sokodata.OrderByDescending(convertedSortKey);
                }
                else {
                    sokodata = sokodata.OrderBy(convertedSortKey);
                }
            }

            //Postgeが外部結合できないから、倉庫在庫をリスト化してメモリに持っておく

            IList<SokoZaiko> enumSokodata = sokodata.ToList();

            //以下、注文実績明細と上記倉庫在庫の結合処理となる
            //Postgeが外部結合できないから、注文実績明細をリスト化してメモリに持っておく

            IList<ChumonJissekiMeisai> chumonJ =
                _context.ChumonJissekiMeisai.AsNoTracking().Where(x => x.ChumonZan > 0).ToList();

            //倉庫在庫の主キー粒度にあわせ、注文実績をグループ化した上で結合する
            //ZaikoListLineは、ビューモデル内にある表示用

            IQueryable<ZaikoListLine> joinresult = enumSokodata.GroupJoin(chumonJ,
                soko => new { soko.ShiireSakiId, soko.ShiirePrdId, soko.ShohinId },
                cjm => new { cjm.ShiireSakiId, cjm.ShiirePrdId, cjm.ShohinId },
                 (soko, cjm) => new ZaikoListLine {
                     SokoZaiko = soko,
                     ChumonJissekiMeisai = cjm.FirstOrDefault()
                 }).AsQueryable();

            //ソートキーの変換（メモリ上の結合結果用）
            var convertedSortKeyToCjm = sortKey as Expression<Func<ZaikoListLine, TKey>>;

            //ソート順の設定をする（メモリ上の結合結果用）
            if (typeof(TSource) == typeof(ZaikoListLine)) {
                if (descending) {
                    joinresult = joinresult.OrderByDescending(convertedSortKeyToCjm);
                }
                else {
                    joinresult = joinresult.OrderBy(convertedSortKeyToCjm);
                }
            }

            //検索画面に出すために、ビューモデル内のオブジェクトに検索データ一覧を反映
            //
            IList<ZaikoListLine> returnValue = new List<ZaikoListLine>() { };

            foreach (var item in joinresult.ToList()) {
                returnValue.Add(new ZaikoListLine() {
                    SokoZaiko = item.SokoZaiko,
                    ChumonJissekiMeisai = item.ChumonJissekiMeisai
                });
            }

            return (returnValue);
        }
    }
}