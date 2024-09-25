using Convenience.Models.ViewModels.Chumon;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        protected static string ConvertToSerial<T>(T obj){
            return JsonSerializer.Serialize(obj,new JsonSerializerOptions() {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        WriteIndented = true,
                });
        }
        /// <summary>
        /// デシリアライズ化
        /// </summary>
        /// <typeparam name="T">デシリアライズ化対象オブジェクトのタイプ設定</typeparam>
        /// <param name="serial">シリアルデータ</param>
        /// <returns></returns>
        protected static T ConvertFromSerial<T>(string serial) {
            return JsonSerializer.Deserialize<T>(serial);
        }
    }
}

