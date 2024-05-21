using Convenience.Models.DataModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Convenience.Models.ViewModels.Zaiko {

    public class ZaikoViewModel {

        [DisplayName("ソートキー")]
        [MaxLength(20)]
        [Required]
        public string KeyEventData { get; set; }

        public SelectList KeyList = new SelectList(
            new List<SelectListItem>
                {
                    new SelectListItem { Value = nameof(SokoZaiko.ShiireSakiId), Text = "仕入先コード" },
                    new SelectListItem { Value = nameof(SokoZaiko.ShiirePrdId), Text = "仕入商品コード" },
                    new SelectListItem { Value = nameof(SokoZaiko.ShohinId), Text = "商品コード" },
                    new SelectListItem { Value = nameof(SokoZaiko.ShiireMaster.ShohinMaster.ShohinName), Text = "商品名" },
                    new SelectListItem { Value = nameof(SokoZaiko.SokoZaikoCaseSu), Text = "在庫数" },
                    new SelectListItem { Value = nameof(SokoZaiko.SokoZaikoSu), Text = "倉庫在庫数" },
                    new SelectListItem { Value = nameof(SokoZaiko.SokoZaikoSu), Text = "直近仕入日" },
                    new SelectListItem { Value = nameof(SokoZaiko.SokoZaikoSu), Text = "直近払出日" },
                }, "Value", "Text");

        public bool Descending { get; set; } = false;

        public IList<SokoZaiko> sokoZaikos { get; set; }
    }
}