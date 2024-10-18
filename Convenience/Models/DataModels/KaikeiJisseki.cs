using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.Kaikei;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace Convenience.Models.DataModels {

    /// <summary>
    /// 会計実績DTO
    /// </summary>
    /// <Remarks>
    /// 主キー：商品コード、売上日時
    /// </Remarks>
    [Table("kaikei_jisseki")]
    [PrimaryKey(nameof(UriageDatetimeId),nameof(ShohinId), nameof(UriageDatetime))]
    public class KaikeiJisseki : IKaikeiJissekiForAdd {

        [Column("uriage_datetimeid")]
        [DisplayName("売上日時コード")]
        [MaxLength(20)]
        [Required]
        public string UriageDatetimeId { get; set; }

        [Column("shohin_code")]
        [DisplayName("商品コード")]
        [MaxLength(10)]
        [Required]
        public string? ShohinId { get; set; }

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
        public decimal UriageKingaku { get; set; }

        [Column("zeikomi_kingaku")]
        [DisplayName("税込金額")]
        [Required]
        [Precision(15, 2)]
        public decimal ZeikomiKingaku { get; set; }

        [Column("shohin_tanka")]
        [DisplayName("商品単価")]
        [Required]
        [Precision(15, 2)]
        public decimal ShohinTanka { get; set; }

        [Column("shohi_zeiritsu")]
        [DisplayName("消費税率")]
        [Required]
        [Precision(15, 2)]
        public decimal ShohiZeiritsu { get; set; }

        [Column("naigai_class")]
        [DisplayName("内外区分")]
        [Required]
        [MaxLength(1)]
        public string NaigaiClass { get; set; } = "0";

        [Column("kaikei_seq")]
        [DisplayName("会計順位")]
        [Required]
        public int KaikeiSeq { get; set; }

        [ForeignKey(nameof(UriageDatetimeId))]
        public  KaikeiHeader KaikeiHeader { get; set; }

        [ForeignKey(nameof(ShohinId))]
        public ShohinMaster? ShohinMaster { get; set; }

        [ForeignKey(nameof(NaigaiClass))]
        public NaigaiClassMaster? NaigaiClassMaster { get; set; }

        public TentoZaiko? TentoZaiko { get; set; }
    }
}