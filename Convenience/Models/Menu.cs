﻿#pragma warning disable CS8618
using System.Data.SqlTypes;

namespace Convenience.Models {
    /// <summary>
    /// メニューアイテムクラス
    /// </summary>
    public class MenuItem  {
        /// <summary>
        /// メニュー名
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// 起動ＵＲＬ
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// メニュー内容説明
        /// </summary>
        public string Description { get; set; }

        public IList<MenuItem>? ChildrenMenus { get; set; } = new List<MenuItem>();
    }
    /// <summary>
    /// メニュークラス
    /// </summary>
    public class Menu {
        /// <summary>
        /// メニュー表示用リスト（メニューアイテムリスト）
        /// </summary>
        public List<MenuItem> MenuList { get; set; } = new List<MenuItem>
        {
            new (){
                Name = "ホーム", Url = "/Home/Index", Description="このページに戻ってきます"

            },
            new (){
               Name = "業務メニュー",
               ChildrenMenus = new List<MenuItem>() {
                        new (){ Name = "商品注文", Url = "/Chumon/KeyInput" , Description="仕入先毎に注文を作成します。一日一回注文を起こし仕入先単位で注文コードを発番します。注文番号を指定して注文内容を修正することもできます。" },
                        new (){ Name = "仕入入力", Url = "/Shiire/ShiireKeyInput" , Description="仕入先毎に注文を作成します。注文コードを指定し注文済みの内容に対して、仕入数量を入力し、仕入数量分倉庫在庫に登録し、注文残を調整します。" },
                        new (){ Name = "店頭払出", Url = "/TentoHaraidashi/KeyInput" , Description = "倉庫在庫を店頭にケース単位で払い出します。払い出し後は店頭在庫になります。倉庫在庫が減少し、店頭在庫が増加します。" },
                        new (){ Name = "会計入力", Url = "/Kaikei/KeyInput" , Description = "お客様へ会計を行います。売り上げの分だけ店頭在庫が減ります。" }
                    }
                },
            new (){
               Name = "検索メニュー",
               ChildrenMenus = new List<MenuItem>() {
                        new (){ Name = "倉庫在庫検索", Url = "/Zaiko/Index" , Description = "仕入後の商品出し前の倉庫在庫が検索できます" },
                        new (){ Name = "店頭在庫検索", Url = "/TentoZaiko/Index" , Description = "店頭払い出し後の店頭在庫が検索できます" },
                        new (){ Name = "注文実績検索", Url = "/ChumonJisseki/Index" , Description = "日々の注文実績が検索できます"},
                        new (){ Name = "仕入実績検索", Url = "/ShiireJisseki/Index" , Description = "日々の仕入実績が検索できます"},
                        new (){ Name = "会計実績検索", Url = "/KaikeiJisseki/Index" , Description = "日々の会計実績が検索できます"},
                    }
                },
            new (){
                Name = "マスターメニュー",
                ChildrenMenus = new List<MenuItem>() {
                    new (){ Name = "商品マスタメンテナンス", Url = "/ShohinMaster/Index" , Description = "商品マスタの登録・編集・削除ができます"},
                    new (){ Name = "仕入先マスタメンテナンス", Url = "/ShiireSakiMaster/Index" , Description = "仕入先マスタの登録・編集・削除ができます"},
                    new (){ Name = "仕入マスタメンテナンス", Url = "/ShiireMaster/Index" , Description = "仕入マスタの登録・編集・削除ができます"},
                    new (){ Name = "内外区分マスタメンテナンス", Url = "/NaigaiClassMaster/Index" , Description = "内外区分マスタの登録・編集・削除ができます"}
                    }
            }
        };
    }
}


