using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.DataModels {

    [Table("chumon_jisseki")]
    [PrimaryKey(nameof(ChumonId))]
    public class ChumonJisseki {

        [Column("chumon_code")]
        [DisplayName("注文コード")]
        [MaxLength(20)]
        [Required]
        public string ChumonId { get; set; }

        [Column("shiire_saki_code")]
        [DisplayName("仕入先コード")]
        [MaxLength(10)]
        [Required]
        public string ShiireSakiId { get; set; }

        [Column("chumon_date")]
        [DisplayName("注文日")]
        [Required]
        public DateOnly ChumonDate { get; set; }

        [ForeignKey(nameof(ShiireSakiId))]
        public virtual ShiireSakiMaster? ShiireSakiMaster { get; set; }

        public virtual IList<ChumonJissekiMeisai>? ChumonJissekiMeisais { get; set; } = new List<ChumonJissekiMeisai>() { };

        [Timestamp]
        public uint Version { get; set; }
    }
}