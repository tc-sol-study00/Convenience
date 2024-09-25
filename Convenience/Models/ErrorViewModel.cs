using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Convenience.Models
{
    /// <summary>
    /// エラーページ用ビューモデル
    /// </summary>
    public class ErrorViewModel  {
        /// <summary>
        /// リクエストID
        /// </summary>
        public string? RequestId { get; set; }
        /// <summary>
        /// 上記リクエストIDを出すか出さないか
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        /// <summary>
        /// ステータスコード
        /// </summary>
        public int? StatusCode { get; set; }
        /// <summary>
        /// エラーイベント発生日時
        /// </summary>
        public DateTime? EventAt { get; set; }
        /// <summary>
        /// <para>エラー発生時の詳細情報</para>
        /// <para>例外が発生したパスや例外自体などの情報</para>
        /// </summary>
        public IExceptionHandlerPathFeature? ExceptionHandlerPathFeature;

        /// <summary>
        /// <para>エラーハンドリングに関連する機能を提供するインターフェース</para>
        /// <para>特に、404 Not Found や 500 Internal Server Error などのステータスコードが発生した場合に、</para>
        /// <para>再実行されるリクエストの情報を取得するために使用</para>
        /// </summary>
        public IStatusCodeReExecuteFeature? StatusCodeReExecuteFeature;

    }
}
