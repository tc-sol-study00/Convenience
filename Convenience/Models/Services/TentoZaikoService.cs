using AutoMapper;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.TentoZaiko;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel;

namespace Convenience.Models.Services {
    /// <summary>
    /// 店頭在庫検索サービス
    /// </summary>
    public class TentoZaikoService : ITentoZaikoService, ISharedTools {

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

            /*
             *  店頭在庫のクエリを初期セット。OrderbyやWhereを追加するから、
             *  IQueryable型としている
             */
            IQueryable<TentoZaiko> queriedTentoZaiko = _context.TentoZaiko.AsNoTracking()
                .Include(tz => tz.ShohinMaster)
            ;
            /*
             * 画面上の検索キーの指示を店頭在庫クエリに追加
             */
            queriedTentoZaiko = SearchItemRecognizer(argTentoZaikoViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, queriedTentoZaiko);

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

            IEnumerable<TentoZaiko> instanceTentoZaiko = await queriedTentoZaiko.ToListAsync();
            tentoZaikoViewModel.DataArea.TentoZaIkoLines =
                mapper.Map<IEnumerable<DataAreaClass.TentoZaIkoLine>>(instanceTentoZaiko);

            /*
             *  マップされた店頭在庫の表情報を画面上のソート指示によりソートする
             */
            //ソートするために表情報を取り出す
            IEnumerable<DataAreaClass.TentoZaIkoLine> beforeDisplayForTentoZaIkoLines =
                tentoZaikoViewModel.DataArea.TentoZaIkoLines;
            //ソートする
            IEnumerable<DataAreaClass.TentoZaIkoLine> SortedTentoZaikoLines =
                SetSortKey(argTentoZaikoViewModel.KeywordArea.SortArea.KeyEventList, beforeDisplayForTentoZaIkoLines);

            /*
             * 表情報をセットし返却
             */
            tentoZaikoViewModel.DataArea.TentoZaIkoLines = SortedTentoZaikoLines;
            return tentoZaikoViewModel;
        }

        /// <summary>
        /// 画面で指示されたソート指示を元に表情報に対しソートする
        /// </summary>
        /// <param name="argSortEventRec">ソート指示部</param>
        /// <param name="argTentoZaikos">ソート対象となる表示用店頭在庫データ</param>
        /// <returns></returns>
        private IEnumerable<DataAreaClass.TentoZaIkoLine> SetSortKey
            (KeywordAreaClass.SortAreaClass.SortEventRec[] argSortEventRec, IEnumerable<DataAreaClass.TentoZaIkoLine> argTentoZaikos) {

            //OrderByのときはtrue、ThenByのときはfalse
            bool IsOrderBy = true;

            //画面のソート指示行を処理する
            for (int i = 0; i < argSortEventRec.Length; i++) {

                var aSortEventRec = argSortEventRec[i];
                if (ISharedTools.IsExistCheck(aSortEventRec.KeyEventData)) {     //指示がない場合があるので、チェック
                    string sortKey = aSortEventRec.KeyEventData!;
                    bool descending = aSortEventRec.Descending!;

                    //Linqの組み立て
                    if (IsOrderBy) {
                        argTentoZaikos = argTentoZaikos.AsQueryable().OrderBy(sortKey + (descending ? " descending" : ""));
                        IsOrderBy = false;
                    }
                    else {
                        argTentoZaikos = ((IOrderedQueryable<DataAreaClass.TentoZaIkoLine>)argTentoZaikos).ThenBy(sortKey + (descending ? " descending" : ""));
                    }
                }
            }

            return argTentoZaikos;
        }

