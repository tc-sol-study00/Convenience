using Convenience.Models.DataModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass.ChumonJissekiLineClass>;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass.ChumonJissekiLineClass>.IKeywordAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass.ChumonJissekiLineClass>.IKeywordAreaClass.ISortAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass.ChumonJissekiLineClass>.IKeywordAreaClass.IKeyAreaClass;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Convenience.Models.ViewModels.ChumonJisseki {
    /// <summary>
    /// 注文実績検索ビューモデル
    /// </summary>
    public class ChumonJissekiViewModel : IRetrivalViewModel<ChumonJissekiLineClass> {

        /// <summary>
        /// ソートキー・検索キーエリア管理用
        /// </summary>
        public IKeywordAreaClass KeywordArea { get; set; }
        /// <summary>
        /// データ表示管理用
        /// </summary>
        public IDataAreaClass DataArea { get; set; }

        /// <summary>
        /// コンストラクタ
        /// ソート・検索キーエリア管理用オブジェクト初期化
        /// </summary>
        public ChumonJissekiViewModel() {
            this.KeywordArea = new KeywordAreaClass();
            this.DataArea = new DataAreaClass();
        }

        /// <summary>
        /// ソートキー・検索キーエリア管理用クラス
        /// </summary>
        public class KeywordAreaClass : IKeywordAreaClass {
            /// <summary>
            /// ソートキーエリア管理用
            /// </summary>
            public ISortAreaClass SortArea { get; set; }
            /// <summary>
            /// 検索キーエリア管理用
            /// </summary>
            public IKeyAreaClass KeyArea { get; set; }

            /// <summary>
            /// コンストラクター
            /// ソートキーエリア・検索キーエリア管理用オブジェクト初期化
            /// </summary>
            public KeywordAreaClass() {
                this.SortArea = new SortAreaClass();
                this.KeyArea = new KeyAreaClass();
            }
            /// <summary>
            /// ソートエリア管理用クラス
            /// </summary>
            public class SortAreaClass : ISortAreaClass {

                /// <summary>
                /// ソートキー指示データ管理用
                /// </summary>
                public SortEventRec[] KeyEventList { get; set; }
                
                /// <summary>
                /// ソートキー一覧表示用
                /// </summary>
                [JsonIgnore]
                public SelectList KeyList { get; set; }

                /// <summary>
                /// ソートキー指示データ初期データセット
                /// 注文コード（昇順）でセット
                /// </summary>
                public SortEventRec GetDefaltSortForSort(int index) {
                    return index switch {
                        0 => new SortEventRec(nameof(ChumonJissekiLineClass.ChumonId), false),
                        _ => new SortEventRec()
                    };
                }

                /// <summary>
                /// ソートキー入力最大行数
                /// </summary>
                public int LineCountForSelectorOfOrder { get; set; } = 6;   //Order入力６行

                /// <summary>
                /// ソートキーエリア管理用クラス
                /// </summary>
                public SortAreaClass() {
                    /*
                     * 初期化
                     */
                    KeyEventList = new SortEventRec[LineCountForSelectorOfOrder];

                    /*
                     *  ソートキー一覧表示セット
                     */
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

                    /*
                     * ソートキー指示データセット
                     */
                    ((ISortAreaClass)this).InitSortArea();

                }

            }

            /// <summary>
            /// 検索キー管理用クラス
            /// </summary>
            public class KeyAreaClass : IKeyAreaClass {

                /// <summary>
                ///  検索キー指示データ管理用
                /// </summary>
                public SelecteWhereItem[] SelecteWhereItemArray { get; set; }

                /// <summary>
                /// 検索キー入力最大行数
                /// </summary>
                [JsonIgnore]
                public int LineCountForSelectorOfWhere { get; set; } = 6; //Where入力６行


                /// <summary>
                /// 比較演算子選択用
                /// </summary>
                /// 
                [JsonIgnore]
                public SelectList ComparisonOperatorList { get; set; }

                /// <summary>
                /// Where左辺用カラムセット用
                /// </summary>
                /// 
                [JsonIgnore]
                public SelectList SelectWhereLeftSideList { get; set; }

                /// <summary>
                /// 検索キー指示データ初期データセット
                /// 注文日（6か月前以降）でセット
                /// </summary>

                public SelecteWhereItem GetDefaltSortForWhere(int index) {
                    return  index switch {
                        0 => new SelecteWhereItem(nameof(ChumonJissekiLineClass.ChumonDate), Comparisons.GreaterThanOrEqual.ToString(), (new DateOnly(DateTime.Now.AddMonths(-6).Year, DateTime.Now.AddMonths(-6).Month, 1)).ToString()),
                        _ => new SelecteWhereItem()
                    };
                }
                /// <summary>
                /// 検索キーエリア管理用クラス
                /// </summary>
                public KeyAreaClass() {

                    /*
                     * 初期化
                     */
                    SelecteWhereItemArray = new SelecteWhereItem[LineCountForSelectorOfWhere];
                    ComparisonOperatorList = new SelectList(new List<SelectListItem>());

                    /*
                     *  検索キー一覧表示セット
                     */
                    SelectWhereLeftSideList = new SelectList(
                    new List<SelectListItem>{
                        new() { Value = nameof(ChumonJissekiLineClass.ChumonId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass),nameof(ChumonJissekiLineClass.ChumonId)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ChumonDate), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass),nameof(ChumonJissekiLineClass.ChumonDate)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiireSakiId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass),nameof(ChumonJissekiLineClass.ShiireSakiId)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiireSakiKaisya), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiireSakiKaisya)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiirePrdId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiirePrdId)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiirePrdName), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiirePrdName)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShohinId), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShohinId)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShohinName), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShohinName)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ChumonSu), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonSu)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiireZumiSu), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiireZumiSu)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ChumonZan), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonZan)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ChumonKingaku), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonKingaku)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ShiireZumiKingaku), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ShiireZumiKingaku)) },
                        new() { Value = nameof(ChumonJissekiLineClass.ChumonZanKingaku), Text = ISharedTools.GetDisplayName(typeof(ChumonJissekiLineClass), nameof(ChumonJissekiLineClass.ChumonZanKingaku)) },
                    },
                    "Value",
                    "Text"
                    );

                    /*
                     * 比較演算子一覧のセット
                     * 検索キーキー指示データセット
                     */
                    ((IKeyAreaClass)this).InitKeyArea();

                }

            }
        }

        /// <summary>
        /// データ表示管理用クラス
        /// </summary>
        public class DataAreaClass : IDataAreaClass {

            /// <summary>
            /// データ表示用リスト
            /// </summary>
            public IEnumerable<ChumonJissekiLineClass> Lines { get; set; }

            public ChumonJissekiLineClass SummaryLine { get; set; }

            /// <summary>
            /// コンストラクタ
            ///  データ表示用リストの初期化
            /// </summary>
            public DataAreaClass() {
                Lines = new List<ChumonJissekiLineClass>();
                SummaryLine = new ChumonJissekiLineClass();
            }

            /// <summary>
            /// データ表示用リストの１レコード定義
            /// </summary>
            public class ChumonJissekiLineClass : ChumonJissekiMeisai {

                [DisplayName("注文日")]
                [Required]
                public DateOnly ChumonDate { get; set; }

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

