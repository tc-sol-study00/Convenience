using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace Convenience.Models.DataModels {
    /// <summary>
    /// 商品マスタDTO
    /// </summary>
    /// <Remarks>
    /// 主キー：商品コード
    /// </Remarks>
    [Table("shohin_master")]
    public class ShohinMaster : ISelectList {

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

        [Column("shohin_tanka")]
        [DisplayName("商品単価")]
        [Required]
        [Precision(15, 2)]
        public decimal ShohinTanka { get; set; }

        [Column("shohi_zeiritsu")]
        [DisplayName("消費税率（持ち帰り）")]
        [Required]
        [Precision(15, 2)]
        public decimal ShohiZeiritsu { get; set; }

        [Column("shohi_zeiritsu_eatin")]
        [DisplayName("消費税率（イートイン）")]
        [Required]
        [Precision(15, 2)]
        public decimal ShohiZeiritsuEatIn { get; set; }

        public virtual IList<ShiireMaster>? ShiireMasters { get; set; }

        public virtual TentoZaiko? TentoZaiko { get; set; }

        //SelectList用

        public string Value => ShohinId;
        public string Text => ShohinName;

        public string[] OrderKey { get; } = { nameof(ShohinId) };
    }
}