using Convenience.Models.DataModels;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 店頭払出プロパティ用インターフェース
    /// </summary>
    public interface ITentoHaraidashi {
        /// <summary>
        /// <para>プロパティ</para>
        /// <para>店頭払出ヘッダー</para>
        /// </summary>
        public TentoHaraidashiHeader? TentoHaraidashiHeader { get; set; }

        /// <summary>
        /// <para>倉庫在庫から店頭払出実績（ヘッダー＋実績）を作成する</para>
        /// <para>①店頭払出ヘッダーを作成する</para>
        /// <para>②倉庫在庫より、店頭払出実績（ヘッダー＋実績）を作成する</para>
        /// <para>③データ表示用に＋倉庫在庫＋仕入マスタもリンク接続する</para>
        /// <para>4️⃣店頭在庫をリンク接続する</para>
        /// </summary>
        /// <param name="argCurrentDateTime"></param>
        /// <returns>TentoHaraidashiHeader 店頭払出ヘッダー＋店頭払出実績</returns>
        public Task<TentoHaraidashiHeader> TentoHaraidashiSakusei(DateTime argCurrentDateTime);

        /// <summary>
        /// <para>店頭払出問い合わせ</para>
        /// <para>①店頭払出ヘッダー＋実績を問い合わせる</para>
        /// <para>②実績に倉庫在庫をくっつける</para>
        /// <para>②実績に仕入マスタ＋商品マスタをくっつける</para>
        /// <para>③実績に店頭在庫をくっつける</para>
        /// </summary>
        /// <param name="argTentoHaraidashiId">店頭払出コード</param>
        /// <returns>TentoHaraidashiHeader 店頭払出ヘッダ</returns>
        public Task<TentoHaraidashiHeader?> TentoHaraidashiToiawase(string argTentoHaraidashiId);

        /// <summary>
        /// Postデータを上書きしてＤＢ更新準備をする
        /// </summary>
        /// <param name="argTentoHaraidashiJissekis">Postされた店頭払出実績</param>
        /// <returns>IList<TentoHaraidashiJisseki> Postデータが上書きされた店頭払出実績</returns>
        /// <exception cref="Exception"></exception>
        public IList<TentoHaraidashiJisseki> TentoHaraidashiUpdate(IEnumerable<TentoHaraidashiJisseki> argTentoHaraidashiJissekis);

        /// <summary>
        /// 店頭払出ヘッダーのリストを条件より作成
        /// </summary>
        /// <param name="whereExpression">条件式</param>
        /// <returns>IQueryable<TentoHaraidashiHeader> 店頭払出ヘッダーリスト（遅延実行）</returns>
        public IQueryable<TentoHaraidashiHeader> TentoHaraidashiHeaderList(Expression<Func<TentoHaraidashiHeader, bool>> whereExpression);
    }
}