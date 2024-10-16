using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.DataModels {

    /// <summary>
    /// 会計ヘッダーDTO
    /// </summary>
    /// <Remarks>
    /// 主キー：売上日時コード
    /// </Remarks>
    [Table("kaikei_header")]
    [PrimaryKey(nameof(UriageDatetimeId))]
    public class KaikeiHeader {

        [Column("uriage_datetimeid")]
        [DisplayName("売上日時コード")]
        [MaxLength(20)]
        [Required]
        public string UriageDatetimeId { get; set; }

        [Column("uriage_datetime")]
        [DisplayName("売上日時")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime UriageDatetime { get; set; }
        public IList<KaikeiJisseki> KaikeiJissekis { get; set; } 
    }
}