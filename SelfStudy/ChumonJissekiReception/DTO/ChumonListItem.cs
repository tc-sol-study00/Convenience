namespace SelfStudy.ChumonJissekiReception.DTO {

    /// <summary>
    /// 注文残リスト構成
    /// </summary>
    public class ChumonListItem {
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public required string ShiireSakiId { get; set; }
        /// <summary>
        /// 注文コード
        /// </summary>
        public required string ChumonId { get; set; }
        /// <summary>
        /// 注文残
        /// </summary>
        public required decimal ChumonZan { get; set; }
    }
}