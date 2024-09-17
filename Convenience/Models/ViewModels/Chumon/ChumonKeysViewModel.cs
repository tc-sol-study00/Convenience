using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.ViewModels.Chumon {

    public class ChumonKeysViewModel {

        [Column("shiire_saki_code")]
        [DisplayName("仕入先コード")]
        [MaxLength(10)]
        [Required]
        public string ShiireSakiId { get; set; }

        [Column("chumon_date")]
        [DisplayName("注文日")]
        public DateOnly ChumonDate { get; set; }

        public List<SelectListItem>? ShiireSakiList { get; set; }
    }
}