using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace Convenience.Models.DataModels {
    /// <summary>
    /// 仕入マスタDTO
    /// </summary>
    /// <Remarks>
    /// 主キー：注文コード、仕入日付、仕入SEQ、仕入先コード、仕入商品コード
    /// </Remarks>
    [Table("shiire_master")]
    [PrimaryKey(nameof(ShiireSakiId), nameof(ShiirePrdId), nameof(ShohinId))]
    public class ShiireMaster {

        [Column("shiire_saki_code")]
        [DisplayName("仕入先コード")]
        [MaxLength(10)]
        [Required]
        public string ShiireSakiId { get; set; }

        [Column("shiire_prd_code")]
        [DisplayName("仕入商品コード")]
        [MaxLength(10)]
        [Required]
        public string ShiirePrdId { get; set; }

        [Column("shohin_code")]
        [DisplayName("商品コード")]
        [MaxLength(10)]
        [Required]
        public string ShohinId { get; set; }

        [Column("shiire_prd_name")]
        [DisplayName("仕入商品名称")]
        [MaxLength(30)]
        [Required]
        public string ShiirePrdName { get; set; }

        [Column("shiire_pcs_unit")]
        [DisplayName("仕入単位における数量")]
        [Required]
        [Precision(7, 2)]
        public decimal ShiirePcsPerUnit { get; set; }

        [Column("shiire_unit")]
        [DisplayName("仕入単位")]
        [MaxLength(10)]
        [Required]
        public string ShiireUnit { get; set; }

        [Column("shiire_tanka")]
        [DisplayName("仕入単価")]
        [Required]
        [Precision(7, 2)]
        public decimal ShireTanka { get; set; }

        [ForeignKey(nameof(ShohinId))]
        public virtual ShohinMaster? ShohinMaster { get; set; }

        [ForeignKey(nameof(ShiireSakiId))]
        public virtual ShiireSakiMaster? ShiireSakiMaster { get; set; }

        public virtual ICollection<ChumonJissekiMeisai>? ChumonJissekiMeisaiis { get; set; }

        public virtual SokoZaiko? SokoZaiko { get; set; }

        public virtual IList<TentoHaraidashiJisseki>? TentoHaraidashiJissekis { get; set; }
    }
}