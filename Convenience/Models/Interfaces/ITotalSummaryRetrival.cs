using System.Collections.Specialized;
using System.Reflection;

namespace Convenience.Models.Interfaces {
    public interface ITotalSummaryRetrival: ISharedTools {

        public T TotalSummary<T>(IEnumerable<T> argDatas, T argOutDatas, string? aCountProperty) {

            int count = 0;
            foreach (var aData in argDatas) {
                count++;
                foreach(PropertyInfo inProperty in aData.GetType().GetProperties()) {
                    if (IsNumericType(inProperty.PropertyType)) {

                        PropertyInfo? outputProperty = argOutDatas.GetType().GetProperties().FirstOrDefault(p => p.Name == inProperty.Name);

                        if (IsExistCheck(argOutDatas)) {

                            var inValue = inProperty.GetValue(aData);
                            var outValue = outputProperty.GetValue(argOutDatas);
                            var sum = Convert.ToDecimal(outValue)+ Convert.ToDecimal(inValue);
                            outputProperty.SetValue(argOutDatas, Convert.ChangeType(sum, outputProperty.PropertyType));
                        }
                    }
                }
            }

            PropertyInfo? setCountProperty =argOutDatas.GetType().GetProperty(aCountProperty);

            if (IsExistCheck(setCountProperty)) {
                if (IsNumericType(setCountProperty.PropertyType)) {
                    setCountProperty.SetValue(argOutDatas, count);
                } else {
                    setCountProperty.SetValue(argOutDatas, count.ToString());
                }
            }
            return argOutDatas;

        }

        // 数値型かどうかを確認するメソッド
        private static bool IsNumericType(Type type) {
            return type == typeof(int) || type == typeof(double) || type == typeof(float) ||
                   type == typeof(decimal) || type == typeof(long) || type == typeof(short) ||
                   type == typeof(byte);
        }
    }
}
