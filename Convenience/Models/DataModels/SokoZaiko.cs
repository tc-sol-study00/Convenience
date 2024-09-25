using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.DataModels {
    /// <summary>
    /// 倉庫在庫DTO
    /// </summary>
    /// <Remarks>
    /// 主キー：仕入先コード、仕入商品コード、商品コード
    /// </Remarks>
    [Table("soko_zaiko")]
    [PrimaryKey(nameof(ShiireSakiId), nameof(ShiirePrdId), nameof(ShohinId))]
    public class SokoZaiko {

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

        [Column("soko_zaiko_case_su")]
        [DisplayName("仕入単位在庫数")]
        [Precision(10, 2)]
        public decimal SokoZaikoCaseSu { get; set; }

        [Column("soko_zaiko_su")]
        [DisplayName("在庫数")]
        [Precision(10, 2)]
        public decimal SokoZaikoSu { get; set; }

        [Column("last_shiire_date")]
        [DisplayName("直近仕入日")]
        [Required]
        public DateOnly LastShiireDate { get; set; }

        [Column("last_delivery_date")]
        [DisplayName("直近払出日")]
        public DateOnly? LastDeliveryDate { get; set; }

        [ForeignKey(nameof(ShiireSakiId) + "," + nameof(ShiirePrdId) + "," + nameof(ShohinId))]
        public virtual ShiireMaster? ShiireMaster { get; set; }

        [NotMapped]
        public virtual IList<ShiireJisseki>? ShiireJissekis { get; set; }

        [Timestamp]
        public uint Version { get; set; }
    }
}