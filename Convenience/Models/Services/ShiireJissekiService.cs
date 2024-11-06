using AutoMapper;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.ShiireJisseki;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel;

namespace Convenience.Models.Services {
    /// <summary>
    /// 仕入実績検索サービス
    /// </summary>
    public class ShiireJissekiService : IShiireJissekiService, ISharedTools {

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

            /*
             *  仕入実績のクエリを初期セット。OrderbyやWhereを追加するから、
             *  IQueryable型としている
             */
            IQueryable<ShiireJisseki> queried = 
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
            queried = SearchItemRecognizer(argShiireJissekiViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, queried);

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

            IEnumerable<ShiireJisseki> instanceShiireJisseki = await queried.ToListAsync();
            ShiireJissekiViewModel.DataArea.ShiireJissekiLines =
                mapper.Map<IEnumerable<DataAreaClass.ShiireJissekiLineClass>>(instanceShiireJisseki);

            /*
             *  マップされた仕入実績の表情報を画面上のソート指示によりソートする
             */
            //ソートするために表情報を取り出す
            IEnumerable<DataAreaClass.ShiireJissekiLineClass> beforeDisplayForShiireJissekiLines =
                ShiireJissekiViewModel.DataArea.ShiireJissekiLines;
            //ソートする
            IEnumerable<DataAreaClass.ShiireJissekiLineClass> SortedShiireJissekiLines =
                SetSortKey(argShiireJissekiViewModel.KeywordArea.SortArea.KeyEventList, beforeDisplayForShiireJissekiLines);

            /*
             * 表情報をセットし返却
             */
            ShiireJissekiViewModel.DataArea.ShiireJissekiLines = SortedShiireJissekiLines;
            return ShiireJissekiViewModel;
        }

        /// <summary>
        /// 画面で指示されたソート指示を元に表情報に対しソートする
        /// </summary>
        /// <param name="argSortEventRec">ソート指示部</param>
        /// <param name="argShiireJissekis">ソート対象となる表示用仕入実績データ</param>
        /// <returns></returns>
        private static IEnumerable<DataAreaClass.ShiireJissekiLineClass> SetSortKey
            (KeywordAreaClass.SortAreaClass.SortEventRec[] argSortEventRec, IEnumerable<DataAreaClass.ShiireJissekiLineClass> argShiireJissekis) {

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
                        argShiireJissekis = argShiireJissekis.AsQueryable().OrderBy(sortKey + (descending ? " descending" : ""));
                        IsOrderBy = false;
                    } else {
                        argShiireJissekis = ((IOrderedQueryable<DataAreaClass.ShiireJissekiLineClass>)argShiireJissekis).ThenBy(sortKey + (descending ? " descending" : ""));
                    }
                }
            }

            return argShiireJissekis;
        }

        /// <summary>
        /// 検索指示項目を認識しラムダ式を作る
        /// </summary>
        /// <param name="argSelecteWhereItemArray">検索指示項目</param>
        /// <param name="tentoZaiko">仕入実績クエリ</param>
        /// <returns></returns>
        private static IQueryable<ShiireJisseki> SearchItemRecognizer
            (KeywordAreaClass.KeyAreaClass.SelecteWhereItem[] argSelecteWhereItemArray, IQueryable<ShiireJisseki> s) {

            bool needAnd = false;
            Expression<Func<ShiireJisseki, bool>>? setExpression = default; //初期化


            const string shiireSakiKaisyaPath = 
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii)}."+
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster)}."+
                $"{nameof(ShiireJisseki.ChumonJissekiMeisaii.ShiireMaster.ShiireSakiMaster)}."+
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

            //検索指示項目行を処理する
            for (int i = 0; i < argSelecteWhereItemArray.Length; i++) {
                string? leftSide, rightSide, comparison;
                //左辺側が指示されているものだけ処理
                if (ISharedTools.IsExistCheck(leftSide = argSelecteWhereItemArray[i].LeftSide)) {
                    if (ISharedTools.IsExistCheck(rightSide = argSelecteWhereItemArray[i].RightSide)) {
                        if (ISharedTools.IsExistCheck(comparison = argSelecteWhereItemArray[i].ComparisonOperator)) {
                            /* Where系ラムダ式を作る */
                            Expression<Func<ShiireJisseki, bool>> lambda = leftSide switch {
                                nameof(DataAreaClass.ShiireJissekiLineClass.ChumonId) =>
                                    BuildComparison<ShiireJisseki>(nameof(ShiireJisseki.ChumonId), comparison!, rightSide!),
                                nameof(DataAreaClass.ShiireJissekiLineClass.ShiireDate) =>
                                    BuildComparison<ShiireJisseki>(nameof(ShiireJisseki.ShiireDate), comparison!, rightSide!),
                                nameof(DataAreaClass.ShiireJissekiLineClass.SeqByShiireDate) =>
                                    BuildComparison<ShiireJisseki>(nameof(ShiireJisseki.SeqByShiireDate), comparison!, uint.Parse(rightSide!)),
                                nameof(DataAreaClass.ShiireJissekiLineClass.ShiireDateTime) =>
                                    BuildComparison<ShiireJisseki>(nameof(ShiireJisseki.ShiireDateTime), comparison!, DateTime.Parse(rightSide!)),
                                nameof(DataAreaClass.ShiireJissekiLineClass.ShiireSakiId) =>
                                    BuildComparison<ShiireJisseki>(nameof(ShiireJisseki.ShiireSakiId), comparison!, rightSide!),
                                nameof(DataAreaClass.ShiireJissekiLineClass.ShiirePrdId) =>
                                BuildComparison<ShiireJisseki>(nameof(ShiireJisseki.ShiirePrdId), comparison!, rightSide!),
                                nameof(DataAreaClass.ShiireJissekiLineClass.ShohinId) =>
                                    BuildComparison<ShiireJisseki>(nameof(ShiireJisseki.ShohinId), comparison!, rightSide!),
                                nameof(DataAreaClass.ShiireJissekiLineClass.NonyuSu) =>
                                    BuildComparison<ShiireJisseki>(nameof(ShiireJisseki.NonyuSu), comparison!, decimal.Parse(rightSide!)),
                                nameof(DataAreaClass.ShiireJissekiLineClass.ShiireSakiKaisya) =>
                                BuildComparison<ShiireJisseki>(shiireSakiKaisyaPath, comparison!, rightSide!),
                                nameof(DataAreaClass.ShiireJissekiLineClass.ShiirePrdName) =>
                                    BuildComparison<ShiireJisseki>(shiirePrdNamePath, comparison!, rightSide!),
                                nameof(DataAreaClass.ShiireJissekiLineClass.ShohinName) =>
                                    BuildComparison<ShiireJisseki>(shohinNamePath, comparison!, rightSide!),
                                _ => throw new Exception("検索キー指示エラー({leftSide})")
                            };; 
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
             *    仕入実績クエリにWhere文追加
             */
            if (ISharedTools.IsExistCheck(setExpression)) {
                s = s.Where(setExpression!);
            }
            return s;
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
