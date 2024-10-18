using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;

namespace Convenience.Models.Properties {

    /// <summary>
    /// 注文実績明細検索＆倉庫在庫（遅延実行）
    /// </summary>
    public class Zaiko : IZaiko {

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;
        /// <summary>
        /// 生成される問い合わせ（遅延実行用）
        /// </summary>
        public IQueryable<ZaikoListLine>? SoKoZaikoQueryable { get; set; } = null;
        /// <summary>
        /// OrderByかThenByか切り替えるフラグ
        /// </summary>
        bool isFirstCalled = true;  //true->Orderby false->ThenBy

        /// <summary>
        /// コンストラクター（コンソール実行＝デバッグ用）
        /// </summary>
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

        /// <summary>
        /// ＤＢコンテキスト設定（ＡＳＰ実行用）
        /// </summary>
        /// <param name="context"></param>
        public Zaiko(ConvenienceContext context) {
            _context = context;
        }

        /// <summary>
        /// 注文実績明細検索＆倉庫在庫（遅延実行）
        /// </summary>
        /// <param name="inSearchKey">検索キー</param>
        /// <returns>倉庫在庫　＆　注文実績明細(変数:SoKoZaikoQueryable) Where指示付き　ISoKoZaikoQueryable型にして遅延実行化</returns>
        public IQueryable<ZaikoListLine> CreateSokoZaikoList(string inSearchKey) {

            //検索処理初回か？　nullの場合初回
            //Whereが複数セットされるが、セットのたびにこのメソッドはコールされるため、初回判断が必要
            if (SoKoZaikoQueryable == null) {

                //初回の場合は、注文実績と倉庫在庫を結合するLinq実行
                //注文実績検索（遅延実行）
                IQueryable<ChumonJissekiMeisai> chumonJissekiQueryable = _context.ChumonJissekiMeisai.AsNoTracking()
                .Where(x => x.ChumonZan > 0)
                .GroupBy(gr => new { gr.ShiireSakiId, gr.ShiirePrdId, gr.ShohinId })
                .Select(s => new ChumonJissekiMeisai { ShiireSakiId = s.Key.ShiireSakiId, ShiirePrdId = s.Key.ShiirePrdId, ShohinId = s.Key.ShohinId, ChumonSu = s.Sum(item => item.ChumonSu), ChumonZan = s.Sum(item => item.ChumonZan) })
                .AsQueryable();

                //倉庫在庫検索（遅延実行）
                SoKoZaikoQueryable = _context.SokoZaiko.AsNoTracking()
                    .Include(i => i.ShiireMaster).ThenInclude(j => j!.ShohinMaster)
                    .GroupJoin(chumonJissekiQueryable,
                            soko => new { soko.ShiireSakiId, soko.ShiirePrdId, soko.ShohinId },
                            cjm => new { cjm.ShiireSakiId, cjm.ShiirePrdId, cjm.ShohinId },
                            (soko, cjm) => new { soko, cjm })
                    .SelectMany(
                            soko => soko.cjm.DefaultIfEmpty(),
                            (soko, cjm) => new ZaikoListLine {
                                ShiireSakiId = soko.soko.ShiireSakiId,
                                ShiirePrdId = soko.soko.ShiirePrdId,
                                ShohinId = soko.soko.ShohinId,
                                ShohinName = soko.soko.ShiireMaster!.ShohinMaster!.ShohinName,
                                SokoZaikoCaseSu = soko.soko.SokoZaikoCaseSu,
                                SokoZaikoSu = soko.soko.SokoZaikoSu,
                                LastShiireDate = soko.soko.LastShiireDate,
                                LastDeliveryDate = soko.soko.LastDeliveryDate,
                                ChumonZan = cjm!.ChumonZan
                            })
                    .AsQueryable();
            }

            //inSearchKeyによるwhereの設定
            //初回のみならず、メソッド実行のたびに、whereが追加される
            SoKoZaikoQueryable = !string.IsNullOrEmpty(inSearchKey) ?
                SoKoZaikoQueryable = SoKoZaikoQueryable.Where(inSearchKey).AsQueryable() : SoKoZaikoQueryable;

            //Linqの結果返却（まだ、EFは実行されていない）
            //倉庫在庫　＆　注文実績明細(Where指示付き）
            return SoKoZaikoQueryable;
        }

        /// <summary>
        ///  機能：注文実績明細検索＆倉庫在庫（遅延実行）＋Where内容の状態から、ソート順の追加セットを行う
        /// </summary>
        /// <param name="sortKey">ソートキー</param>
        /// <param name="descending">降順・昇順区分</param>
        /// <returns>倉庫在庫　＆　注文実績明細(変数:SoKoZaikoQueryable) ソート指示付き　ISoKoZaikoQueryable型にして遅延実行化</returns>
        public IQueryable<ZaikoListLine>? AddOrderby(string sortKey, bool descending) {

            //倉庫在庫の検索指示があるかどうか。あればオーダー設定を追加する   
            if (SoKoZaikoQueryable != null) {

                //ソートキーの昇順・降順設定（遅延実行）
                IQueryable<ZaikoListLine>? orderQueryable;
                
                if (isFirstCalled) {
                    //初回であれば、OrderBy
                    if (descending) {
                        //降順
                        orderQueryable=SoKoZaikoQueryable.OrderBy(sortKey + " descending");
                    }
                    else {
                        //昇順
                        orderQueryable=SoKoZaikoQueryable.OrderBy(sortKey);
                    }
                }
                else {
                    //２回目以降、ThenBy
                    if (descending) {
                        //降順
                        orderQueryable=((IOrderedQueryable<ZaikoListLine>)SoKoZaikoQueryable).ThenBy(sortKey + " descending");
                    }
                    else {
                        //昇順
                        orderQueryable=((IOrderedQueryable<ZaikoListLine>)SoKoZaikoQueryable).ThenBy(sortKey);
                    }
                }

                //ソート指示追加(まだ実行されない）
                SoKoZaikoQueryable = orderQueryable?.AsQueryable();

                //１回処理されれば、次回はThenByになるように
                isFirstCalled = false;
            }
            //倉庫在庫　＆　注文実績明細　ソート指示付き
            return SoKoZaikoQueryable;
        }
    }
}