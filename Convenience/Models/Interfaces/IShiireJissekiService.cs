using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.ShiireJisseki;
using Convenience.Models.ViewModels.TentoZaiko;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 仕入実績検索サービス用インターフェース
    /// </summary>
    public interface IShiireJissekiService {
        /// <summary>
        /// 仕入実績検索
        /// </summary>
        /// <param name="argShiireJissekiViewModel"仕入実績検索ビューモデル</param>
        /// <returns>会計実績検索ビューモデル（検索内容含む）</returns>
        public Task<ShiireJissekiViewModel> ShiireJissekiRetrival(ShiireJissekiViewModel argShiireJissekiViewModel);
        }
}