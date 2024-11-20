using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using static Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel.DataAreaClass;
using Convenience.Models.Interfaces;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel.DataAreaClass.KaikeiJissekiLineClass>;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel.DataAreaClass.KaikeiJissekiLineClass>.IKeywordAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel.DataAreaClass.KaikeiJissekiLineClass>.IKeywordAreaClass.ISortAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel.DataAreaClass.KaikeiJissekiLineClass>.IKeywordAreaClass.IKeyAreaClass;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Convenience.Models.ViewModels.KaikeiJisseki {
    /// <summary>
    /// 会計実績検索ビューモデル
    /// </summary>
    public class KaikeiJissekiViewModel : IRetrivalViewModel<KaikeiJissekiLineClass> {

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
        public KaikeiJissekiViewModel() {
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

            public KeywordAreaClass() {
                this.SortArea = new SortAreaClass();
                this.KeyArea = new KeyAreaClass();
            }

            /// <summary>
            /// コンストラクター
            /// ソートキーエリア・検索キーエリア管理用オブジェクト初期化
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
                /// </summary>
                public SortEventRec GetDefaltSortForSort(int index) {
                    //ソート初期設定
                    return index switch {
                        0 => new SortEventRec(nameof(KaikeiJissekiLineClass.UriageDatetime), false),
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
                     * ソートキーのセレクトリストセット
                     */
                    KeyList = new SelectList(
                        new List<SelectListItem>
                        {
                            new() { Value = nameof(KaikeiJissekiLineClass.UriageDatetime), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.UriageDatetime)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.ShohinId), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ShohinId)) },
                            new() { Value = nameof(KaikeiJissekiLineClass.ShohinName), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.ShohinName)) },
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

                    /*
                     * ソートキー指示データセット
                     */
                    ((ISortAreaClass)this).InitSortArea();
                }

            }
            /// <summary>
            /// キー入力エリア
            /// </summary>
            public class KeyAreaClass : IKeyAreaClass {

                /// <summary>
                /// Where入力リスト初期化
                /// </summary>
                [JsonIgnore]
                public SelecteWhereItem[] SelecteWhereItemArray { get; set; }

                /// <summary>
                /// Where入力行数
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
                    return index switch {
                        0 => new SelecteWhereItem(nameof(KaikeiJissekiLineClass.UriageDatetime), Comparisons.GreaterThanOrEqual.ToString(), (new DateTime(DateTime.Now.AddMonths(-6).Year, DateTime.Now.AddMonths(-6).Month, 1)).ToString()),
                        _ => new SelecteWhereItem()
                    };
                }
                /// <summary>
                /// キー入力エリア管理用クラス
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
                        new() { Value = nameof(KaikeiJissekiLineClass.UriageDatetime), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass), nameof(KaikeiJissekiLineClass.UriageDatetime)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.ShohinId), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass),nameof(KaikeiJissekiLineClass.ShohinId)) },
                        new() { Value = nameof(KaikeiJissekiLineClass.ShohinName), Text = ISharedTools.GetDisplayName(typeof(KaikeiJissekiLineClass),nameof(KaikeiJissekiLineClass.ShohinName)) },
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
                    /*
                     * 比較演算子一覧のセット
                     * 検索キーキー指示データセット
                     */
                    ((IKeyAreaClass)this).InitKeyArea();
                }

            }

        }

        /// <summary>
        /// データ表示用クラス
        /// </summary>
        public class DataAreaClass : IDataAreaClass {

            /// <summary>
            /// データ表示用リスト
            /// </summary>
            public IEnumerable<KaikeiJissekiLineClass> Lines { get; set; }

            public KaikeiJissekiLineClass SummaryLine { get; set; }

            /// <summary>
            /// コンストラクタ
            ///  データ表示用リストの初期化
            /// </summary>
            public DataAreaClass() {
                this.Lines = new List<KaikeiJissekiLineClass>();
                this.SummaryLine = new KaikeiJissekiLineClass();
            }

            /// <summary>
            /// データ表示用リストの１レコード定義
            /// </summary>
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

