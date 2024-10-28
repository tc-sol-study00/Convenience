using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Convenience.Models.DataModels;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;
using System.Reflection;

namespace Convenience.Models.ViewModels.TentoZaiko {



    public class KeyArea {
        public KeyArea.KeyEventRec[] KeyEventList { get; set; }
        public SelectList KeyList { get; set; }

        public KeyArea() {
            KeyEventList = Enumerable.Range(0, 5).Select(_ => new KeyArea.KeyEventRec()).ToArray();
            KeyList = new(
        new List<SelectListItem>
        {
           new() { Value = nameof(DataModels.TentoZaiko.ShohinId), Text = GetDisplayName(typeof(DataModels.TentoZaiko), nameof(DataModels.TentoZaiko.ShohinId)) },
        }, "Value", "Text");

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
        public class KeyEventRec {
            [DisplayName("ソート項目")]
            public string? KeyEventData { get; set; }
            [DisplayName("昇順・降順")]
            public bool Descending { get; set; } = false;
        }

    }

    public class TentoZaikoViewModel {

        public KeyArea KeyArea { get; set; }

        public DataArea DataArea { get; set; }

        public TentoZaikoViewModel() {
            KeyArea = new KeyArea();
            DataArea = new DataArea();
        }
    }

    public class DataArea {
        public IEnumerable<DataArea.TentoZaIkoLine> TentoZaIkoLines { get; set; } 

        public DataArea() {
            TentoZaIkoLines = new List<DataArea.TentoZaIkoLine>() { };
        }

        public class TentoZaIkoLine {

            [DisplayName("商品コード")]
            [MaxLength(10)]
            [Required]
            public string? ShohinId { get; set; }

            [Column("zaiko_su")]
            [DisplayName("店頭在庫数")]
            [Precision(7, 2)]
            public decimal ZaikoSu { get; set; }

            [Column("last_shiire_datetime")]
            [DisplayName("直近仕入日時")]
            [Required]
            [DataType(DataType.DateTime)]
            public DateOnly LastShireDateTime { get; set; }

            [Column("last_haraidashi_datetime")]
            [DisplayName("直近払出日時")]
            [Required]
            [DataType(DataType.DateTime)]
            public DateTime? LastHaraidashiDate { get; set; }

            [Column("last_uriage_datetime")]
            [DisplayName("直近売上日時")]
            [DataType(DataType.DateTime)]
            public DateTime? LastUriageDatetime { get; set; }
        }
    }

}
