using Convenience.Models.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using static Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass;
using System.Reflection;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel.DataAreaClass;

namespace Convenience.Models.ViewModels.ChumonJisseki {
    /// <summary>
    /// 注文実績検索ビューモデル
    /// </summary>
    public class ChumonJissekiViewModel : ISharedTools {

        public KeywordAreaClass KeywordArea { get; set; }
        public DataAreaClass DataArea { get; set; }

        public ChumonJissekiViewModel() {
            this.KeywordArea = new KeywordAreaClass();
            this.DataArea = new DataAreaClass();
        }
        public class KeywordAreaClass : Convenience.Models.DataModels.ChumonJisseki {
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
                        0 => new SortEventRec(nameof(ChumonJissekiLineClass.ChumonId), false),
                        _ => new SortEventRec()
                    };

                    KeyEventList = Enumerable.Range(0, LineCountForSelectorOfOrder).Select(x => getEvent(x)).ToArray();

                    KeyList = new SelectList(
                        new List<SelectListItem>
                        {
                            new() { Value = nameof(ChumonJissekiLineClass.ChumonId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonId)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ShiireSakiId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiireSakiId)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ShiirePrdId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiirePrdId)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ShohinId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShohinId)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ChumonSu), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonSu)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ShiireZumiSu), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiireZumiSu)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ChumonZan), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonZan)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ShiireSakiKaisya), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiireSakiKaisya)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ShiirePrdName), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiirePrdName)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ShohinName), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShohinName)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ChumonKingaku), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonKingaku)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ShiireZumiKingaku), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiireZumiKingaku)) },
                            new() { Value = nameof(ChumonJissekiLineClass.ChumonZanKingaku), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonZanKingaku)) },

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
                        new() { Value = nameof(ChumonJissekiLineClass.ChumonId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass),nameof(ChumonJissekiLineClass.ChumonId)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiireSakiId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass),nameof(ChumonJissekiLineClass.ShiireSakiId)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiirePrdId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiirePrdId)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShohinId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShohinId)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ChumonSu), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonSu)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ChumonZan), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonZan)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiireSakiKaisya), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiireSakiKaisya)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiirePrdName), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiirePrdName)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShohinName), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShohinName)) },

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

            public IEnumerable<ChumonJissekiLineClass> ChumonJissekiLines { get; set; }

            public DataAreaClass() {
                this.ChumonJissekiLines = new List<ChumonJissekiLineClass>();
            }
            public class ChumonJissekiLineClass : Convenience.Models.DataModels.ChumonJissekiMeisai {

                [DisplayName("仕入先会社")]
                [Required]
                public string ShiireSakiKaisya { get; set; }

                [DisplayName("仕入商品名称")]
                [Required]
                public string ShiirePrdName { get; set; }

                [DisplayName("商品名称")]
                [Required]
                public string ShohinName { get; set; }

                [DisplayName("仕入済数")]
                [Required]
                [Precision(10, 2)]
                public decimal ShiireZumiSu { get; set; }

                [DisplayName("注文金額")]
                [Required]
                [Precision(10, 2)]
                public decimal ChumonKingaku { get; set; }

                [DisplayName("注文残金額")]
                [Required]
                [Precision(10, 2)]
                public decimal ChumonZanKingaku { get; set; }

                [DisplayName("仕入済金額")]
                [Required]
                [Precision(10, 2)]
                public decimal ShiireZumiKingaku { get; set; }


                public ChumonJissekiLineClass() {
                    ShiireSakiKaisya = string.Empty;
                    ShiirePrdName = string.Empty;
                    ShohinName = string.Empty;
                }


            }


        }

    }

}

