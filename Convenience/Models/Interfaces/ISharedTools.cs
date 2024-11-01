using Convenience.Models.ViewModels.Chumon;
using Newtonsoft.Json;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// シリアライズ・デシリアライズ化
    /// </summary>
    public interface ISharedTools {
        /// <summary>
        /// シリアライズ化
        /// </summary>
        /// <typeparam name="T">シリアル対象オブジェクトのタイプ設定</typeparam>
        /// <param name="obj">シリアル化する対象オブジェクト</param>
        /// <returns></returns>
        protected static string ConvertToSerial<T>(T obj) {
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }
        /// <summary>
        /// デシリアライズ化
        /// </summary>
        /// <typeparam name="T">デシリアライズ化対象オブジェクトのタイプ設定</typeparam>
        /// <param name="serial">シリアルデータ</param>
        /// <returns></returns>
        protected static T ConvertFromSerial<T>(string serial) {
            return JsonConvert.DeserializeObject<T>(serial) ?? throw new Exception("Deserializeエラー");
        }

        /// <summary>
        /// データがnullではないか、リスト形式ならば0件ではないか
        /// </summary>
        /// <typeparam name="T">チェックする型</typeparam>
        /// <param name="checkdata">チェックするデータ</param>
        /// <returns></returns>
        protected static bool IsExistCheck<T>(T? checkdata) {
            if (checkdata == null) {
                return false; // null の場合は false を返す
            }

            // T が IEnumerable かどうかを確認
            if (checkdata is IEnumerable<object>) {
                return ((IEnumerable<object>)checkdata).Any(); // リストの場合は要素があるかどうかを確認
            }
            return true;
        }
    }
}

