using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.Kaikei;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    public interface IKaikei {
        /// <summary>
        /// 会計ヘッダープロパティ
        /// </summary>
        public KaikeiHeader? KaikeiHeader { get; set; }

        /// <summary>
        /// <para>会計作成（新規会計用）</para>
        /// <para>新規会計ヘッダーと実績を作成する</para>
        /// </summary>
        /// <param name="argCurrentDateTime">適用日時</param>
        /// <returns>KaikeiHeader 会計ヘッダー＋実績</returns>
        /// <remarks>
        /// <para>①会計ヘッダーの作成</para>
        /// <para>②会計実績の作成</para>
        /// </remarks>
        public KaikeiHeader KaikeiSakusei(DateTime argCurrentDateTime);
        /// <summary>
        /// 会計実績の品目追加
        /// </summary>
        /// <param name="argKaikeiDateTime"></param>
        /// <param name="argKaikeiJisseki"></param>
        /// <returns>KaikeiJisseki 品目を追加された会計実績</returns>
        public Task<IList<KaikeiJisseki>> KaikeiAddcommodity(IKaikeiJissekiForAdd argKaikeiJisseki);


        /// <summary>
        /// 会計問い合わせ(ソート指示なし）
        /// </summary>
        /// <param name="argTentoHaraidashiId">店頭払出コード</param>
        /// <returns>会計実績ヘッダー＋実績</returns>
        public Task<KaikeiHeader?> KaikeiToiawase(string argTentoHaraidashiId);

        /// <summary>
        /// 会計問い合わせ
        /// </summary>
        /// <typeparam name="T">ソート指示された項目のタイプ</typeparam>
        /// <param name="argTentoHaraidashiId">店頭払出コード</param>
        /// <param name="orderexpression">ソート指示用ラムダ式</param>
        /// <returns>会計実績ヘッダー＋実績</returns>
        public Task<KaikeiHeader?> KaikeiToiawase<T>(string argTentoHaraidashiId, Expression<Func<KaikeiJisseki, T>>? orderexpression);

        /// <summary>
        /// 会計ヘッダー＋実績のＤＢ更新
        /// </summary>
        /// <param name="argpostedKaikeiHeader">反映する会計ヘッダ＋実績</param>
        /// <returns>KaikeiHeader DB更新された会計ヘッダ＋実績</returns>
        public Task<KaikeiHeader> KaikeiUpdate(KaikeiHeader argpostedKaikeiHeader);

        /// <summary>
        /// 店頭在庫から売上分を差し引く
        /// </summary>
        /// <param name="argShohinId">商品コード</param>
        /// <param name="argUriageDateTime">売上日時</param>
        /// <param name="argDiffUriageSu">差し引く個数</param>
        /// <param name="argTentoZaiko">店頭在庫</param>
        /// <returns>店頭在庫</returns>
        public TentoZaiko? ZaikoConnection(string argShohinId, DateTime argUriageDateTime, decimal argDiffUriageSu, TentoZaiko? argTentoZaiko);

        /// <summary>
        /// 消費税計算
        /// </summary>
        /// <param name="inKaikeiJisseki">会計実績</param>
        /// <param name="outKaikeiJisseki">消費税関連で計算されたものの反映先（会計実績）</param>
        /// <returns>消費税関連で計算されたものの反映先（会計実績）</returns>
        /// <exception cref="InvalidDataException"></exception>
        /// <remarks>（条件）売上金額が第一引数側にセットされていること</remarks>
        public KaikeiJisseki ShohizeiKeisan(IKaikeiJissekiForAdd inKaikeiJisseki, KaikeiJisseki outKaikeiJisseki);

        }
}