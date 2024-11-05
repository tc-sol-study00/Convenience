using AutoMapper;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.KaikeiJisseki;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel;

namespace Convenience.Models.Services {
    /// <summary>
    /// 店頭在庫検索サービス
    /// </summary>
    public class KaikeiJissekiService : IKaikeiJissekiService, ISharedTools {

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
        /// 店頭在庫検索
        /// </summary>
        /// <param name="argTentoZaikoViewModel">店頭在庫検索ビューモデル</param>
        /// <returns>店頭在庫ビューモデル（検索内容含む）</returns>
        public async Task<KaikeiJissekiViewModel> KaikeiJissekiRetrival(KaikeiJissekiViewModel argKaikeiJissekiViewModel) {

            /*
             *  会計実績のクエリを初期セット。OrderbyやWhereを追加するから、
             *  IQueryable型としている
             */
            IQueryable<KaikeiJisseki> queriedKaikeiJisseki = _context.KaikeiJisseki.AsNoTracking()
                .Include(kj => kj.KaikeiHeader)
                .Include(kj => kj.ShohinMaster)
                .Include(kj => kj.NaigaiClassMaster)
            ;
            /*
             * 画面上の検索キーの指示を店頭在庫クエリに追加
             */
            queriedKaikeiJisseki = SearchItemRecognizer(argKaikeiJissekiViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, queriedKaikeiJisseki);

            /*
             * クエリの結果を画面に反映する
             */
            //Mapping クエリの結果 to　店頭在庫ビューモデル

            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddProfile(new KaikeiJissekiPostdataToKaikeiJissekiViewModel());
            }).CreateMapper();

            //キー入力、ソート指示部分をマッピング
            KaikeiJissekiViewModel kaikeiJissekiViewModel =
                mapper.Map<KaikeiJissekiViewModel>(argKaikeiJissekiViewModel);

            //クエリ結果のマッピング

            IEnumerable<KaikeiJisseki> instanceKaikeiJisseki = await queriedKaikeiJisseki.ToListAsync();
            kaikeiJissekiViewModel.DataArea.KaikeiJissekiLines =
                mapper.Map<IEnumerable<DataAreaClass.KaikeiJissekiLineClass>>(instanceKaikeiJisseki);

            /*
             *  マップされた店頭在庫の表情報を画面上のソート指示によりソートする
             */
            //ソートするために表情報を取り出す
            IEnumerable<DataAreaClass.KaikeiJissekiLineClass> beforeDisplayForKaikeiJissekiLines =
                kaikeiJissekiViewModel.DataArea.KaikeiJissekiLines;
            //ソートする
            IEnumerable<DataAreaClass.KaikeiJissekiLineClass> SortedKaikeiJissekiLines =
                SetSortKey(argKaikeiJissekiViewModel.KeywordArea.SortArea.KeyEventList, beforeDisplayForKaikeiJissekiLines);

