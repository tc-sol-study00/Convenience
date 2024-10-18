using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace Convenience.Models.DataModels {
    /// <summary>
    /// 店頭払出ヘッダー実績DTO
    /// </summary>
    /// <Remarks>
    /// 主キー：仕入先コード、仕入商品コード、商品コード、払出日時
    /// </Remarks>
    [Table("tento_haraidashi_header")]
    [PrimaryKey(nameof(TentoHaraidashiId))]
    public class TentoHaraidashiHeader {

        [Column("tento_haraidashi_code")]
        [DisplayName("店頭払出コード")]
        [MaxLength(20)]
        [Required]
        public string TentoHaraidashiId { get; set; }

        [Column("haraidashi_datetime")]
        [DisplayName("払出日時")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime HaraidashiDateTime { get; set; }

        public virtual IList<TentoHaraidashiJisseki> TentoHaraidashiJissekis { get; set;}

    }
}