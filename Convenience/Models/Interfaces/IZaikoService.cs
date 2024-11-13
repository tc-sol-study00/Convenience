using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.Zaiko;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 倉庫在庫検索（サービス）インターフェース 
    /// </summary>
    public interface IZaikoService {
        /// <summary>
        /// <para>検索キー画面の情報取得</para>
        /// </summary>
        /// <param name="inKeySetOrderArray"></param>
        /// <param name="inSelectWhereItemArray"></param>
        /// <returns></returns>
        public Task<ZaikoViewModel> KeyInput(KeyEventRec[] keydata, SelecteWhereItem[] selecteWhereItemArray);
    }
}
