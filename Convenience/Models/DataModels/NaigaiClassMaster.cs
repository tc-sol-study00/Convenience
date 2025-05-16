using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace Convenience.Models.DataModels {

    /// <summary>
    /// 内外区分マスタＤＴＯ
    /// </summary>
    /// <Remarks>
    /// 主キー：内外区分
    /// </Remarks>
    [Table("naigai_class_master")]
    public class NaigaiClassMaster {

        [Column("naigai_class")]
        [DisplayName("内外区分")]
        [Required]
        [MaxLength(1)]
        [Key]
        public string NaigaiClass { get; set; }

        [Column("naigai_class_name")]
        [DisplayName("内外区分名称")]
        [Required]
        [MaxLength(30)]
        public string NaigaiClassName { get; set; }
        public virtual IEnumerable<KaikeiJisseki> KaikeiJissekis { get; set; }
    }
}
