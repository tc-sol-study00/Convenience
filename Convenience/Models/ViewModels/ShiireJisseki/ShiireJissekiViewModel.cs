using Convenience.Models.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using static Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel.DataAreaClass;
using System.Reflection;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Convenience.Models.ViewModels.ShiireJisseki {
    /// <summary>
    /// 仕入実績検索ビューモデル
    /// </summary>
    public class ShiireJissekiViewModel : ISharedTools {

        public KeywordAreaClass KeywordArea { get; set; }
        public DataAreaClass DataArea { get; set; }

        public ShiireJissekiViewModel() {
            this.KeywordArea = new KeywordAreaClass();
            this.DataArea = new DataAreaClass();
        }
        public class KeywordAreaClass : Convenience.Models.DataModels.ShiireJisseki {
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

                    //ソート初期設定
                    static SortEventRec getEvent(int number) => number switch {
                        0 => new SortEventRec(nameof(ShiireJissekiLineClass.ShiireDate), false),
                        1 => new SortEventRec(nameof(ShiireJissekiLineClass.SeqByShiireDate), false),
                        _ => new SortEventRec()
                    };

                    KeyEventList = Enumerable.Range(0, LineCountForSelectorOfOrder).Select(x => getEvent(x)).ToArray();

                    KeyList = new SelectList(
                        new List<SelectListItem>
                        {
                            new() { Value = nameof(ShiireJissekiLineClass.ShiireDate), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiireDate)) },
                            new() { Value = nameof(ShiireJissekiLineClass.SeqByShiireDate), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.SeqByShiireDate)) },
                            new() { Value = nameof(ShiireJissekiLineClass.ShiireDateTime), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiireDateTime)) },
                            new() { Value = nameof(ShiireJissekiLineClass.ChumonId), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ChumonId)) },
                            new() { Value = nameof(ShiireJissekiLineClass.ShiireSakiId), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiireSakiId)) },
                            new() { Value = nameof(ShiireJissekiLineClass.ShiireSakiKaisya), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiireSakiKaisya)) },
                            new() { Value = nameof(ShiireJissekiLineClass.ShiirePrdId), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiirePrdId)) },
                            new() { Value = nameof(ShiireJissekiLineClass.ShiirePrdName), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiirePrdName)) },
                            new() { Value = nameof(ShiireJissekiLineClass.ShohinId), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShohinId)) },
                            new() { Value = nameof(ShiireJissekiLineClass.ShohinName), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShohinName)) },
                            new() { Value = nameof(ShiireJissekiLineClass.NonyuSu), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.NonyuSu)) },

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

                    public SortEventRec(string? KeyEventData, bool Descending) {
                        this.KeyEventData = KeyEventData;
                        this.Descending = Descending;
                    }
                    public SortEventRec() {
                    }
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
                        new() { Value = nameof(ShiireJissekiLineClass.ShiireDate), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass),nameof(ShiireJissekiLineClass.ShiireDate)) },
                        new() { Value = nameof(ShiireJissekiLineClass.SeqByShiireDate), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.SeqByShiireDate)) },
                        new() { Value = nameof(ShiireJissekiLineClass.ShiireDateTime), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiireDateTime)) },
                        new() { Value = nameof(ShiireJissekiLineClass.ChumonId), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass),nameof(ShiireJissekiLineClass.ChumonId)) },
                        new() { Value = nameof(ShiireJissekiLineClass.ShiireSakiId), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiireSakiId)) },
                        new() { Value = nameof(ShiireJissekiLineClass.ShiireSakiKaisya), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiireSakiKaisya)) },
                        new() { Value = nameof(ShiireJissekiLineClass.ShiirePrdId), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiirePrdId)) },
                        new() { Value = nameof(ShiireJissekiLineClass.ShiirePrdName), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShiirePrdName)) },
                        new() { Value = nameof(ShiireJissekiLineClass.ShohinId), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShohinId)) },
                        new() { Value = nameof(ShiireJissekiLineClass.ShohinName), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.ShohinName)) },
                        new() { Value = nameof(ShiireJissekiLineClass.NonyuSu), Text = ISharedTools.GetDisplayName(typeof(ShiireJissekiLineClass), nameof(ShiireJissekiLineClass.NonyuSu)) },
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

            public IEnumerable<ShiireJissekiLineClass> ShiireJissekiLines { get; set; }

            public DataAreaClass() {
                this.ShiireJissekiLines = new List<ShiireJissekiLineClass>();
            }
            public class ShiireJissekiLineClass : Convenience.Models.DataModels.ShiireJisseki {

                [DisplayName("仕入先会社")]
                [Required]
                public string ShiireSakiKaisya { get; set; }

                [DisplayName("仕入商品名称")]
                [Required]
                public string ShiirePrdName { get; set; }

                [DisplayName("商品名称")]
                [Required]
                public string ShohinName { get; set; }

                public ShiireJissekiLineClass() {
                    ShiireSakiKaisya = string.Empty;
                    ShiirePrdName = string.Empty;
                    ShohinName = string.Empty;
                }
            }
        }

    }

}

