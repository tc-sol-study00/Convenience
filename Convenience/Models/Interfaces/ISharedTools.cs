using Convenience.Models.ViewModels.Chumon;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Convenience.Models.Interfaces {
    public interface ISharedTools {

        //シリアライズ化
        protected static string ConvertToSerial<T>(T obj){
            return JsonSerializer.Serialize(obj,new JsonSerializerOptions() {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        WriteIndented = true,
                });
        }
        //デシリアライズ化
        protected static T ConvertFromSerial<T>(string serial) {
            return JsonSerializer.Deserialize<T>(serial);
        }
    }
}

