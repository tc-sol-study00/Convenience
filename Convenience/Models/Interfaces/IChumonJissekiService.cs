using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.ChumonJisseki;
using Convenience.Models.ViewModels.TentoZaiko;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.ChumonJisseki.ChumonJissekiViewModel.DataAreaClass;

namespace Convenience.Models.Interfaces {
    public interface IChumonJissekiService {
        /// <summary>
        /// 店頭在庫検索
        /// </summary>
        /// <param name="argChumonJissekiViewModel"会計実績検索ビューモデル</param>
        /// <returns>会計実績検索ビューモデル（検索内容含む）</returns>
        public Task<ChumonJissekiViewModel> ChumonJissekiRetrival(ChumonJissekiViewModel argChumonJissekiViewModel);
        }
}