            /*
             * 表情報をセットし返却
             */
            kaikeiJissekiViewModel.DataArea.KaikeiJissekiLines = SortedKaikeiJissekiLines;
            return kaikeiJissekiViewModel;
        }

        /// <summary>
        /// 画面で指示されたソート指示を元に表情報に対しソートする
        /// </summary>
        /// <param name="argSortEventRec">ソート指示部</param>
        /// <param name="argTentoZaikos">ソート対象となる表示用店頭在庫データ</param>
        /// <returns></returns>
        private static IEnumerable<DataAreaClass.KaikeiJissekiLineClass> SetSortKey
            (KeywordAreaClass.SortAreaClass.SortEventRec[] argSortEventRec, IEnumerable<DataAreaClass.KaikeiJissekiLineClass> argKaikeijissekis) {

            //OrderByのときはtrue、ThenByのときはfalse
            bool IsOrderBy = true;

            //画面のソート指示行を処理する
            for (int i = 0; i < argSortEventRec.Length; i++) {

                KeywordAreaClass.SortAreaClass.SortEventRec aSortEventRec = argSortEventRec[i];
                if (ISharedTools.IsExistCheck(aSortEventRec.KeyEventData)) {     //指示がない場合があるので、チェック
                    string sortKey = aSortEventRec.KeyEventData!;
                    bool descending = aSortEventRec.Descending!;

                    //Linqの組み立て
                    if (IsOrderBy) {
                        argKaikeijissekis = argKaikeijissekis.AsQueryable().OrderBy(sortKey + (descending ? " descending" : ""));
                        IsOrderBy = false;
                    } else {
                        argKaikeijissekis = ((IOrderedQueryable<DataAreaClass.KaikeiJissekiLineClass>)argKaikeijissekis).ThenBy(sortKey + (descending ? " descending" : ""));
                    }
                }
            }

            return argKaikeijissekis;
        }

        /// <summary>
        /// 検索指示項目を認識しラムダ式を作る
        /// </summary>
        /// <param name="argSelecteWhereItemArray">検索指示項目</param>
        /// <param name="tentoZaiko">店頭在庫クエリ</param>
        /// <returns></returns>
        private static IQueryable<KaikeiJisseki> SearchItemRecognizer
            (KeywordAreaClass.KeyAreaClass.SelecteWhereItem[] argSelecteWhereItemArray, IQueryable<KaikeiJisseki> kaikeiJissekis) {

            bool needAnd = false;
            Expression<Func<KaikeiJisseki, bool>>? setExpression = default; //初期化

            //検索指示項目行を処理する
            for (int i = 0; i < argSelecteWhereItemArray.Length; i++) {
                string? leftSide, rightSide, comparison;
                //左辺側が指示されているものだけ処理
                if (ISharedTools.IsExistCheck(leftSide = argSelecteWhereItemArray[i].LeftSide)) {
                    if (ISharedTools.IsExistCheck(rightSide = argSelecteWhereItemArray[i].RightSide)) {
                        if (ISharedTools.IsExistCheck(comparison = argSelecteWhereItemArray[i].ComparisonOperator)) {
                            /* Where系ラムダ式を作る */
                            Expression<Func<KaikeiJisseki, bool>> lambda = leftSide switch {
                                nameof(DataAreaClass.KaikeiJissekiLineClass.ShohinId) => 
                                    BuildComparison<KaikeiJisseki>(nameof(KaikeiJisseki.ShohinId), comparison!, rightSide!),
                                nameof(DataAreaClass.KaikeiJissekiLineClass.ShohinName) =>
                                    BuildComparison<KaikeiJisseki>(nameof(KaikeiJisseki.ShohinMaster.ShohinName), comparison!, rightSide!),
                                nameof(DataAreaClass.KaikeiJissekiLineClass.UriageDatetime) =>
                                    BuildComparison<KaikeiJisseki>(nameof(KaikeiJisseki.UriageDatetime), comparison!, DateTime.Parse(rightSide!)),
                                nameof(DataAreaClass.KaikeiJissekiLineClass.UriageSu) =>
                                    BuildComparison<KaikeiJisseki>(nameof(KaikeiJisseki.UriageSu), comparison!, decimal.Parse(rightSide!)),
                                nameof(DataAreaClass.KaikeiJissekiLineClass.UriageKingaku) =>
                                    BuildComparison<KaikeiJisseki>(nameof(KaikeiJisseki.UriageKingaku), comparison!, decimal.Parse(rightSide!)),
                                nameof(DataAreaClass.KaikeiJissekiLineClass.ZeikomiKingaku) =>
                                    BuildComparison<KaikeiJisseki>(nameof(KaikeiJisseki.ZeikomiKingaku), comparison!, decimal.Parse(rightSide!)),
                                nameof(DataAreaClass.KaikeiJissekiLineClass.ShohinTanka) =>
                                    BuildComparison<KaikeiJisseki>(nameof(KaikeiJisseki.ShohinTanka), comparison!, decimal.Parse(rightSide!)),
                                nameof(DataAreaClass.KaikeiJissekiLineClass.ShohiZeiritsu) =>
                                    BuildComparison<KaikeiJisseki>(nameof(KaikeiJisseki.ShohiZeiritsu), comparison!, decimal.Parse(rightSide!)),
                                nameof(DataAreaClass.KaikeiJissekiLineClass.NaigaiClass) =>
                                    BuildComparison<KaikeiJisseki>(nameof(KaikeiJisseki.NaigaiClass), comparison!, rightSide!),
                                _ => throw new Exception("検索キー指示エラー({leftSide})")
                            };
                            if (needAnd) {
                                //2回目以降はAnd定義を追加
                                setExpression = CombineExpressions(setExpression!, lambda);
                            } else {
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
                kaikeiJissekis = kaikeiJissekis.Where(setExpression!);
            }
            return kaikeiJissekis;
        }

        /*
         * 複数のWhere系ラムダ式をAndで結ぶ
         */
        private static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2) {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            BinaryExpression combined = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        /*
         * Where系ラムダ式を作る
         */
        private static Expression<Func<T, bool>> BuildComparison<T>(
            string propertyName,
            string strComparisonType,
            object value
        ) {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

            Type type = value.GetType();
            ConstantExpression constant = Expression.Constant(value, type);
            Expression property = parameter;

            // ShohinMaster.ShohinNameの指定を認識させる
            foreach (string member in propertyName.Split('.')) {
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
                System.Reflection.MethodInfo? compareMethod = typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) });
                MethodCallExpression compareExpression = Expression.Call(compareMethod!, property, constant);

                comparison = comparisonType switch {
                    KeywordAreaClass.KeyAreaClass.Comparisons.Equal => Expression.Equal(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.NotEqual => Expression.NotEqual(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.GreaterThanOrEqual => Expression.GreaterThanOrEqual(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.GreaterThan => Expression.GreaterThan(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.LessThanOrEqual => Expression.LessThanOrEqual(compareExpression, Expression.Constant(0)),
                    KeywordAreaClass.KeyAreaClass.Comparisons.LessThan => Expression.LessThan(compareExpression, Expression.Constant(0)),
                    _ => throw new NotSupportedException($"Comparison type {comparisonType} is not supported.")
                };
            } else {
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
