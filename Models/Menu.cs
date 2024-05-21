namespace Convenience.Models {
    public class MenuItem {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }

    public class Menu {
        public List<MenuItem> MenuList { get; set; } = new List<MenuItem>
        {
            new MenuItem { Name = "ホーム", Url = "/Home/Index", Description="このページに戻ってきます" },
            new MenuItem { Name = "商品注文", Url = "/Chumon/KeyInput" , Description="仕入先毎に注文を作成します。一日一回注文を起こし仕入先単位で注文コードを発番します。注文番号を指定して注文内容を修正することもできます。"},
            new MenuItem { Name = "仕入入力", Url = "/Shiire/ShiireKeyInput" , Description="仕入先毎に注文を作成します。注文コードを指定し注文済みの内容に対して、仕入数量を入力し、仕入数量分倉庫在庫に登録し、注文残を調整します。"},
            new MenuItem { Name = "在庫検索", Url = "/Zaiko/Index" , Description = "仕入後の商品出し前の倉庫在庫が検索できます"}
        };
    } 
}
