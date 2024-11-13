using Convenience.Models.DataModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Convenience.Models.ViewModels.Zaiko {
    /// <summary>
    /// 在庫検索用ビューモデル
    /// </summary>
    public class ZaikoViewModel {
        /// <summary>
        /// コンストラクタ（なし）
        /// </summary>
        public ZaikoViewModel() {
        }

        /// <summary>
        /// ソートキー選択結果セット用
        /// </summary>
        [DisplayName("ソートキー")]
        public class KeyEventRec {
            [DisplayName("ソート項目")]
            public string? KeyEventData { get; set; }
            [DisplayName("昇順・降順")]
            public bool Descending { get; set; } = false;
        }
        /// <summary>
        /// ソート指示項目の最大値
        /// </summary>
        const int LineCountForSelectorOfOderBy = 3;    //ソート用３つまで入力ＯＫ
        /// <summary>
        /// ソートキー選択結果セット用を使ってリストを初期化
        /// </summary>
        public KeyEventRec[] KeyEventList { get; set; } = Enumerable.Range(0, LineCountForSelectorOfOderBy).Select(_ => new KeyEventRec()).ToArray();

        /// <summary>
        /// ソート指示選択用一覧（画面表示と選択用）
        /// </summary>
        public SelectList KeyList = new(
            new List<SelectListItem>
                {
                    new() { Value = nameof(ZaikoListLine.ShiireSakiId), Text = "仕入先コード" },
                    new() { Value = nameof(ZaikoListLine.ShiirePrdId), Text = "仕入商品コード" },
                    new() { Value = nameof(ZaikoListLine.ShohinId), Text = "商品コード" },
                    new() { Value = nameof(ZaikoListLine.ShohinName), Text = "商品名" },
                    new() { Value = nameof(ZaikoListLine.SokoZaikoCaseSu), Text = "在庫数" },
                    new() { Value = nameof(ZaikoListLine.SokoZaikoSu), Text = "倉庫在庫数" },
                    new() { Value = nameof(ZaikoListLine.LastShiireDate), Text = "直近仕入日" },
                    new() { Value = nameof(ZaikoListLine.LastDeliveryDate), Text = "直近払出日" },
                    new() { Value = nameof(ZaikoListLine.ChumonZan), Text = "注文残" },
                }, "Value", "Text");

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
        public SelectList ComparisonOperatorList = new(
            new List<SelectListItem> {
            new (){ Value = "==", Text = "=" },
            new (){ Value = "!=", Text = "!=" },
            new (){ Value = ">=", Text = ">=" },
            new (){ Value = ">", Text = ">" },
            new (){ Value = "<=", Text = "<=" },
            new (){ Value = "<", Text = "<" },
        }, "Value", "Text");

        /// <summary>
        /// Where入力行数
        /// </summary>
        const int LineCountForSelectorOfWhere = 6; //Where入力６行
        /// <summary>
        /// Where入力リスト初期化
        /// </summary>
        public SelecteWhereItem[] SelecteWhereItemArray { get; set; } = Enumerable.Range(0, LineCountForSelectorOfWhere).Select(_ => new SelecteWhereItem()).ToArray();

        /// <summary>
        /// Where左辺用カラムセット用
        /// </summary>
        public SelectList SelectWhereLeftSideList = new (
                new List<SelectListItem>
                    {
                    new() { Value = nameof(ZaikoListLine.ShiireSakiId), Text = "仕入先コード" },
                    new() { Value = nameof(ZaikoListLine.ShiirePrdId), Text = "仕入商品コード" },
                    new() { Value = nameof(ZaikoListLine.ShohinId), Text = "商品コード" },
                    new() { Value = nameof(ZaikoListLine.ShohinName), Text = "商品名" },
                    new() { Value = nameof(ZaikoListLine.SokoZaikoCaseSu), Text = "在庫数" },
                    new() { Value = nameof(ZaikoListLine.SokoZaikoSu), Text = "倉庫在庫数" },
                    new() { Value = nameof(ZaikoListLine.LastShiireDate), Text = "直近仕入日" },
                    new() { Value = nameof(ZaikoListLine.LastDeliveryDate), Text = "直近払出日" },
                    new() { Value = nameof(ZaikoListLine.ChumonZan), Text = "注文残" },
                    }, "Value", "Text");

#pragma warning disable CS8618
        /// <summary>
        /// 倉庫在庫・注文実績明細変策用レコード
        /// </summary>
        public class ZaikoListLine {
            [DisplayName("仕入先コード")]
            public string ShiireSakiId { get; set; }
            [DisplayName("仕入商品コード")]
            public string ShiirePrdId { get; set; }
            [DisplayName("商品コード")]
            public string ShohinId { get; set; }
            [DisplayName("商品名称")]
            public string? ShohinName { get; set; }
            [DisplayName("仕入単位在庫数")]
            public decimal SokoZaikoCaseSu { get; set; }
            [DisplayName("在庫数")]
            public decimal SokoZaikoSu { get; set; }
            [DisplayName("直近仕入日")]
            public DateOnly LastShiireDate { get; set; }
            [DisplayName("直近払出日")]
            public DateOnly? LastDeliveryDate { get; set; }
            [DisplayName("注文残")]
            public decimal? ChumonZan { get; set; }
        }

#pragma warning restore CS8618

        /// <summary>
        /// 倉庫在庫・注文実績明細変策用レコードのリスト（検索結果がここに入る）
        /// </summary>
        public IList<ZaikoListLine> ZaikoListLines { get; set; } = new List<ZaikoListLine>();

        public ZaikoListLine ZaikoLine { get; set; } = new ZaikoListLine();
    }
}