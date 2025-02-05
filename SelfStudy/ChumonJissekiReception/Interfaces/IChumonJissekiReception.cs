namespace SelfStudy.ChumonJissekiReception.Interfaces {
    public interface IChumonJissekiReception : IDisposable {

        /// <summary>
        /// 注文実績を一括仕入れを行うメインメソッド
        /// </summary>
        /// <remarks>
        /// ①一覧教示
        /// ②番号を選ばせる
        /// ③注文実績と明細を読む＋注文残求める
        /// ④仕入実績を登録する
        /// ⑤倉庫在庫を登録・更新する
        /// ⑥注文残を０にする
        /// </remarks>
        /// <returns>正常であれば0、なにか異常であれば-1</returns>
        public int ChumonJissekiToShiireJisseki();
    }
}
