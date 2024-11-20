using AutoMapper;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.ChumonJisseki;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel;
using static Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass;

namespace Convenience.Models.Services {
    /// <summary>
    /// 注文実績検索サービス
    /// </summary>
    public class ChumonJissekiService : IChumonJissekiService, IRetrivalService, ITotalSummaryRetrival,ISharedTools {

        /// <summary>
        /// DBコンテクスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">ＤＢコンテキストＤＩ</param>
        public ChumonJissekiService(ConvenienceContext context) {
            this._context = context;
        }
        /// <summary>
        /// 注文実績検索
        /// </summary>
        /// <param name="argChumonJissekiViewModel">注文実績検索ビューモデル</param>
        /// <returns>注文実績ビューモデル（検索内容含む）</returns>
        public async Task<ChumonJissekiViewModel> ChumonJissekiRetrival(ChumonJissekiViewModel argChumonJissekiViewModel) {

            IRetrivalService t = this;

            /*
             *  注文実績のクエリを初期セット。OrderbyやWhereを追加するから、
             *  IQueryable型としている
             */
            IEnumerable<ChumonJissekiMeisai> queriedMeisai =
                _context.ChumonJissekiMeisai.AsNoTracking()
                    .Include(cjm => cjm.ChumonJisseki)
                    .Include(cjm => cjm.ShiireMaster)
                        .ThenInclude(sm => sm!.ShohinMaster)
                    .Include(cjm => cjm.ShiireMaster)
                        .ThenInclude(sm => sm!.ShiireSakiMaster)
            ;
            /*
             * 画面上の検索キーの指示を注文実績クエリに追加（DBにある項目だけ）
             */
            queriedMeisai = t.SearchItemRecognizer<ChumonJissekiMeisai, ChumonJissekiLineClass>
                (argChumonJissekiViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, queriedMeisai);

            /*
             * クエリの結果を画面に反映する
             */
            //Mapping クエリの結果 to　注文実績ビューモデル

            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddProfile(new ChumonJissekiPostdataToChumonJissekiViewModel());
            }).CreateMapper();

            //キー入力、ソート指示部分をマッピング
            ChumonJissekiViewModel ChumonJissekiViewModel =
                mapper.Map<ChumonJissekiViewModel>(argChumonJissekiViewModel);

            //クエリ結果のマッピング

            IEnumerable<ChumonJissekiMeisai> instanceChumonJisseki = await queriedMeisai.AsQueryable().ToListAsync();

            IEnumerable<ChumonJissekiLineClass> chumonJissekiLines =
               mapper.Map<IEnumerable<DataAreaClass.ChumonJissekiLineClass>>(instanceChumonJisseki);

            /*
             * 画面上の検索キーの指示を注文実績クエリに追加（DBにない項目だけ）
             */
            chumonJissekiLines = t.SearchItemRecognizer<ChumonJissekiLineClass,ChumonJissekiLineClass>
                (argChumonJissekiViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, chumonJissekiLines);
            ChumonJissekiViewModel.DataArea.Lines = chumonJissekiLines;


            /*
             *  マップされた注文実績の表情報を画面上のソート指示によりソートする
             */
            //ソートするために表情報を取り出す
            IEnumerable<DataAreaClass.ChumonJissekiLineClass> beforeDisplayForChumonJissekiLines =
                ChumonJissekiViewModel.DataArea.Lines;

            //ソートする
            IEnumerable<DataAreaClass.ChumonJissekiLineClass> SortedChumonJissekiLines =
                t.SetSortKey(argChumonJissekiViewModel.KeywordArea.SortArea.KeyEventList, beforeDisplayForChumonJissekiLines);

            /*
             * 総合計
             */
            ChumonJissekiLineClass summaryChumonJissekiLine = new ChumonJissekiLineClass();
            summaryChumonJissekiLine =
                ((ITotalSummaryRetrival)this).TotalSummary(SortedChumonJissekiLines, summaryChumonJissekiLine, nameof(summaryChumonJissekiLine.ShohinName));

