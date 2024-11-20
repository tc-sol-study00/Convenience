using System.Collections.Specialized;
using System.Reflection;
using System.Linq;

namespace Convenience.Models.Interfaces {

    /// <summary>
    /// 合計集計用インターフェース
    /// </summary>
    /// <remarks>
    /// 合計機能が必要なクラスに実装する
    /// </remarks>
    public interface ITotalSummaryRetrival : ISharedTools {

        /// <summary>
        /// 合計集計
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argDatas">合計集計される元リスト</param>
        /// <param name="argOutDatas">合計集計結果</param>
        /// <param name="aCountProperty">集計されるリストの件数をセットするプロパティ名</param>
        /// <remarks>
        /// argOutDatasはインスタンス化され、数字項目は初期設定されていること
        /// </remarks>
        /// <returns></returns>
        public T TotalSummary<T>(IEnumerable<T> argDatas, T argOutDatas, string? aCountProperty) {

            if (IsExistCheck(argOutDatas)) {
                /*
                 *合計集計する
                 */
                int count = 0;

                foreach (T? aData in argDatas) {
                    if (IsExistCheck(aData)) {
                        count++;
                        foreach (PropertyInfo? inProperty in aData!.GetType().GetProperties().Where(
                        //合計集計される元リストの一レコードの項目を認識する
                        inProperty => IsNumericType(inProperty.PropertyType))
                        //もし数値タイプであれば
                        ) {
                            //数字として扱えるプロパティを求める
                            PropertyInfo? outputProperty = argOutDatas!.GetType().GetProperties().FirstOrDefault(p => p.Name == inProperty.Name);
                            if (IsExistCheck(outputProperty)) {
                                //合計集計結果を求める
                                if (IsExistCheck(argOutDatas)) {
                                    object? inValue = inProperty.GetValue(aData);
                                    object? outValue = outputProperty!.GetValue(argOutDatas);
                                    decimal sum = Convert.ToDecimal(outValue) + Convert.ToDecimal(inValue);
                                    outputProperty.SetValue(argOutDatas, Convert.ChangeType(sum, outputProperty.PropertyType));
                                }
                            }
                        }
                    }
                }

                //集計されるリストの件数をセットするプロパティ名に件数をセット

                if (IsExistCheck(aCountProperty)) {
                    PropertyInfo? setCountProperty = argOutDatas!.GetType().GetProperty(aCountProperty!);

                    if (IsExistCheck(setCountProperty)) {
                        if (IsNumericType(setCountProperty!.PropertyType)) {
                            setCountProperty.SetValue(argOutDatas, count);
                        } else {
                            setCountProperty.SetValue(argOutDatas, count.ToString());
                        }
                    }
                }

            }

            //合計集計結果を返す
            return argOutDatas;
        }

        /// <summary>
        /// 数値型かどうかを確認するメソッド
        /// </summary>
        /// <param name="type"></param>
        /// <returns>数値型であればtrue</returns>
        private static bool IsNumericType(Type type) {
            return type == typeof(int) || type == typeof(double) || type == typeof(float) ||
                   type == typeof(decimal) || type == typeof(long) || type == typeof(short) ||
                   type == typeof(byte);
        }
    }
}
