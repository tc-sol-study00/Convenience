using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.Kaikei;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    public interface IKaikei {
        /// <summary>
        /// 会計ヘッダープロパティ
        /// </summary>
        public KaikeiHeader KaikeiHeader { get; set; }

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
        public Task<KaikeiHeader> KaikeiSakusei(DateTime argCurrentDateTime);
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
        public Task<KaikeiHeader?> KaikeiUpdate(KaikeiHeader argpostedKaikeiHeader);

    }
}