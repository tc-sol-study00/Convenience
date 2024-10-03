using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.DataModels {
    /// <summary>
    /// 店頭在庫
    /// </summary>
    /// <Remarks>
    /// 主キー：商品コード
    /// </Remarks>
    [Table("tento_zaiko")]
    [PrimaryKey(nameof(ShohinId))]
    public class TentoZaiko {

        [Column("shohin_code")]
        [DisplayName("商品コード")]
        [MaxLength(10)]
        [Required]
        [Key]
        public string ShohinId { get; set; }

        [Column("zaiko_su")]
        [DisplayName("在庫数")]
        [Precision(7, 2)]
        public decimal ZaikoSu { get; set; }

        [Column("last_shiire_datetime")]
        [DisplayName("直近仕入日時")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateOnly LastShireDateTime { get; set; }

        [Column("last_haraidashi_datetime")]
        [DisplayName("直近払出日時")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? LastHaraidashiDate { get; set; }

        [Column("last_uriage_datetime")]
        [DisplayName("直近売上日時")]
        [DataType(DataType.DateTime)]
        public DateTime? LastUriageDatetime { get; set; }

        [ForeignKey(nameof(ShohinId))]
        public virtual ShohinMaster? ShohinMaster { get; set; }
    }
}