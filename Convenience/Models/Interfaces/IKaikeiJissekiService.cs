using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.KaikeiJisseki;
using Convenience.Models.ViewModels.TentoZaiko;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    public interface IKaikeiJissekiService {
        /// <summary>
        /// 店頭在庫検索
        /// </summary>
        /// <param name="argKaikeiJissekiViewModel"会計実績検索ビューモデル</param>
        /// <returns>会計実績検索ビューモデル（検索内容含む）</returns>
        public Task<KaikeiJissekiViewModel> KaikeiJissekiRetrival(KaikeiJissekiViewModel argKaikeiJissekiViewModel);
        }
}