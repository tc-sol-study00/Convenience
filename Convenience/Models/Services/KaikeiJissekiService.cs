using AutoMapper;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.KaikeiJisseki;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel;
using static Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel.DataAreaClass;

namespace Convenience.Models.Services {
    /// <summary>
    /// 会計実績検索サービス
    /// </summary>
    public class KaikeiJissekiService : IKaikeiJissekiService, IRetrivalService, ISharedTools {

        /// <summary>
        /// DBコンテクスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">ＤＢコンテキストＤＩ</param>
        public KaikeiJissekiService(ConvenienceContext context) {
            this._context = context;
        }
        /// <summary>
        /// 会計実績検索
        /// </summary>
        /// <param name="argKaikeiJissekiViewModel">会計実績検索ビューモデル</param>
        /// <returns>会計実績ビューモデル（検索内容含む）</returns>
        public async Task<KaikeiJissekiViewModel> KaikeiJissekiRetrival(KaikeiJissekiViewModel argKaikeiJissekiViewModel) {

            IRetrivalService t = this;

            /*
             *  会計実績のクエリを初期セット。OrderbyやWhereを追加するから、
             *  IQueryable型としている
             */
            IEnumerable<KaikeiJisseki> queriedKaikeiJisseki = _context.KaikeiJisseki.AsNoTracking()
                .Include(kj => kj.KaikeiHeader)
                .Include(kj => kj.ShohinMaster)
                .Include(kj => kj.NaigaiClassMaster)
            ;
            /*
             * 画面上の検索キーの指示を会計実績クエリに追加
             */
            queriedKaikeiJisseki = t.SearchItemRecognizer<KaikeiJisseki, KaikeiJissekiLineClass>(argKaikeiJissekiViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, queriedKaikeiJisseki);

            /*
             * クエリの結果を画面に反映する
             */
            //Mapping クエリの結果 to　会計実績ビューモデル

            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddProfile(new KaikeiJissekiPostdataToKaikeiJissekiViewModel());
            }).CreateMapper();

            //キー入力、ソート指示部分をマッピング
            KaikeiJissekiViewModel kaikeiJissekiViewModel =
                mapper.Map<KaikeiJissekiViewModel>(argKaikeiJissekiViewModel);

            //クエリ結果のマッピング

            IEnumerable<KaikeiJisseki> instanceKaikeiJisseki = await queriedKaikeiJisseki.AsQueryable().ToListAsync();
            kaikeiJissekiViewModel.DataArea.Lines =
                mapper.Map<IEnumerable<DataAreaClass.KaikeiJissekiLineClass>>(instanceKaikeiJisseki);

            /*
             *  マップされた会計実績の表情報を画面上のソート指示によりソートする
             */
            //ソートするために表情報を取り出す
            IEnumerable<DataAreaClass.KaikeiJissekiLineClass> beforeDisplayForKaikeiJissekiLines =
                kaikeiJissekiViewModel.DataArea.Lines;
            //ソートする
            IEnumerable<DataAreaClass.KaikeiJissekiLineClass> SortedKaikeiJissekiLines =
                t.SetSortKey(argKaikeiJissekiViewModel.KeywordArea.SortArea.KeyEventList, beforeDisplayForKaikeiJissekiLines);

            /*
             * 表情報をセットし返却
             */
            kaikeiJissekiViewModel.DataArea.Lines = SortedKaikeiJissekiLines;
            return kaikeiJissekiViewModel;
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
                nameof(DataAreaClass.KaikeiJissekiLineClass.ShohinId) =>
                    t.BuildComparison<T>(nameof(KaikeiJisseki.ShohinId), comparison!, rightSide!),
                nameof(DataAreaClass.KaikeiJissekiLineClass.ShohinName) =>
                    t.BuildComparison<T>
                        ($"{nameof(KaikeiJisseki.ShohinMaster)}.{nameof(KaikeiJisseki.ShohinMaster.ShohinName)}",
                        comparison!,
                        rightSide!
                    ),
                nameof(DataAreaClass.KaikeiJissekiLineClass.UriageDatetime) =>
                    t.BuildComparison<T>(nameof(KaikeiJisseki.UriageDatetime), comparison!, DateTime.Parse(rightSide!)),
                nameof(DataAreaClass.KaikeiJissekiLineClass.UriageSu) =>
                    t.BuildComparison<T>(nameof(KaikeiJisseki.UriageSu), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.KaikeiJissekiLineClass.UriageKingaku) =>
                    t.BuildComparison<T>(nameof(KaikeiJisseki.UriageKingaku), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.KaikeiJissekiLineClass.ZeikomiKingaku) =>
                    t.BuildComparison<T>(nameof(KaikeiJisseki.ZeikomiKingaku), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.KaikeiJissekiLineClass.ShohinTanka) =>
                    t.BuildComparison<T>(nameof(KaikeiJisseki.ShohinTanka), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.KaikeiJissekiLineClass.ShohiZeiritsu) =>
                    t.BuildComparison<T>(nameof(KaikeiJisseki.ShohiZeiritsu), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.KaikeiJissekiLineClass.NaigaiClass) =>
                    t.BuildComparison<T>(nameof(KaikeiJisseki.NaigaiClass), comparison!, rightSide!),
                _ => throw new Exception("検索キー指示エラー({leftSide})")
            };
            return lambda;

        }

        public Expression<Func<T, bool>>? WhereLambda2<T>(string leftSide, string comparison, string rightSide) 
            { return null; }

    }

}
