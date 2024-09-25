using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.DataModels {
    /// <summary>
    /// 仕入実績実績DTO
    /// </summary>
    /// <Remarks>
    /// 主キー：注文コード、仕入日付、仕入SEQ、仕入先コード、仕入商品コード
    /// </Remarks>
    [Table("shiire_jisseki")]
    [PrimaryKey(nameof(ChumonId), nameof(ShiireDate), nameof(SeqByShiireDate), nameof(ShiireSakiId), nameof(ShiirePrdId))]
    public class ShiireJisseki {

        [Column("chumon_code")]
        [DisplayName("注文コード")]
        [MaxLength(20)]
        [Required]
        public string ChumonId { get; set; }

        [Column("shiire_date")]
        [DisplayName("仕入日付")]
        [Required]
        public DateOnly ShiireDate { get; set; }

        [Column("seq_by_shiiredate")]
        [DisplayName("仕入SEQ")]
        [Required]
        public uint SeqByShiireDate { get; set; }

        [Column("shiire_datetime")]
        [DisplayName("仕入日時")]
        [Required]
        public DateTime ShiireDateTime { get; set; }

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

        [Column("nonyu_su")]
        [DisplayName("納入数")]
        [Precision(10, 2)]
        public decimal NonyuSu { get; set; }

        [NotMapped]
        [DisplayName("納入差")]
        [Precision(10, 2)]
        public decimal? NonyuSubalance { get; set; }

        [ForeignKey(nameof(ChumonId) + "," + nameof(ShiireSakiId) + "," + nameof(ShiirePrdId) + "," + nameof(ShohinId))]
        public ChumonJissekiMeisai ChumonJissekiMeisaii { get; set; }

        [NotMapped]
        [ForeignKey(nameof(ShiireSakiId) + "," + nameof(ShiirePrdId) + "," + nameof(ShohinId))]
        public virtual SokoZaiko? SokoZaiko { get; set; }

        [Timestamp]
        public uint Version { get; set; }
    }
}