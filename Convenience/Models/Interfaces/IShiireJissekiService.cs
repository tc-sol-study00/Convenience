using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.ShiireJisseki;
using Convenience.Models.ViewModels.TentoZaiko;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    public interface IShiireJissekiService {
        /// <summary>
        /// 店頭在庫検索
        /// </summary>
        /// <param name="argShiireJissekiViewModel"仕入実績検索ビューモデル</param>
        /// <returns>会計実績検索ビューモデル（検索内容含む）</returns>
        public Task<ShiireJissekiViewModel> ShiireJissekiRetrival(ShiireJissekiViewModel argShiireJissekiViewModel);
        }
}