            /*
             * 表情報をセットし返却
             */
            ChumonJissekiViewModel.DataArea.Lines = SortedChumonJissekiLines;
            ChumonJissekiViewModel.DataArea.SummaryLine = summaryChumonJissekiLine;
            return ChumonJissekiViewModel;
        }

        /// <summary>
        /// 注文実績明細に対する条件ラムダ式
        /// </summary>
        /// <param name="leftSide">左辺</param>
        /// <param name="comparison">比較演算子</param>
        /// <param name="rightSide">右辺</param>
        /// <returns>ラムダ式</returns>
        public Expression<Func<T, bool>>? WhereLambda1<T>(string leftSide, string comparison, string rightSide) {
            
            IRetrivalService t =this;

            string shiireSakiKaisya =
                $"{nameof(ChumonJissekiMeisai.ShiireMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShiireSakiMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShiireSakiMaster.ShiireSakiKaisya)}";

            string shiirePrdName =
                $"{nameof(ChumonJissekiMeisai.ShiireMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShiirePrdName)}";

            string shohinName =
                $"{nameof(ChumonJissekiMeisai.ShiireMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShohinMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShohinMaster.ShohinName)}";

            string chumonDate =
                $"{nameof(ChumonJissekiMeisai.ChumonJisseki)}." +
                $"{nameof(ChumonJissekiMeisai.ChumonJisseki.ChumonDate)}";

            Expression<Func<T, bool>>? lambda = leftSide switch {
                nameof(DataAreaClass.ChumonJissekiLineClass.ChumonId) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiMeisai.ChumonId), comparison!, rightSide!),
                nameof(DataAreaClass.ChumonJissekiLineClass.ChumonDate) =>
                    t.BuildComparison<T>(chumonDate, comparison!, DateOnly.Parse(rightSide!)),
                nameof(DataAreaClass.ChumonJissekiLineClass.ShiireSakiId) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiMeisai.ShiireSakiId), comparison!, rightSide!),
                nameof(DataAreaClass.ChumonJissekiLineClass.ShiirePrdId) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiMeisai.ShiirePrdId), comparison!, rightSide!),
                nameof(DataAreaClass.ChumonJissekiLineClass.ShohinId) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiMeisai.ShohinId), comparison!, rightSide!),
                nameof(DataAreaClass.ChumonJissekiLineClass.ChumonSu) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiMeisai.ChumonSu), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.ChumonJissekiLineClass.ChumonZan) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiMeisai.ChumonZan), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.ChumonJissekiLineClass.ShiireSakiKaisya) =>
                    t.BuildComparison<T>(shiireSakiKaisya, comparison!, rightSide!),
                nameof(DataAreaClass.ChumonJissekiLineClass.ShiirePrdName) =>
                    t.BuildComparison<T>(shiirePrdName, comparison!, rightSide!),
                nameof(DataAreaClass.ChumonJissekiLineClass.ShohinName) =>
                    t.BuildComparison<T>(shohinName, comparison!, rightSide!),
                _ => null
            };
            return lambda;

        }
        public Expression<Func<T, bool>>? WhereLambda2<T>(string leftSide, string comparison, string rightSide) {

            IRetrivalService t = (IRetrivalService)this;

            Expression<Func<T, bool>>? lambda = leftSide switch {
                nameof(DataAreaClass.ChumonJissekiLineClass.ShiireZumiSu) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiLineClass.ShiireZumiSu), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.ChumonJissekiLineClass.ChumonKingaku) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiLineClass.ChumonKingaku), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.ChumonJissekiLineClass.ShiireZumiKingaku) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiLineClass.ShiireZumiKingaku), comparison!, decimal.Parse(rightSide!)),
                nameof(DataAreaClass.ChumonJissekiLineClass.ChumonZanKingaku) =>
                    t.BuildComparison<T>(nameof(ChumonJissekiLineClass.ChumonZanKingaku), comparison!, decimal.Parse(rightSide!)),
                _ => null
            };
            return lambda;
        }

    }

}
