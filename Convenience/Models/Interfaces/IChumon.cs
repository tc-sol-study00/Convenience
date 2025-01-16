using Convenience.Data;
using Convenience.Models.DataModels;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 注文クラスインターフェース
    /// </summary>
    public interface IChumon {
        /// <summary>
        /// 注文実績プロパティ
        /// </summary>
        public ChumonJisseki? ChumonJisseki { get; set; }

        /// <summary>
        /// 注文作成
        /// </summary>
        /// <remarks>
        /// 仕入先より注文実績データ（親）を生成する
        /// 注文実績明細データ（子）を仕入マスタを元に作成する
        /// 注文実績データ（親）と注文実績明細データ（子）を連結する
        /// 注文実績（プラス注文実績明細）を戻り値とする
        /// </remarks>
        /// <param name="inShireSakiId">仕入先コード</param>
        /// <param name="inChumonDate">注文日</param>
        /// <returns>新規作成された注文実績</returns>
        /// <exception cref="Exception"></exception>
        public Task<ChumonJisseki> ChumonSakusei(string inShireSakiId, DateOnly inChumonDate);

        /// <summary>
        /// 注文更新用問い合わせ
        /// </summary>
        /// <remarks>
        /// ①注文実績＋注文実績＋仕入マスタ＋商品マスタ検索
        /// ②戻り値を注文実績＋注文実績明細とする
        /// </remarks>
        /// <param name="inShireSakiId">仕入先コード</param>
        /// <param name="inChumonDate">注文日</param>
        /// <returns>既存の注文実績</returns>
        public Task<ChumonJisseki?> ChumonToiawase(string inShireSakiId, DateOnly inChumonDate, bool includeShiireAndShohinMaster = true);

        /// <summary>
        /// 注文実績＋注文明細更新
        /// </summary>
        /// <param name="postedChumonJisseki">postされた注文実績</param>
        /// <returns>postされた注文実績を上書きされた注文実績</returns>
        public Task<ChumonJisseki> ChumonUpdate(ChumonJisseki postedChumonJisseki);


        /// <summary>
        /// DB更新
        /// </summary>
        /// <returns></returns>
        /// <remarks>注文実績＋注文実績明細を更新する</remarks>
        /// <exception cref="DbUpdateConcurrencyException"></exception>
        public Task<int> ChumonSaveChanges();

        /// <summary>
        /// 注文可能な仕入先一覧を作成する
        /// </summary>
        /// <returns>IEnumerable<ShiireSakiMaster>注文可能な仕入先マスタのリスト（実行前）</returns>
        public IQueryable<ShiireSakiMaster> ShiireSakiList<T>(Expression<Func<ShiireSakiMaster, T>> orderExpression);
    }
}