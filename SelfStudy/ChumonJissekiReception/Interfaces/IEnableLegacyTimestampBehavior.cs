namespace SelfStudy.ChumonJissekiReception.Interfaces {

    /// <summary>
    /// .NetがUTC仕様になったから、with TimeZoneで時間を管理するpostgreと仕様が合わないので、
    /// それを処置するもの
    /// これやらないと、DateTime系でPostgreSQLを更新しようとするとABENDする
    /// </summary>
    /// <remarks>
    /// ①ChumonJissekiReceptionにこのインターフェースを実装
    /// ②コンストラクタで以下の内容を記述
    /// 　(this as IEnableLegacyTimestampBehavior).SetSwitch();
    /// </remarks>
    public interface IEnableLegacyTimestampBehavior {
        public void SetSwitch() {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
