using Convenience.Models.DataModels;
using static Convenience.Models.Properties.Shiire;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 仕入クラスインターフェース
    /// </summary>
    public interface IShiire {
        /*
         * プロパティ
         */
        /// <summary>
        /// 仕入実績プロパティ
        /// Include ChuumonJissekiMeisai
        /// </summary>
        public IList<ShiireJisseki> Shiirejissekis { get; set; } //Include ChuumonJissekiMeisai
        /// <summary>
        /// 倉庫在庫プロパティ
        /// </summary>
        public IList<SokoZaiko> SokoZaikos { get; set; }

        /*
         *  Method群 
         */

        /// <summary>
        /// 仕入実績に既に存在しているかチェック
        /// </summary>
        /// <param name="inChumonId">注文コード（仕入実績に対する検索キー）</param>
        /// <param name="inShiireDate">仕入日付（仕入実績に対する検索キー）</param>
        /// <param name="inSeqByShiireDate">仕入SEQ（仕入実績に対する検索キー）</param>
        /// <returns>データがあればtrueなければfalse/returns>
        public Task<bool> ChuumonIdOnShiireJissekiExistingCheck(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate);

        /*
         *  初期表示のために利用する想定
         */

        /// <summary>
        /// 注文コード、仕入日を元に、次の仕入SEQを求める
        /// 仕入実績の主キーは注文コード、仕入日、仕入SEQなので、仕入日に数回仕入れる場合は、
        /// 仕入SEQをインクリメントして利用する
        /// </summary>
        /// <param name="inChumonId">仕入実績検索キー：注文コード</param>
        /// <param name="inShiireDate">仕入実績検索キー：仕入日</param>
        /// <returns>次の仕入SEQ（次に仕入実績を登録する仕入SEQ）</returns>
        public Task<uint> NextSeq(string inChumonId, DateOnly inShiireDate);

        /// <summary>
        /// 仕入実績作成
        /// </summary>
        /// <param name="inChumonId">注文コード（注文実績問い合わせキー）</param>
        /// <param name="inShiireDate">仕入日付（仕入実績にセットされる）</param>
        /// <param name="inSeqByShiireDate">仕入日付内のシーケンス（仕入実績にセットされる）</param>
        /// <returns>注文実績から新規作成された仕入実績</returns>
        public Task<IList<ShiireJisseki>> ChumonToShiireJisseki(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate);

        /// <summary>
        /// 注文実績から仕入実績プロパティに反映
        /// </summary>
        /// <param name="inChumonId">注文コード</param>
        /// <param name="inShiireDate">仕入日付</param>
        /// <param name="inSeqByShiireDate">仕入SEQ</param>
        /// <returns></returns>
        public Task<IList<ShiireJisseki>> ShiireToShiireJisseki(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate);

        /// <summary>
        /// 倉庫在庫を仕入データに接続する（表示前に利用する）　
        /// NotMappedは外部キーが使えないから、includeできないため
        /// </summary>
        /// <param name="inShiireJissekis">仕入実績</param>
        /// <param name="indata">仕入実績に結合する倉庫在庫</param>
        /// <return>倉庫在庫が接続された仕入実績</return>
        public IList<ShiireJisseki> ShiireSokoConnection(IList<ShiireJisseki> inShiireJissekis, IEnumerable<SokoZaiko> indata);

        /// <summary>
        /// 注文残・在庫数量調整
        /// </summary>
        /// <param name="inChumonId">注文コード</param>
        /// <param name="inShiireJissekis">仕入実績（注文実績がインクルードされていること）</param>
        /// <returns>注文残・倉庫在庫が調整された注文残・倉庫在庫調整用モデル</returns>
        public Task<ShiireUkeireReturnSet> ChuumonZanZaikoSuChousei(string inChumonId, IList<ShiireJisseki> inShiireJissekis);

        /*
         *  Post後で利用想定
         */

        /// <summary>
        /// 仕入データPost内容の反映 
        /// </summary>
        /// <param name="inShiireJissekis">Postされた仕入実績</param>
        /// <returns>Postされた注仕入実績がオーバライドされた仕入実績プロパティ</returns>
        public IList<ShiireJisseki> ShiireUpdate(IList<ShiireJisseki> inShiireJissekis);

        /*
         *  注文残があるものリスト 
         */

        /// <summary>
        /// 注文残がある注文のリスト化
        /// </summary>
        /// <returns>注文残のある注文コード一覧</returns>
        public Task<IList<ChumonList>> ZanAriChumonList();

    }
}

