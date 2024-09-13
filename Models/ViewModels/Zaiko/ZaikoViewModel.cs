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

    public class ZaikoViewModel {

        public ZaikoViewModel() {

        }

        //ソートキー選択結果セット用
        [DisplayName("ソートキー")]

        public class KeyEventRec {
            [DisplayName("ソート項目")]
            public string? KeyEventData { get; set; }
            [DisplayName("昇順・降順")]
            public bool Descending { get; set; } = false;
        }

        const int LineCountForSelectorOfOderBy = 3;    //ソート用３つまで入力ＯＫ
        public KeyEventRec[] KeyEventList { get; set; } = Enumerable.Range(0, LineCountForSelectorOfOderBy).Select(_ => new KeyEventRec()).ToArray();

        //ソート指示選択用一覧（画面表示と選択用）
        public SelectList KeyList = new SelectList(
            new List<SelectListItem>
                {
                    new SelectListItem { Value = nameof(ZaikoListLine.ShiireSakiId), Text = "仕入先コード" },
                    new SelectListItem { Value = nameof(ZaikoListLine.ShiirePrdId), Text = "仕入商品コード" },
                    new SelectListItem { Value = nameof(ZaikoListLine.ShohinId), Text = "商品コード" },
                    new SelectListItem { Value = nameof(ZaikoListLine.ShohinName), Text = "商品名" },
                    new SelectListItem { Value = nameof(ZaikoListLine.SokoZaikoCaseSu), Text = "在庫数" },
                    new SelectListItem { Value = nameof(ZaikoListLine.SokoZaikoSu), Text = "倉庫在庫数" },
                    new SelectListItem { Value = nameof(ZaikoListLine.LastShiireDate), Text = "直近仕入日" },
                    new SelectListItem { Value = nameof(ZaikoListLine.LastDeliveryDate), Text = "直近払出日" },
                    new SelectListItem { Value = nameof(ZaikoListLine.ChumonZan), Text = "注文残" },
                }, "Value", "Text");


        //Where指示選択結果セット用
        public class SelecteWhereItem {
            [DisplayName("検索項目項目")]
            public string? LeftSide { get; set; }

            [DisplayName("比較")]
            [MaxLength(2)]
            public string? ComparisonOperator { get; set; } = "==";
            [DisplayName("検索キー")]
            public string? RightSide { get; set; }
        }

        //比較演算子選択用
        public SelectList ComparisonOperatorList = new SelectList(
            new List<SelectListItem> {
            new SelectListItem { Value = "==", Text = "=" },
            new SelectListItem { Value = "!=", Text = "!=" },
            new SelectListItem { Value = ">=", Text = ">=" },
            new SelectListItem { Value = ">", Text = ">" },
            new SelectListItem { Value = "<=", Text = "<=" },
            new SelectListItem { Value = "<", Text = "<" },
        }, "Value", "Text");

        const int LineCountForSelectorOfWhere = 6; //Where入力６行
        public SelecteWhereItem[] SelecteWhereItemArray { get; set; } = Enumerable.Range(0, LineCountForSelectorOfWhere).Select(_ => new SelecteWhereItem()).ToArray();

        //Where左辺用カラムセット用
        public SelectList SelectWhereLeftSideList = new SelectList(
                new List<SelectListItem>
                    {
                    new SelectListItem { Value = nameof(ZaikoListLine.ShiireSakiId), Text = "仕入先コード" },
                    new SelectListItem { Value = nameof(ZaikoListLine.ShiirePrdId), Text = "仕入商品コード" },
                    new SelectListItem { Value = nameof(ZaikoListLine.ShohinId), Text = "商品コード" },
                    new SelectListItem { Value = nameof(ZaikoListLine.ShohinName), Text = "商品名" },
                    new SelectListItem { Value = nameof(ZaikoListLine.SokoZaikoCaseSu), Text = "在庫数" },
                    new SelectListItem { Value = nameof(ZaikoListLine.SokoZaikoSu), Text = "倉庫在庫数" },
                    new SelectListItem { Value = nameof(ZaikoListLine.LastShiireDate), Text = "直近仕入日" },
                    new SelectListItem { Value = nameof(ZaikoListLine.LastDeliveryDate), Text = "直近払出日" },
                    new SelectListItem { Value = nameof(ZaikoListLine.ChumonZan), Text = "注文残" },
                    }, "Value", "Text");

        //倉庫在庫・注文実績明細変策用レコード
        public class ZaikoListLine {
            [DisplayName("仕入先コード")]
            public string ShiireSakiId { get; set; }
            [DisplayName("仕入商品コード")]
            public string ShiirePrdId { get; set; }
            [DisplayName("商品コード")]
            public string ShohinId { get; set; }
            [DisplayName("商品名称")]
            public string ShohinName { get; set; }
            [DisplayName("仕入単位在庫数")]
            public decimal SokoZaikoCaseSu { get; set; }
            [DisplayName("在庫数")]
            public decimal SokoZaikoSu { get; set; }
            [DisplayName("直近仕入日")]
            public DateOnly LastShiireDate { get; set; }
            [DisplayName("直近払出日")]
            public DateOnly? LastDeliveryDate { get; set; }
            [DisplayName("注文残")]
            public decimal ChumonZan { get; set; }
        }
        //倉庫在庫・注文実績明細変策用レコードのリスト（検索結果がここに入る）
        public IList<ZaikoListLine> zaikoListLine { get; set; } = new List<ZaikoListLine>();
    }
}