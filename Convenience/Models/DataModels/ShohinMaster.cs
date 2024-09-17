using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.DataModels {

    [Table("shohin_master")]
    public class ShohinMaster {

        [Column("shohin_code")]
        [DisplayName("商品コード")]
        [MaxLength(10)]
        [Required]
        [Key]
        public string ShohinId { get; set; }

        [Column("shohin_name")]
        [DisplayName("商品名称")]
        [MaxLength(50)]
        [Required]
        public string ShohinName { get; set; }

        [Column("shohi_zeiritsu")]
        [DisplayName("消費税率")]
        [Required]
        [Precision(15, 2)]
        public decimal ShohiZeiritsu { get; set; }

        [Column("shohi_zeiritsu_gaisyoku")]
        [DisplayName("消費税率（外食）")]
        [Required]
        [Precision(15, 2)]
        public decimal ShohiZeiritsuGaishoku { get; set; }

        public virtual ICollection<ShiireMaster>? ShiireMasters { get; set; }

        public virtual TentoZaiko? TentoZaikos { get; set; }
    }
}