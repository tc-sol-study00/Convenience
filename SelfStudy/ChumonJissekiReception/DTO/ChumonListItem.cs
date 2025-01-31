namespace SelfStudy.ChumonJissekiReception.DTO {

    /// <summary>
    /// 注文残リスト構成
    /// </summary>
    public class ChumonListItem {
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string ShiireSakiId { get; set; }
        /// <summary>
        /// 注文コード
        /// </summary>
        public string ChumonId { get; set; }
        /// <summary>
        /// 注文残
        /// </summary>
        public decimal ChumonZan { get; set; }
    }
}