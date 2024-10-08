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

        public Task<KaikeiViewModel> KaikeiAdd(KaikeiViewModel argKaikeiViewModel);
    }
}
