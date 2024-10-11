using Convenience.Models.ViewModels.Kaikei;

namespace Convenience.Models.Interfaces {
    public interface IKaikeiService {

        /// <summary>
        /// 会計ビューモデル設定
        /// </summary>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public Task<KaikeiViewModel> SetKaikeiViewModel();

        /// <summary>
        /// 会計セッティング
        /// </summary>
        /// <param name="argKaikeiViewModel">会計ビューモデル</param>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public Task<KaikeiViewModel> KaikeiSetting(KaikeiViewModel argKaikeiViewModel);

        /// <summary>
        /// 会計Commit
        /// </summary>
        /// <param name="argKaikeiViewModel">会計ビューモデル</param>
        /// <returns>KaikeiViewModel 会計ビューモデル</returns>
        public Task<KaikeiViewModel> KaikeiCommit(KaikeiViewModel argKaikeiViewModel);

        /// <summary>
        /// 会計データの画面上の追加
        /// </summary>
        /// <param name="argKaikeiViewModel">会計ビューモデル</param>
        /// <returns>KaikeiViewModel 会計品目が追加追加された会計ビュー</returns>
        public Task<KaikeiViewModel> KaikeiAdd(KaikeiViewModel argKaikeiViewModel);
        
        /// <summary>
        /// 商品コードを引数に、商品名称を回答する
        /// </summary>
        /// <param name="argShohinId">商品コード</param>
        /// <returns>商品名称</returns>
        public Task<string> GetShohinName(string argShohinId);
    }
}
