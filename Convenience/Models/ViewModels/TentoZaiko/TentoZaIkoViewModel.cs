using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel.DataAreaClass;
using Newtonsoft.Json;
using Convenience.Models.Interfaces;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel.DataAreaClass.TentoZaIkoLine>;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel.DataAreaClass.TentoZaIkoLine>.IKeywordAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel.DataAreaClass.TentoZaIkoLine>.IKeywordAreaClass.ISortAreaClass;
using static Convenience.Models.Interfaces.IRetrivalViewModel<Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel.DataAreaClass.TentoZaIkoLine>.IKeywordAreaClass.IKeyAreaClass;
using static Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Convenience.Models.ViewModels.TentoZaiko {

    /// <summary>
    /// 店頭在庫検索ビューモデル
    /// </summary>
    public class TentoZaikoViewModel : IRetrivalViewModel<TentoZaIkoLine> {

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
        public TentoZaikoViewModel() {
            KeywordArea = new KeywordAreaClass();
            DataArea = new DataAreaClass();
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
                /// </summary>
                public SortEventRec GetDefaltSortForSort(int index) {
                    return index switch {
                        0 => new SortEventRec(nameof(TentoZaIkoLine.ShohinId), false),
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

                    KeyList = new SelectList(
                        new List<SelectListItem>
                        {
                            new() { Value = nameof(TentoZaIkoLine.ShohinId), Text = ISharedTools.GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.ShohinId)) },
                            new() { Value = nameof(TentoZaIkoLine.ShohinName), Text = ISharedTools.GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.ShohinName)) },
                            new() { Value = nameof(TentoZaIkoLine.ZaikoSu), Text = ISharedTools.GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.ZaikoSu)) },
                            new() { Value = nameof(TentoZaIkoLine.LastShireDateTime), Text = ISharedTools.GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.LastShireDateTime)) },
                            new() { Value = nameof(TentoZaIkoLine.LastHaraidashiDate), Text = ISharedTools.GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.LastHaraidashiDate)) },
                            new() { Value = nameof(TentoZaIkoLine.LastUriageDatetime), Text = ISharedTools.GetDisplayName(typeof(TentoZaIkoLine), nameof(TentoZaIkoLine.LastUriageDatetime)) },
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
                /// </summary>
                public SelecteWhereItem GetDefaltSortForWhere(int index) {
                    return new SelecteWhereItem();
                }
                public KeyAreaClass() {
                    /*
                     * 初期化
                     */
                    SelecteWhereItemArray = new SelecteWhereItem[LineCountForSelectorOfWhere];
                    ComparisonOperatorList = new SelectList(new List<SelectListItem>());

                    /*
                     * 検索キー指示データ初期データセット
                     */

                    //処理なし

                    /*
                     *  検索キー一覧表示セット
                     */
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
            public IEnumerable<TentoZaIkoLine> Lines { get; set; }

            /// <summary>
            /// コンストラクタ
            ///  データ表示用リストの初期化
            /// </summary>
            public DataAreaClass() {
                Lines = new List<TentoZaIkoLine>();
            }

            /// <summary>
            /// データ表示用リストの１レコード定義
            /// </summary>
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
