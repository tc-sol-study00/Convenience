using Convenience.Models.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;


namespace Convenience.Models.Interfaces {

    /// <summary>
    /// 比較演算子のスキーマー化
    /// </summary>
    public enum Comparisons {
        Equal,              //  ==
        NotEqual,           //  !=
        GreaterThanOrEqual, //  >=
        GreaterThan,        //  >
        LessThanOrEqual,    //  <=
        LessThan            //  <
    }

    /// <summary>
    /// 注文実績検索ビューモデル用インターフェース
    /// </summary>
    public interface IRetrivalViewModel<T> : ISharedTools {
        /// <summary>
        /// ソートキー・検索キーエリア管理用
        /// </summary>
        IKeywordAreaClass KeywordArea { get; set; }
        /// <summary>
        /// データ表示管理用
        /// </summary>
        IDataAreaClass DataArea { get; set; }

        /// <summary>
        /// ソートキー・検索キーエリア管理用インターフェース
        /// </summary>
        public interface IKeywordAreaClass {
            /// <summary>
            /// ソートキーエリア管理用
            /// </summary>
            ISortAreaClass SortArea { get; set; }
            /// <summary>
            /// 検索キーエリア管理用
            /// </summary>
            IKeyAreaClass KeyArea { get; set; }

            /// <summary>
            /// ソートエリア管理用インターフェース
            /// </summary>

            public interface ISortAreaClass {

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
                public SortEventRec GetDefaltSortForSort(int index);

                /// <summary>
                /// ソートキーエリア管理用クラス
                /// </summary>
                public int LineCountForSelectorOfOrder { get; set; }

                /// <summary>
                ///  ソートキー指示データセット
                /// </summary>
                public void InitSortArea() {
                    KeyEventList = Enumerable.Range(0, LineCountForSelectorOfOrder).Select(x => GetDefaltSortForSort(x)).ToArray();
                }
                /// <summary>
                /// ソートキー指示データクラス
                /// </summary>
                public class SortEventRec {
                    [DisplayName("ソート項目")]
                    public string? KeyEventData { get; set; }
                    [DisplayName("昇順・降順")]
                    public bool Descending { get; set; } = false;
                    public SortEventRec(string? KeyEventData, bool Descending) {
                        this.KeyEventData = KeyEventData;
                        this.Descending = Descending;
                    }
                    public SortEventRec() { }

                }
            }

            /// <summary>
            /// 検索キー管理用クラス
            /// </summary>
            public interface IKeyAreaClass {

                /// <summary>
                /// Where入力リスト1レコード定義用クラス
                /// </summary>
                public class SelecteWhereItem {
                    [DisplayName("検索項目項目")]
                    public string? LeftSide { get; set; }

                    [DisplayName("比較")]
                    [MaxLength(2)]
                    public string? ComparisonOperator { get; set; }

                    [DisplayName("検索キー")]
                    public string? RightSide { get; set; }

                    public SelecteWhereItem(string leftSide, string comparisonOperator, string rightSide) {
                        LeftSide = leftSide;
                        ComparisonOperator = comparisonOperator;
                        RightSide = rightSide;
                    }

                    public SelecteWhereItem() {

                    }
                }

                /// <summary>
                /// Where入力リスト初期化
                /// </summary>
                public SelecteWhereItem[] SelecteWhereItemArray { get; set; }

                /// <summary>
                /// Where入力行数
                /// </summary>
                [JsonIgnore]
                public int LineCountForSelectorOfWhere { get; set; }

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
                /// </summary>
                public SelecteWhereItem GetDefaltSortForWhere(int index);


                /// <summary>
                /// 比較演算子一覧のセット
                /// 検索キーキー指示データセット
                /// </summary>
                public void InitKeyArea() {

                    /*
                     *  比較演算子一覧のセット
                     */
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

                    /*
                     *  検索キーキー指示データセット
                     */

                    SelecteWhereItemArray
                        = Enumerable.Range(0, LineCountForSelectorOfWhere).Select(x => GetDefaltSortForWhere(x)).ToArray();
                }
            }
        }

        /// <summary>
        /// データ表示管理用インターふぇおす
        /// </summary>
        public interface IDataAreaClass {

            /// <summary>
            ///  データ表示用リスト
            /// </summary>
            public IEnumerable<T> Lines { get; set; }
        }

    }

}

