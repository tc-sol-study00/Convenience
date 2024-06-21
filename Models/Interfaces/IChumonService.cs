using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.Chumon;
using Convenience.Models.Properties;
using static Convenience.Models.Properties.Message;
using Convenience.Data;

namespace Convenience.Models.Interfaces {

    public interface IChumonService {
        /// <summary>
        /// * 注文クラス用オブジェクト変数
        /// </summary>
        public IChumon chumon { get; set; }

        /// <summary>
        /// 注文セッティング
        /// </summary>
        /// <param name="inShiireSakiId">仕入先コード（画面より）</param>
        /// <param name="inChumonDate">注文日付（画面より）</param>
        /// <returns>注文viewモデル</returns>
        public Task<ChumonViewModel> ChumonSetting(string inShiireSakiId, DateOnly inChumonDate);

        /// <summary>
        /// 注文データをDBに書き込む
        /// </summary>
        /// <param name="inChumonJisseki">Postされた注文実績</param>
        /// <returns></returns>
        /// <exception cref="Exception">排他制御の例外が起きたらスローする</exception>
        public Task<(ChumonJisseki, int, bool, ErrDef)> ChumonCommit(ChumonJisseki inChumonJisseki);
    }

}