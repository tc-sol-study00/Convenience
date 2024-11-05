using Convenience.Models.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using static Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel.DataAreaClass;
using System.Reflection;
using Convenience.Models.Interfaces;

namespace Convenience.Models.ViewModels.KaikeiJisseki {
    public class KaikeiJissekiViewModel : ISharedTools {

        public KeywordAreaClass KeywordArea { get; set; }
        public DataAreaClass DataArea { get; set; }

        public KaikeiJissekiViewModel() {
            this.KeywordArea = new KeywordAreaClass();
            this.DataArea = new DataAreaClass();
        }
        public class KeywordAreaClass : Convenience.Models.DataModels.KaikeiJisseki {
            public SortAreaClass SortArea { get; set; }
            public KeyAreaClass KeyArea { get; set; }

            public KeywordAreaClass() {
                this.SortArea = new SortAreaClass();
                this.KeyArea = new KeyAreaClass();
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
                            new() { Value = nameof(KaikeiJissekiLineClass.ShohinId), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ShohinId)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.ShohinName), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ShohinName)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.UriageDatetime), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.UriageDatetime)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.UriageSu), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.UriageSu)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.UriageKingaku), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.UriageKingaku)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.ZeikomiKingaku), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ZeikomiKingaku)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.ShohinTanka), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ShohinTanka)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.ShohiZeiritsu), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ShohiZeiritsu)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.NaigaiClass), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.NaigaiClass)) },
                        },
                        "Value",
                        "Text"
                    );
                }

                public class SortEventRec {
                    [DisplayName("ソート項目")]
                    public string? KeyEventData { get; set; }
                    [DisplayName("昇順・降順")]
                    public bool Descending { get; set; } = false;
                }
            }

            public class KeyAreaClass {
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
                        new() { Value = nameof(KaikeiJissekiLineClass.ShohinId), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass),nameof(KaikeiJissekiLineClass.ShohinId)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.ShohinName), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass),nameof(KaikeiJissekiLineClass.ShohinName)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.UriageDatetime), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.UriageDatetime)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.UriageSu), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.UriageSu)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.UriageKingaku), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.UriageKingaku)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.ZeikomiKingaku), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ZeikomiKingaku)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.ShohinTanka), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ShohinTanka)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.ShohiZeiritsu), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ShohiZeiritsu)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.NaigaiClass), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.NaigaiClass)) },

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

            public IEnumerable<KaikeiJissekiLineClass> KaikeiJissekiLines { get; set; }

            public DataAreaClass() {
                this.KaikeiJissekiLines = new List<KaikeiJissekiLineClass>();
            }
            public class KaikeiJissekiLineClass : Convenience.Models.DataModels.KaikeiJisseki {
                [DisplayName("商品名称")]
                [Required]
                public string ShohinName { get; set; }

                [DisplayName("内外区分名称")]
                [Required]
                public string NaigaiClassName { get; set; }

                public KaikeiJissekiLineClass() {
                    ShohinName = string.Empty;
                    NaigaiClassName = string.Empty;
                }


            }


        }

    }

}