        /// <summary>
        /// 検索指示項目を認識しラムダ式を作る
        /// </summary>
        /// <param name="argSelecteWhereItemArray">検索指示項目</param>
        /// <param name="tentoZaiko">店頭在庫クエリ</param>
        /// <returns></returns>
        private IQueryable<TentoZaiko> SearchItemRecognizer
            (KeywordAreaClass.KeyAreaClass.SelecteWhereItem[] argSelecteWhereItemArray, IQueryable<TentoZaiko> tentoZaiko) {

            bool needAnd = false;
            Expression<Func<TentoZaiko, bool>>? setExpression = default; //初期化

            //検索指示項目行を処理する
            for (var i = 0; i < argSelecteWhereItemArray.Length; i++) {
                string? leftSide, rightSide, comparison;
                //左辺側が指示されているものだけ処理
                if (ISharedTools.IsExistCheck(leftSide = argSelecteWhereItemArray[i].LeftSide)) {
                    if (ISharedTools.IsExistCheck(rightSide = argSelecteWhereItemArray[i].RightSide)) {
                        if (ISharedTools.IsExistCheck(comparison = argSelecteWhereItemArray[i].ComparisonOperator)) {
                            /* Where系ラムダ式を作る */
                            Expression<Func<TentoZaiko, bool>> lambda = leftSide switch {
                                nameof(DataAreaClass.TentoZaIkoLine.ShohinId) =>BuildComparison<TentoZaiko>(nameof(TentoZaiko.ShohinId), comparison!, rightSide!),
                                nameof(DataAreaClass.TentoZaIkoLine.ShohinName) =>
                                    BuildComparison<TentoZaiko>
                                        ($"{nameof(TentoZaiko.ShohinMaster)}.{nameof(TentoZaiko.ShohinMaster.ShohinName)}",
                                        comparison!,
                                        rightSide!
                                    ),
                                nameof(DataAreaClass.TentoZaIkoLine.ZaikoSu) =>
                                    BuildComparison<TentoZaiko>(nameof(TentoZaiko.ZaikoSu), comparison!, decimal.Parse(rightSide!)),
                                nameof(DataAreaClass.TentoZaIkoLine.LastShireDateTime) =>
                                    BuildComparison<TentoZaiko>(nameof(TentoZaiko.LastShireDateTime), comparison!, DateOnly.Parse(rightSide!)),
                                nameof(DataAreaClass.TentoZaIkoLine.LastHaraidashiDate) =>
                                    BuildComparison<TentoZaiko>(nameof(TentoZaiko.LastHaraidashiDate), comparison!, DateTime.Parse(rightSide!)),
                                nameof(DataAreaClass.TentoZaIkoLine.LastUriageDatetime) =>
                                    BuildComparison<TentoZaiko>(nameof(TentoZaiko.LastUriageDatetime), comparison!, DateTime.Parse(rightSide!)),
                                _ => throw new ArgumentOutOfRangeException("検索キー指示エラー({leftSide})")
                            };
                            if(needAnd){
                                //2回目以降はAnd定義を追加
                                setExpression = CombineExpressions(setExpression, lambda);
                            }
                            else {
                                //初回はセットのみ
                                setExpression = lambda;
                                needAnd = true;
                            }
                        }
                    }
                }
            }

            /*
             *    店頭在庫クエリにWhere文追加
             */
            if (ISharedTools.IsExistCheck(setExpression)) {
                tentoZaiko = tentoZaiko.Where(setExpression!);
            }
            return tentoZaiko;
        }

        /*
         * 複数のWhere系ラムダ式をAndで結ぶ
         */
        private Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2) {
            var parameter = Expression.Parameter(typeof(T), "x");
            var combined = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        /*
         * Where系ラムダ式を作る
         */
        private Expression<Func<T, bool>> BuildComparison<T>(
            string propertyName,
            string strComparisonType,
            object value
        ) {
            var parameter = Expression.Parameter(typeof(T), "x");

            Type type = value.GetType();
            var constant = Expression.Constant(value, type);
            Expression property = parameter;

            // ShohinMaster.ShohinNameの指定を認識させる
            foreach (var member in propertyName.Split('.')) {
                property = Expression.Property(property, member);
            }

            // Nullable型の場合、比較のために型を一致させる
            if (Nullable.GetUnderlyingType(property.Type) != null) {
                property = Expression.Convert(property, type);
            }

            // 比較演算子のenumの値に変換
            KeywordAreaClass.KeyAreaClass.Comparisons comparisonType = (KeywordAreaClass.KeyAreaClass.Comparisons)Enum.Parse(typeof(KeywordAreaClass.KeyAreaClass.Comparisons), strComparisonType);

            // 比較演算子をラムダ式に組み込む
            Expression comparison;
            if (property.Type == typeof(string)) {
                // 文字列の比較の場合、String.Compareを使用する
                var compareMethod = typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) });
                var compareExpression = Expression.Call(compareMethod, property, constant);

                comparison = comparisonType switch {
                    KeywordAreaClass.KeyAreaClass.Comparisons.Equal => Expression.Equal(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.NotEqual => Expression.NotEqual(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.GreaterThanOrEqual => Expression.GreaterThanOrEqual(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.GreaterThan => Expression.GreaterThan(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.LessThanOrEqual => Expression.LessThanOrEqual(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.LessThan => Expression.LessThan(compareExpression, Expression.Constant(0)),
                    _ => throw new NotSupportedException($"Comparison type {comparisonType} is not supported.")
                };
            }
            else {
                // 非文字列の場合、通常の比較演算を使用する
                comparison = comparisonType switch {
                    KeywordAreaClass.KeyAreaClass.Comparisons.Equal => Expression.Equal(property, constant),
                    KeywordAreaClass.KeyAreaClass.Comparisons.NotEqual => Expression.NotEqual(property, constant),
                    KeywordAreaClass.KeyAreaClass.Comparisons.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
                    KeywordAreaClass.KeyAreaClass.Comparisons.GreaterThan => Expression.GreaterThan(property, constant),
                    KeywordAreaClass.KeyAreaClass.Comparisons.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
                    KeywordAreaClass.KeyAreaClass.Comparisons.LessThan => Expression.LessThan(property, constant),
                    _ => throw new NotSupportedException($"Comparison type {comparisonType} is not supported.")
                };
            }

            // ラムダ式の作成
            return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        }


    }

}
