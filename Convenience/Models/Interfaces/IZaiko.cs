using Convenience.Models.DataModels;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 在庫クラスインターフェース
    /// </summary>
    public interface IZaiko {
        /// <summary>
        /// 生成される問い合わせ（遅延実行用）
        /// </summary>
        public IQueryable<ZaikoListLine>? SoKoZaikoQueryable { get; set; }
        /// <summary>
        /// 注文実績明細検索＆倉庫在庫（遅延実行）
        /// </summary>
        /// <param name="inSearchKey">検索キー</param>
        /// <returns>倉庫在庫　＆　注文実績明細(変数:SoKoZaikoQueryable) Where指示付き　ISoKoZaikoQueryable型にして遅延実行化</returns>
        public IQueryable<ZaikoListLine> CreateSokoZaikoList(string searchKey);
        /// <summary>
        ///  機能：注文実績明細検索＆倉庫在庫（遅延実行）＋Where内容の状態から、ソート順の追加セットを行う
        /// </summary>
        /// <param name="sortKey">ソートキー</param>
        /// <param name="descending">降順・昇順区分</param>
        /// <returns>倉庫在庫　＆　注文実績明細(変数:SoKoZaikoQueryable) ソート指示付き　ISoKoZaikoQueryable型にして遅延実行化</returns>
        public IQueryable<ZaikoListLine>? AddOrderby(string sortKey, bool descending);
        }
}
