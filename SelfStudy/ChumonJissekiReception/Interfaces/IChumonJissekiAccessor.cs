using Convenience.Models.DataModels;
using SelfStudy.ChumonJissekiReception.DTO;

namespace SelfStudy.ChumonJissekiReception.Interfaces {
    /// <summary>
    /// ChumonJissekiAccessor用インターフェース
    /// </summary>
    public interface IChumonJissekiAccessor {
        /// <summary>
        /// 注文残リスト用
        /// </summary>
        public IEnumerable<ChumonListItem> ChumonZanList { get; set; }

        /// <summary>
        /// 注文実績
        /// </summary>
        public ChumonJisseki? ChumonJisseki { get; set; }

        /// <summary>
        /// 注文実績と明細を取得し、プロパティ（注文実績）にセットする）
        /// </summary>
        /// <param name="inChumonJissekis">注文実績一覧</param>
        /// <param name="inShiireSakiId">仕入先コード</param>
        /// <param name="inChumonId">注文コード</param>
        /// <returns>注文実績</returns>
        public ChumonJisseki? GetaChumonJisseki(string inShiireSakiId, string inChumonId);
        /// <summary>
        /// 注文リスト作成
        /// </summary>
        /// <returns>注文リスト</returns>
        public IEnumerable<ChumonListItem> GetChumonZanList();
    }
}
