using Convenience.Models.ViewModels.ChumonJisseki;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 注文実績検索サービス用インターフェース
    /// </summary>
    public interface IChumonJissekiService {
        /// <summary>
        /// 店頭在庫検索
        /// </summary>
        /// <param name="argChumonJissekiViewModel"注文実績検索ビューモデル</param>
        /// <returns>注文実績検索ビューモデル（検索内容含む）</returns>
        public Task<ChumonJissekiViewModel> ChumonJissekiRetrival(ChumonJissekiViewModel argChumonJissekiViewModel);
        }
}