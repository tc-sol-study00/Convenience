using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.DataModels {

    [Table("shiire_saki_master")]
    public class ShiireSakiMaster {

        [Column("shiire_saki_code")]
        [DisplayName("仕入先コード")]
        [MaxLength(10)]
        [Required]
        [Key]
        public string ShiireSakiId { get; set; }

        [Column("shiire_saki_kaisya")]
        [DisplayName("仕入先会社")]
        [MaxLength(30)]
        [Required]
        public string ShiireSakiKaisya { get; set; }

        [Column("shiire_saki_busho")]
        [DisplayName("仕入先部署")]
        [MaxLength(30)]
        [Required]
        public string ShiireSakiBusho { get; set; }

        [Column("yubin_bango")]
        [DisplayName("郵便番号")]
        [MaxLength(30)]
        [Required]
        [DataType(DataType.PostalCode)]
        public string YubinBango { get; set; }

        [Column("todoufuken")]
        [DisplayName("都道府県")]
        [MaxLength(10)]
        [Required]
        public string Todoufuken { get; set; }

        [Column("shikuchoson")]
        [DisplayName("市区町村")]
        [MaxLength(10)]
        [Required]
        public string Shikuchoson { get; set; }

        [Column("banchi")]
        [DisplayName("番地")]
        [MaxLength(10)]
        [Required]
        public string Banchi { get; set; }

        [Column("tatemonomei")]
        [DisplayName("建物名・部屋番号")]
        [MaxLength(10)]
        [Required]
        public string Tatemonomei { get; set; }

        public virtual ICollection<ShiireMaster>? ShireMasters { get; set; }

        public virtual ICollection<ChumonJisseki>? ChumonJissekis { get; set; }
    }
}