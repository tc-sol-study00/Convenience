namespace Convenience.Models.Properties {
    /*
     * 例外登録
     */
    /// <summary>
    /// エンティティからのデータがなかったとき
    /// </summary>
    public class NoDataFoundException : Exception {
        public NoDataFoundException(string message) : base($"{message}のデータがありません") { }
    }

    /// <summary>
    /// 注文コード発番時のエラー
    /// </summary>
    public class OrderCodeGenerationException : Exception {
        public OrderCodeGenerationException(string message) : base(message) { }
    }

    /// <summary>
    /// データ上乗せ時のindex位置エラー
    /// </summary>
    public class DataPositionMismatchException : Exception {
        public DataPositionMismatchException(string message) : base(message) { }
    }

    /// <summary>
    /// ０件データ
    /// </summary>
    public class DataCountMismatchException : Exception {
        public DataCountMismatchException(string message) : base(message) { }
    }

    /// <summary>
    /// 注文クラスのセッティングエラー
    /// </summary>
    public class ChumonJissekiSetupException : Exception {
        public ChumonJissekiSetupException(string message) : base(message) { }
    }

    /// <summary>
    /// ポストデータチェックエラー
    /// </summary>
    public class PostDataInValidException : Exception {
        public PostDataInValidException(string message) : base(message) { }
    }
}
