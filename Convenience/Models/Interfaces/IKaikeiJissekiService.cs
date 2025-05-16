using Convenience.Models.ViewModels.KaikeiJisseki;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 会計実績検索サービス用インターフェース
    /// </summary>
    public interface IKaikeiJissekiService {
        /// <summary>
        /// 会計実績検索
        /// </summary>
        /// <param name="argKaikeiJissekiViewModel"会計実績検索ビューモデル</param>
        /// <returns>会計実績検索ビューモデル（検索内容含む）</returns>
        public Task<KaikeiJissekiViewModel> KaikeiJissekiRetrival(KaikeiJissekiViewModel argKaikeiJissekiViewModel);
        }
}