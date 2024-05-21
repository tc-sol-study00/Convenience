using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Convenience.Models.ViewModels.Shiire {

    public class ShiireKeysViewModel {

        [DisplayName("注文コード")]
        [MaxLength(20)]
        [Required]
        public string ChumonId { get; set; }

        public IList<SelectListItem> ChumonIdList { get; set; }
    }
}