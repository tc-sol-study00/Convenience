using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Convenience.Models.ViewModels.Shiire {
    /// <summary>
    /// 仕入キービューモデル
    /// </summary>
    public class ShiireKeysViewModel {

        [DisplayName("注文コード")]
        [MaxLength(20)]
        [Required]
        public string? ChumonId { get; set; }

        /// <summary>
        /// 注文コードリスト（注文残が残っているリスト格納用）
        /// </summary>
        public IList<SelectListItem> ChumonIdList { get; set; } = new List<SelectListItem>();
    }
}