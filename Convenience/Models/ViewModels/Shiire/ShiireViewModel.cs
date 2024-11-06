using Convenience.Models.DataModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace Convenience.Models.ViewModels.Shiire {
    /// <summary>
    /// 仕入ビューモデル
    /// </summary>
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
        
        /// <summary>
        /// 仕入実績
        /// </summary>
        public IList<DataModels.ShiireJisseki> ShiireJissekis { get; set; }
        /// <summary>
        /// 処理が正常がどうか（正常=true)
        /// </summary>
        public bool? IsNormal { get; set; }
        /// <summary>
        /// 処理結果（ＤＢ反映結果）表示内容
        /// </summary>
        public string Remark { get; set; }
    }
}