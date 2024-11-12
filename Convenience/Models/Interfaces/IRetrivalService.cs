using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

namespace Convenience.Models.Interfaces {
    public interface IRetrivalService : ISharedTools {

        /// <summary>
        /// 画面で指示されたソート指示を元に表情報に対しソートする
        /// </summary>
        /// <param name="argSortEventRec">ソート指示部</param>
        /// <param name="argChumonJissekis">ソート対象となる表示用注文実績データ</param>
        /// <returns></returns>
        IEnumerable<T1> SetSortKey<T1,T2>
            (IRetrivalViewModel<T2>.IKeywordAreaClass.ISortAreaClass.SortEventRec[] argSortEventRec, IEnumerable<T1> argChumonJissekis) {

            //OrderByのときはtrue、ThenByのときはfalse
            bool IsOrderBy = true;

            //画面のソート指示行を処理する
            for (int i = 0; i < argSortEventRec.Length; i++) {

                IRetrivalViewModel<T2>.IKeywordAreaClass.ISortAreaClass.SortEventRec aSortEventRec = argSortEventRec[i];
                if (ISharedTools.IsExistCheck(aSortEventRec.KeyEventData)) {     //指示がない場合があるので、チェック
                    string sortKey = aSortEventRec.KeyEventData!;
                    bool descending = aSortEventRec.Descending!;

                    //Linqの組み立て
                    if (IsOrderBy) {
                        argChumonJissekis = argChumonJissekis.AsQueryable().OrderBy(sortKey + (descending ? " descending" : ""));
                        IsOrderBy = false;
                    } else {
                        argChumonJissekis = ((IOrderedQueryable<T1>)argChumonJissekis).ThenBy(sortKey + (descending ? " descending" : ""));
                    }
                }
            }

            return argChumonJissekis;
        }

        /// <summary>
        /// ラムダ式を作る
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="leftSide">左辺</param>
        /// <param name="comparisonOperator">比較演算子</param>
        /// <param name="rightSide">右辺</param>
        /// <returns></returns>
        /// 
        /*
         * エンティティに対する条件処理
         */
        public Expression<Func<T, bool>>? WhereLambda1<T>(string leftSide, string comparison, string rightSide);
        
        /*
         * 表示用データエリアに対する条件処理
         */
        public Expression<Func<T, bool>>? WhereLambda2<T>(string leftSide, string comparison, string rightSide);

        /// <summary>
        /// 検索指示項目を認識しラムダ式を作る
        /// </summary>
        /// <param name="argSelecteWhereItemArray">検索指示項目</param>
        /// <param name="Meisais">注文実績クエリ</param>
        /// <returns>ラムダ式で処理された検索結果（注文実績明細が渡されたら遅延実行）</returns>
        IEnumerable<T1> SearchItemRecognizer<T1,T2>
            (IRetrivalViewModel<T2>.IKeywordAreaClass.IKeyAreaClass.SelecteWhereItem[] argSelecteWhereItemArray, IEnumerable<T1> Meisais) {

            bool needAnd = false;
            Expression<Func<T1, bool>>? setExpression = default; //初期化

            //検索指示項目行を処理する
            for (int i = 0; i < argSelecteWhereItemArray.Length; i++) {
                string? leftSide, rightSide, comparison;
                //左辺側が指示されているものだけ処理
                if (ISharedTools.IsExistCheck(leftSide = argSelecteWhereItemArray[i].LeftSide)) {
                    if (ISharedTools.IsExistCheck(rightSide = argSelecteWhereItemArray[i].RightSide)) {
                        if (ISharedTools.IsExistCheck(comparison = argSelecteWhereItemArray[i].ComparisonOperator)) {
                            /* Where系ラムダ式を作る */
                            Expression<Func<T1, bool>>? lambda;

                            if (typeof(T1) != typeof(T2)) {
                                lambda = ((IRetrivalService)this).WhereLambda1<T1>(leftSide!, comparison!, rightSide!);
                            } else {
                                lambda = ((IRetrivalService)this).WhereLambda2<T1>(leftSide!, comparison!, rightSide!);
                            }
                            if (ISharedTools.IsExistCheck(lambda)) {
                                if (needAnd) {
                                    //2回目以降はAnd定義を追加
                                    setExpression = CombineExpressions<T1>(setExpression!, lambda!);
                                } else {
                                    //初回はセットのみ
                                    setExpression = lambda;
                                    needAnd = true;
                                }
                            }
                        }
                    }
                }
            }
            /*
             *    注文実績クエリにWhere文追加
             */
            if (ISharedTools.IsExistCheck(setExpression)) {
                Meisais = Meisais.AsQueryable().Where(setExpression!);
            }
            return Meisais;
        }


        /// <summary>
        ///  複数のWhere系ラムダ式をAndで結ぶ
        /// </summary>
        /// <typeparam name="T">lambaの対象タイプ</typeparam>
        /// <param name="expr1">式に追加される先</param>
        /// <param name="expr2">式に追加する元</param>
        /// <returns>Andで式に追加された先</returns>
         Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2) {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            BinaryExpression combined = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        /// <summary>
        /// Where系ラムダ式を作る
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">左辺</param>
        /// <param name="strComparisonType">比較演算子に対する指示</param>
        /// <param name="value">右辺</param>
        /// <returns>Where系ラムダ式</returns>
        /// <exception cref="NotSupportedException"></exception>
        Expression<Func<T, bool>> BuildComparison<T>(
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
            Comparisons comparisonType = (Comparisons)Enum.Parse(typeof(Comparisons), strComparisonType);

            // 比較演算子をラムダ式に組み込む
            Expression comparison;
            if (property.Type == typeof(string)) {
                // 文字列の比較の場合、String.Compareを使用する
                System.Reflection.MethodInfo? compareMethod = typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) });
                MethodCallExpression compareExpression = Expression.Call(compareMethod!, property, constant);

                comparison = comparisonType switch {
                    Comparisons.Equal => Expression.Equal(compareExpression, Expression.Constant(0)),
                    Comparisons.NotEqual => Expression.NotEqual(compareExpression, Expression.Constant(0)),
                    Comparisons.GreaterThanOrEqual => Expression.GreaterThanOrEqual(compareExpression, Expression.Constant(0)),
                    Comparisons.GreaterThan => Expression.GreaterThan(compareExpression, Expression.Constant(0)),
                    Comparisons.LessThanOrEqual => Expression.LessThanOrEqual(compareExpression, Expression.Constant(0)),
                    Comparisons.LessThan => Expression.LessThan(compareExpression, Expression.Constant(0)),
                    _ => throw new NotSupportedException($"Comparison type {comparisonType} is not supported.")
                };
            } else {
                // 非文字列の場合、通常の比較演算を使用する
                comparison = comparisonType switch {
                    Comparisons.Equal => Expression.Equal(property, constant),
                    Comparisons.NotEqual => Expression.NotEqual(property, constant),
                    Comparisons.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
                    Comparisons.GreaterThan => Expression.GreaterThan(property, constant),
                    Comparisons.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
                    Comparisons.LessThan => Expression.LessThan(property, constant),
                    _ => throw new NotSupportedException($"Comparison type {comparisonType} is not supported.")
                };
            }

            // ラムダ式の作成
            return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        }
    }
}
