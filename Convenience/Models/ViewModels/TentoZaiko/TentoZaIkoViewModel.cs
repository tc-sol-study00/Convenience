using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using static Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel.DataAreaClass;
using Newtonsoft.Json;
namespace Convenience.Models.ViewModels.TentoZaiko {

    /// <summary>
    /// 店頭在庫検索ビューモデル
    /// </summary>
    public class TentoZaikoViewModel {
        public KeywordAreaClass KeywordArea { get; set; }
        public DataAreaClass DataArea { get; set; }

        public TentoZaikoViewModel() {
            KeywordArea = new KeywordAreaClass();
            DataArea = new DataAreaClass();
        }

        public class KeywordAreaClass {
            public SortAreaClass SortArea { get; set; }
            public KeyAreaClass KeyArea { get; set; }

            public KeywordAreaClass() {
                SortArea = new SortAreaClass();
                KeyArea = new KeyAreaClass();
            }

            public class SortAreaClass {
                public SortEventRec[] KeyEventList { get; set; }

                [JsonIgnore]
                public SelectList KeyList { get; set; }

                /// <summary>
                /// Where入力行数
                /// </summary>
                const int LineCountForSelectorOfOrder = 6; //Order入力６行

                public SortAreaClass() {
                    KeyEventList = Enumerable.Range(0, LineCountForSelectorOfOrder).Select(_ => new SortEventRec()).ToArray();

                    KeyList = new SelectList(
                        new List<SelectListItem>
                        {
                            new() { Value = nameof(TentoZaIkoLine.ShohinId), Text = GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.ShohinId)) },
                            new() { Value = nameof(TentoZaIkoLine.ShohinName), Text = GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.ShohinName)) },
                            new() { Value = nameof(TentoZaIkoLine.ZaikoSu), Text = GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.ZaikoSu)) },
                            new() { Value = nameof(TentoZaIkoLine.LastShireDateTime), Text = GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.LastShireDateTime)) },
                            new() { Value = nameof(TentoZaIkoLine.LastHaraidashiDate), Text = GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.LastHaraidashiDate)) },
                            new() { Value = nameof(TentoZaIkoLine.LastUriageDatetime), Text = GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.LastUriageDatetime)) },
                        },
                        "Value",
                        "Text"
                    );
                }

                public static string? GetDisplayName(Type type, string propertyName) {
                    PropertyInfo? property = type.GetProperty(propertyName);
                    if (property != null) {
                        DisplayNameAttribute? displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
                        if (displayNameAttribute != null) {
                            return displayNameAttribute.DisplayName;
                        }
                    }
                    return null; // DisplayNameが存在しない場合はnullを返す
                }

                public class SortEventRec {
                    [DisplayName("ソート項目")]
                    public string? KeyEventData { get; set; }
                    [DisplayName("昇順・降順")]
                    public bool Descending { get; set; } = false;
                }
            }

            public class KeyAreaClass {
                /// <summary>
                /// Where指示選択結果セット用
                /// </summary>
                public class SelecteWhereItem {
                    [DisplayName("検索項目項目")]
                    public string? LeftSide { get; set; }

                    [DisplayName("比較")]
                    [MaxLength(2)]
                    public string? ComparisonOperator { get; set; } = "==";

                    [DisplayName("検索キー")]
                    public string? RightSide { get; set; }
                }

                /// <summary>
                /// 比較演算子選択用
                /// </summary>
                /// 
                [JsonIgnore]
                public SelectList ComparisonOperatorList { get; set; }

                /// <summary>
                /// Where入力行数
                /// </summary>
                [JsonIgnore]
                const int LineCountForSelectorOfWhere = 6; //Where入力６行

                /// <summary>
                /// Where入力リスト初期化
                /// </summary>
                [JsonIgnore]
                public SelecteWhereItem[] SelecteWhereItemArray { get; set; }

                /// <summary>
                /// Where左辺用カラムセット用
                /// </summary>
                /// 
                [JsonIgnore]
                public SelectList SelectWhereLeftSideList { get; set; }

                public KeyAreaClass() {

                    ComparisonOperatorList = new SelectList(
                    new List<SelectListItem> {
                        new (){ Value = Comparisons.Equal.ToString(), Text = "=" },
                        new (){ Value = Comparisons.NotEqual.ToString(), Text = "!=" },
                        new (){ Value = Comparisons.GreaterThanOrEqual.ToString(), Text = ">=" },
                        new (){ Value = Comparisons.GreaterThan.ToString(), Text = ">" },
                        new (){ Value = Comparisons.LessThanOrEqual.ToString(), Text = "<=" },
                        new (){ Value = Comparisons.LessThan.ToString(), Text = "<" },
                    },
                    "Value",
                    "Text"
                );

                    /// <summary>
                    /// Where左辺用カラムセット用
                    /// </summary>
                    SelectWhereLeftSideList = new SelectList(
                    new List<SelectListItem>{
                        new() { Value = nameof(TentoZaIkoLine.ShohinId), Text = "商品コード" },
                        new() { Value = nameof(TentoZaIkoLine.ShohinName), Text = "商品名" },
                        new() { Value = nameof(TentoZaIkoLine.ZaikoSu), Text = "店頭在庫数" },
                        new() { Value = nameof(TentoZaIkoLine.LastShireDateTime), Text = "直近仕入日時" },
                        new() { Value = nameof(TentoZaIkoLine.LastHaraidashiDate), Text = "直近払出日時" },
                        new() { Value = nameof(TentoZaIkoLine.LastUriageDatetime), Text = "直近売上日時" },
                    },
                    "Value",
                    "Text"
                );
                    /// <summary>
                    /// Where入力リスト初期化
                    /// </summary>
                    SelecteWhereItemArray
                        = Enumerable.Range(0, LineCountForSelectorOfWhere).Select(_ => new SelecteWhereItem()).ToArray();

                }


                public enum Comparisons {
                    Equal,              //  ==
                    NotEqual,           //  !=
                    GreaterThanOrEqual, //  >=
                    GreaterThan,        //  >
                    LessThanOrEqual,    //  <=
                    LessThan            //  <
                }


            }
        }

        public class DataAreaClass {
            public IEnumerable<TentoZaIkoLine> TentoZaIkoLines { get; set; }

            public DataAreaClass() {
                TentoZaIkoLines = new List<TentoZaIkoLine>();
            }

            public class TentoZaIkoLine {
                [DisplayName("商品コード")]
                [MaxLength(10)]
                [Required]
                public string? ShohinId { get; set; }

                [DisplayName("商品名称")]
                [MaxLength(10)]
                [Required]
                public string? ShohinName { get; set; }

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
}
