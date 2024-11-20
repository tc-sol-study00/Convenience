using AutoMapper;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.ShiireJisseki;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel;
using static Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel.DataAreaClass;

namespace Convenience.Models.Services {
    /// <summary>
    /// 仕入実績検索サービス
    /// </summary>
    public class ShiireJissekiService : IShiireJissekiService, IRetrivalService, ITotalSummaryRetrival, ISharedTools {

        /// <summary>
        /// DBコンテクスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">ＤＢコンテキストＤＩ</param>
        public ShiireJissekiService(ConvenienceContext context) {
            this._context = context;
        }
        /// <summary>
        /// 仕入実績検索
        /// </summary>
        /// <param name="argShiireJissekiViewModel">仕入実績検索ビューモデル</param>
        /// <returns>仕入実績ビューモデル（検索内容含む）</returns>
        public async Task<ShiireJissekiViewModel> ShiireJissekiRetrival(ShiireJissekiViewModel argShiireJissekiViewModel) {

            IRetrivalService t = this;

            /*
             *  仕入実績のクエリを初期セット。OrderbyやWhereを追加するから、
             *  IQueryable型としている
             */
            IEnumerable<ShiireJisseki> queried = 
                _context.ShiireJisseki.AsNoTracking()
                    .Include(sj => sj.ChumonJissekiMeisaii)
                    .ThenInclude(cm => cm!.ShiireMaster)
                    .ThenInclude(sm => sm!.ShiireSakiMaster)
                    .Include(sj => sj.ChumonJissekiMeisaii)
                    .ThenInclude(cm => cm!.ShiireMaster)
                    .ThenInclude(sm => sm!.ShohinMaster)
            ;
            /*
             * 画面上の検索キーの指示を仕入実績クエリに追加
             */
            queried = t.SearchItemRecognizer<ShiireJisseki,ShiireJissekiLineClass>(argShiireJissekiViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, queried);

            /*
             * クエリの結果を画面に反映する
             */
            //Mapping クエリの結果 to　仕入実績ビューモデル

            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddProfile(new ShiireJissekiPostdataToShiireJissekiViewModel());
            }).CreateMapper();

            //キー入力、ソート指示部分をマッピング
            ShiireJissekiViewModel ShiireJissekiViewModel =
                mapper.Map<ShiireJissekiViewModel>(argShiireJissekiViewModel);

            //クエリ結果のマッピング

            IEnumerable<ShiireJisseki> instanceShiireJisseki = await queried.AsQueryable().ToListAsync();
            ShiireJissekiViewModel.DataArea.Lines =
                mapper.Map<IEnumerable<DataAreaClass.ShiireJissekiLineClass>>(instanceShiireJisseki);

            /*
             *  マップされた仕入実績の表情報を画面上のソート指示によりソートする
             */
            //ソートするために表情報を取り出す
            IEnumerable<DataAreaClass.ShiireJissekiLineClass> beforeDisplayForShiireJissekiLines =
                ShiireJissekiViewModel.DataArea.Lines;
            //ソートする
            IEnumerable<DataAreaClass.ShiireJissekiLineClass> SortedShiireJissekiLines =
                t.SetSortKey(argShiireJissekiViewModel.KeywordArea.SortArea.KeyEventList, beforeDisplayForShiireJissekiLines);

            /*
             * 総合計
             */
            ShiireJissekiLineClass summaryShiireJissekiLine = new ShiireJissekiLineClass();
            summaryShiireJissekiLine =
                ((ITotalSummaryRetrival)this).TotalSummary(SortedShiireJissekiLines, summaryShiireJissekiLine, nameof(summaryShiireJissekiLine.ShohinName));

            /*
             * 表情報をセットし返却
             */
            ShiireJissekiViewModel.DataArea.Lines = SortedShiireJissekiLines;
            ShiireJissekiViewModel.DataArea.SummaryLine = summaryShiireJissekiLine;
            return ShiireJissekiViewModel;
        }

        /// <summary>
        /// 仕入実績に対する条件ラムダ式
        /// </summary>
        /// <param name="leftSide">左辺</param>
        /// <param name="comparison">比較演算子</param>
        /// <param name="rightSide">右辺</param>
        /// <returns>ラムダ式</returns>
        public Expression<Func<T, bool>>? WhereLambda1<T>(string leftSide, string comparison, string rightSide) {
            IRetrivalService t = this;

            const string shiireSakiKaisyaPath =
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii)}." +
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster)}." +
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster.ShiireSakiMaster)}." +
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster.ShiireSakiMaster.ShiireSakiKaisya)}";

            const string shiirePrdNamePath =
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii)}." +
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster)}." +
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster.ShiirePrdName)}";


            const string shohinNamePath =
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii)}." +
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster)}." +
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster.ShohinMaster)}." +
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster.ShohinMaster.ShohinName)}";
            
            Expression<Func<T, bool>> lambda = leftSide switch {
                nameof(DataAreaClass.ShiireJissekiLineClass.ChumonId) =>
                    t.BuildComparison<T>(nameof(ShiireJisseki.ChumonId), comparison!, rightSide!),
                nameof(DataAreaClass.ShiireJissekiLineClass.ShiireDate) =>
                    t.BuildComparison<T>(nameof(ShiireJisseki.ShiireDate), comparison!, DateOnly.Parse(rightSide!)),
                nameof(DataAreaClass.ShiireJissekiLineClass.SeqByShiireDate) =>
                    t.BuildComparison<T>(nameof(ShiireJisseki.SeqByShiireDate), comparison!, uint.Parse(rightSide!)),
                nameof(DataAreaClass.ShiireJissekiLineClass.ShiireDateTime) =>
                    t.BuildComparison<T>(nameof(ShiireJisseki.ShiireDateTime), comparison!, DateTime.Parse(rightSide!)),
                nameof(DataAreaClass.ShiireJissekiLineClass.ShiireSakiId) =>
                    t.BuildComparison<T>(nameof(ShiireJisseki.ShiireSakiId), comparison!, rightSide!),
                nameof(DataAreaClass.ShiireJissekiLineClass.ShiirePrdId) =>
                    t.BuildComparison<T>(nameof(ShiireJisseki.ShiirePrdId), comparison!, rightSide!),
                nameof(DataAreaClass.ShiireJissekiLineClass.ShohinId) =>
                    t.BuildComparison<T>(nameof(ShiireJisseki.ShohinId), comparison!, rightSide!),
                nameof(DataAreaClass.ShiireJissekiLineClass.NonyuSu) =>
                    t.BuildComparison<T>(nameof(ShiireJisseki.NonyuSu), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.ShiireJissekiLineClass.ShiireSakiKaisya) =>
                t.BuildComparison<T>(shiireSakiKaisyaPath, comparison!, rightSide!),
                nameof(DataAreaClass.ShiireJissekiLineClass.ShiirePrdName) =>
                    t.BuildComparison<T>(shiirePrdNamePath, comparison!, rightSide!),
                nameof(DataAreaClass.ShiireJissekiLineClass.ShohinName) =>
                    t.BuildComparison<T>(shohinNamePath, comparison!, rightSide!),
                _ => throw new Exception("検索キー指示エラー({leftSide})")
            };

            return lambda;

        }

        public Expression<Func<T, bool>>? WhereLambda2<T>(string leftSide, string comparison, string rightSide) {
            return null;
        }

    }

}
