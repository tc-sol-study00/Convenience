using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Convenience.Models.Interfaces;
using static Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel.DataAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel.DataAreaClass.ShiireJissekiLineClass>;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel.DataAreaClass.ShiireJissekiLineClass>.IKeywordAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel.DataAreaClass.ShiireJissekiLineClass>.IKeywordAreaClass.ISortAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel.DataAreaClass.ShiireJissekiLineClass>.IKeywordAreaClass.IKeyAreaClass;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Convenience.Models.ViewModels.ShiireJisseki {
    /// <summary>
    /// 仕入実績検索ビューモデル
    /// </summary>
    public class ShiireJissekiViewModel : IRetrivalViewModel<ShiireJissekiLineClass> {

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
        public ShiireJissekiViewModel() {
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
                /// 仕入日付（昇順）・仕入SEQ（昇順）でセット
                /// </summary>
                public SortEventRec GetDefaltSortForSort(int index) {
                    return index switch {
                        0 => new SortEventRec(nameof(ShiireJissekiLineClass.ShiireDate), false),
                        1 => new SortEventRec(nameof(ShiireJissekiLineClass.SeqByShiireDate), false),
                        _ => new SortEventRec()
                    };
                }

                /// <summary>
                /// ソートキー入力最大行数
                /// </summary>
                public int LineCountForSelectorOfOrder { get; set; } = 6; //Order入力６行


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
                /// Where入力リスト初期化
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
                /// 仕入日（6か月前以降）でセット
                /// </summary>
                public SelecteWhereItem GetDefaltSortForWhere(int index) {
                    return index switch {
                        0 => new SelecteWhereItem(nameof(ShiireJissekiLineClass.ShiireDate), Comparisons.GreaterThanOrEqual.ToString(), (new DateOnly(DateTime.Now.AddMonths(-6).Year, DateTime.Now.AddMonths(-6).Month, 1)).ToString()),
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
            public IEnumerable<ShiireJissekiLineClass> Lines { get; set; }

            /// <summary>
            /// コンストラクタ
            ///  データ表示用リストの初期化
            /// </summary>
            public DataAreaClass() {
                this.Lines = new List<ShiireJissekiLineClass>();
            }

            /// <summary>
            /// データ表示用リストの１レコード定義
            /// </summary>
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

