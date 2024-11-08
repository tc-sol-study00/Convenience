using AutoMapper;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.ChumonJisseki;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using static Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel;
using static Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass;

namespace Convenience.Models.Services {
    /// <summary>
    /// 注文実績検索サービス
    /// </summary>
    public class ChumonJissekiService : IChumonJissekiService, ISharedTools {

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
            queriedMeisai = SearchItemRecognizer(argChumonJissekiViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, queriedMeisai);

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
            chumonJissekiLines = SearchItemRecognizer<ChumonJissekiLineClass>(argChumonJissekiViewModel.KeywordArea.KeyArea.SelecteWhereItemArray, chumonJissekiLines);


            ChumonJissekiViewModel.DataArea.ChumonJissekiLines = chumonJissekiLines;

            /*
             *  マップされた注文実績の表情報を画面上のソート指示によりソートする
             */
            //ソートするために表情報を取り出す
            IEnumerable<DataAreaClass.ChumonJissekiLineClass> beforeDisplayForChumonJissekiLines =
                ChumonJissekiViewModel.DataArea.ChumonJissekiLines;
            //ソートする
            IEnumerable<DataAreaClass.ChumonJissekiLineClass> SortedChumonJissekiLines =
                SetSortKey(argChumonJissekiViewModel.KeywordArea.SortArea.KeyEventList, beforeDisplayForChumonJissekiLines);

            /*
             * 表情報をセットし返却
             */
            ChumonJissekiViewModel.DataArea.ChumonJissekiLines = SortedChumonJissekiLines;
            return ChumonJissekiViewModel;
        }

        /// <summary>
        /// 画面で指示されたソート指示を元に表情報に対しソートする
        /// </summary>
        /// <param name="argSortEventRec">ソート指示部</param>
        /// <param name="argChumonJissekis">ソート対象となる表示用注文実績データ</param>
        /// <returns></returns>
        private static IEnumerable<DataAreaClass.ChumonJissekiLineClass> SetSortKey
            (KeywordAreaClass.SortAreaClass.SortEventRec[] argSortEventRec, IEnumerable<DataAreaClass.ChumonJissekiLineClass> argChumonJissekis) {

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
                        argChumonJissekis = argChumonJissekis.AsQueryable().OrderBy(sortKey + (descending ? " descending" : ""));
                        IsOrderBy = false;
                    } else {
                        argChumonJissekis = ((IOrderedQueryable<DataAreaClass.ChumonJissekiLineClass>)argChumonJissekis).ThenBy(sortKey + (descending ? " descending" : ""));
                    }
                }
            }

            return argChumonJissekis;
        }

        /// <summary>
        /// 検索指示項目を認識しラムダ式を作る
        /// </summary>
        /// <param name="argSelecteWhereItemArray">検索指示項目</param>
        /// <param name="Meisais">注文実績クエリ</param>
        /// <returns>ラムダ式で処理された検索結果（注文実績明細が渡されたら遅延実行）</returns>
        private static IEnumerable<T> SearchItemRecognizer<T>
            (KeywordAreaClass.KeyAreaClass.SelecteWhereItem[] argSelecteWhereItemArray, IEnumerable<T> Meisais) {

            bool needAnd = false;
            Expression<Func<T, bool>>? setExpression = default; //初期化

            string shiireSakiKaisya =
                $"{nameof(ChumonJissekiMeisai)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShiireSakiMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShiireSakiMaster.ShiireSakiKaisya)}";

            string shiirePrdName =
                $"{nameof(ChumonJissekiMeisai)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShiirePrdName)}";

            string shohinName =
                $"{nameof(ChumonJissekiMeisai)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShohinMaster)}." +
                $"{nameof(ChumonJissekiMeisai.ShiireMaster.ShohinMaster.ShohinName)}";


            //検索指示項目行を処理する
            for (int i = 0; i < argSelecteWhereItemArray.Length; i++) {
                string? leftSide, rightSide, comparison;
                //左辺側が指示されているものだけ処理
                if (ISharedTools.IsExistCheck(leftSide = argSelecteWhereItemArray[i].LeftSide)) {
                    if (ISharedTools.IsExistCheck(rightSide = argSelecteWhereItemArray[i].RightSide)) {
                        if (ISharedTools.IsExistCheck(comparison = argSelecteWhereItemArray[i].ComparisonOperator)) {
                            /* Where系ラムダ式を作る */
                            Expression<Func<T, bool>>? lambda = default;

                            if ( typeof(T) ==  typeof(ChumonJissekiMeisai)) {
                                lambda = leftSide switch {
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ChumonId) =>
                                        BuildComparison<T>(nameof(ChumonJissekiMeisai.ChumonId), comparison!, rightSide!),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ShiireSakiId) =>
                                        BuildComparison<T>(nameof(ChumonJissekiMeisai.ShiireSakiId), comparison!, rightSide!),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ShiirePrdId) =>
                                        BuildComparison<T>(nameof(ChumonJissekiMeisai.ShiirePrdId), comparison!, rightSide!),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ShohinId) =>
                                        BuildComparison<T>(nameof(ChumonJissekiMeisai.ShohinId), comparison!, rightSide!),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ChumonSu) =>
                                        BuildComparison<T>(nameof(ChumonJissekiMeisai.ChumonSu), comparison!, decimal.Parse(rightSide!)),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ChumonZan) =>
                                        BuildComparison<T>(nameof(ChumonJissekiMeisai.ChumonZan), comparison!, decimal.Parse(rightSide!)),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ShiireSakiKaisya) =>
                                        BuildComparison<T>(shiireSakiKaisya, comparison!, rightSide!),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ShiirePrdName) =>
                                        BuildComparison<T>(shiirePrdName, comparison!, rightSide!),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ShohinName) =>
                                        BuildComparison<T>(shohinName, comparison!, rightSide!),
                                _ => null
                                };
                            } else if (typeof(T) == typeof(ChumonJissekiLineClass)){
                                lambda = leftSide switch {
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ShiireZumiSu) =>
                                        BuildComparison<T>(nameof(ChumonJissekiLineClass.ShiireZumiSu), comparison!, decimal.Parse(rightSide!)),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ChumonKingaku) =>
                                        BuildComparison<T>(nameof(ChumonJissekiLineClass.ChumonKingaku), comparison!, decimal.Parse(rightSide!)),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ShiireZumiKingaku) =>
                                        BuildComparison<T>(nameof(ChumonJissekiLineClass.ShiireZumiKingaku), comparison!, decimal.Parse(rightSide!)),
                                    nameof(DataAreaClass.ChumonJissekiLineClass.ChumonZanKingaku) =>
                                        BuildComparison<T>(nameof(ChumonJissekiLineClass.ChumonZanKingaku), comparison!, decimal.Parse(rightSide!)),
                                    _ => null
                                };
                            } else {
                                throw new Exception("タイプエラー");
                            }

                            if (ISharedTools.IsExistCheck(lambda)) {
                                if (needAnd) {
                                    //2回目以降はAnd定義を追加
                                    setExpression = CombineExpressions<T>(setExpression!, lambda!);
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
        private static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2) {
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
