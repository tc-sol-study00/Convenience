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
        public OrderCodeGenerationException(string message) : base($"{message}の注文コード発番エラーです") { }
    }

    /// <summary>
    /// データ上乗せ時のindex位置エラー
    /// </summary>
    public class DataPositionMismatchException : Exception {
        public DataPositionMismatchException(string message) : base($"「{message}」のPostデータエラーとDB側データの位置エラーです(ソートされていない可能性）") { }
    }

    /// <summary>
    /// ０件データ
    /// </summary>
    public class DataCountMismatchException : Exception {
        public DataCountMismatchException(string message) : base($"「{message}」のPostデータエラーとDB側データの件数アンマッチです") { }
    }

    /// <summary>
    /// ポストデータチェックエラー
    /// </summary>
    public class PostDataInValidException : Exception {
        public PostDataInValidException(string message) : base($"「{message}」のPostデータにエラーがあります") { }
    }
    /// <summary>
    /// DB更新排他制御エラー
    /// </summary>
    public class DbUpdateTimeOutException : Exception {

        public DbUpdateTimeOutException(string message) : base($"「{message}」のDB更新排他制御がタイプアウトしました") { }
    }
}
