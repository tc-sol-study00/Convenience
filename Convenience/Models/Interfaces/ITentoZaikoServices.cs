using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.TentoZaiko;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    public interface ITentoZaikoService {
        /// <summary>
        /// 店頭在庫検索
        /// </summary>
        /// <param name="argTentoZaikoViewModel">店頭在庫検索ビューモデル</param>
        /// <returns>店頭在庫ビューモデル（検索内容含む）</returns>
        public TentoZaikoViewModel TentoZaikoRetrival(TentoZaikoViewModel argTentoZaikoViewModel);
        }
}