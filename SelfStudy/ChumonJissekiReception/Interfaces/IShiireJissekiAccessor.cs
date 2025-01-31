using Convenience.Models.DataModels;

namespace SelfStudy.ChumonJissekiReception.Interfaces {
    public interface IShiireJissekiAccessor {
        /// <summary>
        /// 仕入実績（仕入実績DB登録用）
        /// </summary>
        public ShiireJisseki? ShiireJisseki { get; set; }
        /// <summary>
        /// 仕入実績が当日でぶつからないようにする
        /// </summary>
        /// <param name="inChumonId">注文コード</param>
        /// <param name="inShiireDate">仕入日</param>
        /// <returns>注文コード、仕入日で求められる仕入SEQの最大値</returns>
        public uint GetMaxSeqByShiireDate(string inChumonId, DateOnly inShiireDate);
        /// <summary>
        /// 仕入実績作成
        /// </summary>
        /// <param name="inChumonJissekiMeisai">仕入実績登録のためのベースである注文実績明細</param>
        /// <param name="inShiireSakiId">仕入実績に登録する仕入先コード</param>
        /// <param name="inSeqByShiireDate">仕入実績作成する際のSeqNo</param>
        /// <remarks>
        /// 仕入実績をDBコンテクストで追加する処理も含む
        /// </remarks>
        /// <returns>DB更新後の仕入実績</returns>
        public ShiireJisseki CreateShiireJissekiToDB(ChumonJissekiMeisai inChumonJissekiMeisai, string inShiireSakiId, uint inSeqByShiireDate);
        }
    }
