using Convenience.Models.ViewModels.Chumon;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 各種ツール的なメソッド集
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

        /// <summary>
        /// データがnullか、リスト形式ならば0件か
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="checkdata"></param>
        /// <returns></returns>
        protected static bool IsNotExistCheck<T>(T? checkdata) {
            return !IsExistCheck(checkdata);
        }


        public static string? GetDisplayName(Type type, string propertyName) {
            var property = type.GetProperty(propertyName);
            if (property != null) {
                var displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
                if (displayNameAttribute != null) {
                    return displayNameAttribute.DisplayName;
                }
            }
            return null; // DisplayNameが存在しない場合はnullを返す
        }

        public static string PadString(string input, int length, char padchar = ' ') {
            int byteLength = input.Sum(c => (c > 127 ? 2 : 1)); // 全角文字は2バイト、半角文字は1バイト
            if (length >= 0) {
                return input.PadLeft(length - byteLength + input.Length, padchar);
            }
            else {
                length *= -1;
                return input.PadRight(length - byteLength + input.Length, padchar);
            }
        }

        //Task<IActionResult> InsertRow(int index);
    }
}

