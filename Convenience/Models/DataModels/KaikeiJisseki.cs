using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.DataModels {

    /// <summary>
    /// 会計実績DTO
    /// </summary>
    /// <Remarks>
    /// 主キー：商品コード、売上日時
    /// </Remarks>
    [Table("kaikei_jisseki")]
    [PrimaryKey(nameof(ShohinId), nameof(UriageDatetime))]
    public class KaikeiJisseki {

        [Column("shohin_code")]
        [DisplayName("商品コード")]
        [MaxLength(10)]
        [Required]
        public string ShohinId { get; set; }

        [Column("uriage_datetime")]
        [DisplayName("売上日時")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime UriageDatetime { get; set; }

        [Column("uriage_su")]
        [DisplayName("売上数量")]
        [Required]
        [Precision(10, 2)]
        public decimal UriageSu { get; set; }

        [Column("uriage_kingaku")]
        [DisplayName("売上金額")]
        [Required]
        [Precision(15, 2)]
        public decimal UriageKingakuSu { get; set; }

        [Column("zeikomi_kingaku")]
        [DisplayName("税込金額")]
        [Required]
        [Precision(15, 2)]
        public decimal ZeikomiKingaku { get; set; }

        [Column("shohi_zeiritsu")]
        [DisplayName("消費税率")]
        [Required]
        [Precision(15, 2)]
        public decimal ShohiZeiritsu { get; set; }

        [ForeignKey(nameof(ShohinId))]
        public virtual ShohinMaster ShohinMaster { get; set; }
    }
}