using Convenience.Models.ViewModels.Shiire;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 仕入サービス用インターフェース
    /// </summary>
    public interface IShiireService : IDisposable {

        /// <summary>
        /// 仕入キーモデル設定（仕入画面１枚目用）
        /// </summary>
        /// <returns>ShiireKeysViewModel 仕入キービューモデル</returns>
        public Task<ShiireKeysViewModel> SetShiireKeysModel();

        /// <summary>
        /// <para>仕入セッティング</para>
        /// <para>仕入実績データの画面初期表示用（DB更新後の再表示も含む）</para>
        /// </summary>
        /// <param name="inShiireKeysViewModel">注文キービューモデル</param>
        /// <returns>ShiireViewModel 仕入ビューモデル（仕入実績・インクルードされた注文実績・インクルードされた倉庫在庫）</returns>
        /// <remarks>
        /// <para>①現在時間により仕入日セット</para>
        /// <para>②仕入は毎回新規なので、仕入SEQを発番・注文実績から仕入実績を作る</para>
        /// <para>③関係する倉庫在庫を接続（表示用）</para>
        /// </remarks>
        public Task<ShiireViewModel> ShiireSetting(ShiireKeysViewModel inShiireKeysViewModel);

        /// <summary>
        /// <para>仕入データをDBに書き込む・注文残の調整・倉庫在庫への反映（Post後処理・再表示用）</para>
        /// <para>仕入実績データPost後の後の再表示用</para>
        /// </summary>
        /// <param name="inShiireViewModel">仕入ビューモデル（注文コード・仕入日・仕入SEQ、Postされた仕入実績データ）</param>
        /// <returns>仕入ビューモデル（更新エンティティ数・DB更新された仕入実績）</returns>
        /// <remarks>
        /// <para>①仕入実績がある場合は、仕入実績取り込み、ない場合は注文実績から作成</para>
        /// <para>②　①の内容に対し、ポストデータを反映</para>
        /// <para>③注文実績の注文残と倉庫在庫の在庫数を仕入数にあわせ過不足する</para>
        /// <para>④仕入実績DB更新</para>
        /// <para>⑤仕入実績に倉庫在庫を接続しインクルードできるようにする（表示用）</para>
        /// </remarks>
        public Task<ShiireViewModel> ShiireCommit(ShiireViewModel inShiireViewModel);
    }
}
