using Convenience.Models.DataModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Convenience.Models.ViewModels.Shiire {

    public class ShiireViewModel {

        [DisplayName("注文コード")]
        [MaxLength(20)]
        public string ChumonId { get; set; }

        [DisplayName("注文日")]
        public DateOnly ChumonDate { get; set; }

        [DisplayName("仕入日")]
        public DateOnly ShiireDate { get; set; }

        [DisplayName("仕入SEQ")]
        public uint SeqByShiireDate { get; set; }

        [DisplayName("仕入先コード")]
        [MaxLength(10)]
        public string ShiireSakiId { get; set; }

        [DisplayName("仕入先会社")]
        [MaxLength(30)]
        public string ShiireSakiKaisya { get; set; }

        public IList<ShiireJisseki> ShiireJissekis { get; set; }

        public string Remark { get; set; }
    }
}