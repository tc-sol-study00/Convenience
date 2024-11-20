using AutoMapper;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.TentoZaiko;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel;
using static Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel.DataAreaClass;

namespace Convenience.Models.Services {
    /// <summary>
    /// 店頭在庫検索サービス
    /// </summary>
    public class TentoZaikoService : ITentoZaikoService, IRetrivalService, ITotalSummaryRetrival,ISharedTools {

        /// <summary>
        /// DBコンテクスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">ＤＢコンテキストＤＩ</param>
        public TentoZaikoService(ConvenienceContext context) {
            this._context = context;
        }
        /// <summary>
        /// 店頭在庫検索
        /// </summary>
        /// <param name="argTentoZaikoViewModel">店頭在庫検索ビューモデル</param>
        /// <returns>店頭在庫ビューモデル（検索内容含む）</returns>
        public async Task<TentoZaikoViewModel> TentoZaikoRetrival(TentoZaikoViewModel argTentoZaikoViewModel) {

            IRetrivalService t = this;

            /*
             *  店頭在庫のクエリを初期セット。OrderbyやWhereを追加するから、
             *  IQueryable型としている
             */
            IEnumerable<TentoZaiko> queriedTentoZaiko = _context.TentoZaiko.AsNoTracking()
                .Include(tz => tz.ShohinMaster)
            ;
            /*
             * 画面上の検索キーの指示を店頭在庫クエリに追加
             */
            queriedTentoZaiko = t.SearchItemRecognizer<TentoZaiko, TentoZaIkoLine>(argTentoZaikoViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, queriedTentoZaiko);

            /*
             * クエリの結果を画面に反映する
             */
            //Mapping クエリの結果 to　店頭在庫ビューモデル

            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddProfile(new TentoZaikoPostdataToTentoZaikoViewModel());
            }).CreateMapper();

            //キー入力、ソート指示部分をマッピング
            TentoZaikoViewModel tentoZaikoViewModel =
                mapper.Map<TentoZaikoViewModel>(argTentoZaikoViewModel);

            //クエリ結果のマッピング

            IEnumerable<TentoZaiko> instanceTentoZaiko = await queriedTentoZaiko.AsQueryable().ToListAsync();
            tentoZaikoViewModel.DataArea.Lines =
                mapper.Map<IEnumerable<DataAreaClass.TentoZaIkoLine>>(instanceTentoZaiko);

            /*
             *  マップされた店頭在庫の表情報を画面上のソート指示によりソートする
             */
            //ソートするために表情報を取り出す
            IEnumerable<DataAreaClass.TentoZaIkoLine> beforeDisplayForTentoZaIkoLines =
                tentoZaikoViewModel.DataArea.Lines;
            //ソートする
            IEnumerable<DataAreaClass.TentoZaIkoLine> SortedTentoZaikoLines =
                t.SetSortKey(argTentoZaikoViewModel.KeywordArea.SortArea.KeyEventList, beforeDisplayForTentoZaIkoLines);

            /*
             * 総合計
             */
            TentoZaIkoLine summaryTentoZaIkoLine = new TentoZaIkoLine();
            summaryTentoZaIkoLine =
                ((ITotalSummaryRetrival)this).TotalSummary(SortedTentoZaikoLines, summaryTentoZaIkoLine, nameof(summaryTentoZaIkoLine.ShohinName));

            /*
             * 表情報をセットし返却
             */
            tentoZaikoViewModel.DataArea.Lines = SortedTentoZaikoLines;
            tentoZaikoViewModel.DataArea.SummaryLine = summaryTentoZaIkoLine;
            return tentoZaikoViewModel;
        }

        /// <summary>
        /// 注文実績明細に対する条件ラムダ式
        /// </summary>
        /// <param name="leftSide">左辺</param>
        /// <param name="comparison">比較演算子</param>
        /// <param name="rightSide">右辺</param>
        /// <returns>ラムダ式</returns>
        public Expression<Func<T, bool>>? WhereLambda1<T>(string leftSide, string comparison, string rightSide) {

            IRetrivalService t = this;

            Expression<Func<T, bool>> lambda = leftSide switch {
                nameof(DataAreaClass.TentoZaIkoLine.ShohinId) =>
                    t.BuildComparison<T>(nameof(TentoZaiko.ShohinId), comparison!, rightSide!),
                nameof(DataAreaClass.TentoZaIkoLine.ShohinName) =>
                    t.BuildComparison<T>
                        ($"{nameof(TentoZaiko.ShohinMaster)}.{nameof(TentoZaiko.ShohinMaster.ShohinName)}",
                        comparison!,
                        rightSide!
                    ),
                nameof(DataAreaClass.TentoZaIkoLine.ZaikoSu) =>
                    t.BuildComparison<T>(nameof(TentoZaiko.ZaikoSu), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.TentoZaIkoLine.LastShireDateTime) =>
                    t.BuildComparison<T>(nameof(TentoZaiko.LastShireDateTime), comparison!, DateOnly.Parse(rightSide!)),
                nameof(DataAreaClass.TentoZaIkoLine.LastHaraidashiDate) =>
                    t.BuildComparison<T>(nameof(TentoZaiko.LastHaraidashiDate), comparison!, DateTime.Parse(rightSide!)),
                nameof(DataAreaClass.TentoZaIkoLine.LastUriageDatetime) =>
                    t.BuildComparison<T>(nameof(TentoZaiko.LastUriageDatetime), comparison!, DateTime.Parse(rightSide!)),
                _ => throw new Exception("検索キー指示エラー({leftSide})")
            };

            return lambda;
        }

        public Expression<Func<T, bool>>? WhereLambda2<T>(string leftSide, string comparison, string rightSide) {
            return null;
        }
    }
}
