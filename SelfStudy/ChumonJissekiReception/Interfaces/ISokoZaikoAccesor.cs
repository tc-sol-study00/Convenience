using Convenience.Models.DataModels;

namespace SelfStudy.ChumonJissekiReception.Interfaces {
    internal interface ISokoZaikoAccesor {
        /// <summary>
        /// 倉庫在庫（追加用・更新用）
        /// </summary>
        public SokoZaiko SokoZaiko { get; set; }
        /// <summary>
        /// 倉庫在庫取得
        /// </summary>
        /// <param name="inShiireSakiId">仕入先コード</param>
        /// <param name="inShiirePrdId">仕入商品コード</param>
        /// <param name="inShohinId">商品コード</param>
        /// <returns>倉庫在庫（データがなければnull）</returns>
        public SokoZaiko? GetSokoZaiko(string inShiireSakiId, string inShiirePrdId, string inShohinId);

        /// <summary>
        /// 倉庫在庫作成
        /// </summary>
        /// <param name="inShiireSakiId">仕入先コード</param>
        /// <param name="inShiirePrdId">仕入商品コード</param>
        /// <param name="inShohinId">商品コード</param>
        /// <returns>新規に作成された倉庫在庫</returns>
        public SokoZaiko CreateSokoZaiko(string inShiireSakiId, string inShiirePrdId, string inShohinId);

        }
    }
