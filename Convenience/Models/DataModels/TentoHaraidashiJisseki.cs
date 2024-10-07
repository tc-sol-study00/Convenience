using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.DataModels {
    /// <summary>
    /// 店頭払出実績DTO
    /// </summary>
    /// <Remarks>
    /// 主キー：仕入先コード、仕入商品コード、商品コード、払出日時
    /// </Remarks>
    [Table("tento_haraidashi_jisseki")]
    [PrimaryKey(nameof(TentoHaraidashiId),nameof(ShiireSakiId), nameof(ShiirePrdId), nameof(ShohinId))]
    public class TentoHaraidashiJisseki {

        [Column("tento_haraidashi_code")]
        [DisplayName("店頭払出コード")]
        [MaxLength(20)]
        [Required]
        public string TentoHaraidashiId { get; set; }
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
        [Key]
        public string ShohinId { get; set; }

        [Column("shiire_datetime")]
        [DisplayName("仕入日時")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateOnly ShireDateTime { get; set; }

        [Column("haraidashi_date")]
        [DisplayName("店頭払出日時")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime HaraidashiDate { get; set; }

        [Column("haraidashi_case_su")]
        [DisplayName("仕入単位店頭払出数")]
        [Range(0, 100, ErrorMessage = "0から100の間の値を入力してください")]
        public int HaraidashiCaseSu { get; set; }

        [Column("haraidashi_su")]
        [DisplayName("店頭払出数")]
        [Precision(7, 2)]
        public decimal HaraidashiSu { get; set; }

        [ForeignKey(nameof(ShiireSakiId) + "," + nameof(ShiirePrdId) + "," + nameof(ShohinId))]
        //[NotMapped]
        public virtual ShiireMaster? ShiireMaster { get; set; }

        [ForeignKey(nameof(TentoHaraidashiId))]
        public virtual TentoHaraidashiHeader? TentoHaraidashiHeader { get; set; }

    }
}