using Convenience.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.Diagnostics;

namespace Convenience.Controllers {
    /// <summary>
    /// Homeコントローラ（メニュー表示用）
    /// </summary>
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View(new Menu());
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [IgnoreAntiforgeryToken]
        public IActionResult Error(int id) {
            DateTime dateTime = DateTime.Now;

            IExceptionHandlerPathFeature? exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            IStatusCodeReExecuteFeature? statusCodeReExecuteFeature =
                HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // HttpContextから必要な情報を取得
            var context = HttpContext;
            var request = context.Request;
            var user = context.User;

            // HTTP系エラー対応
            if (statusCodeReExecuteFeature != null) {
                var logEvent = new LogEventInfo(NLog.LogLevel.Warn, "", "HTTP Error occurred!") {
                    Properties = {
                        ["Path"] = statusCodeReExecuteFeature.OriginalPath,
                        ["QueryString"] = statusCodeReExecuteFeature.OriginalQueryString,
                        ["StatusCode"] = id,
                        ["RemoteIp"] = context.Connection.RemoteIpAddress?.ToString(),
                        ["User"] = user.Identity?.Name ?? "Anonymous",
                        ["IsAuthenticated"] = user.Identity?.IsAuthenticated ?? false
                    }
                };

                // ILoggerで構造化ログを出力
                _logger.LogWarning("HTTP Error occurred! {@LogEventProperties}", logEvent.Properties);
            }

            // エラーハンドリング（例外の場合）
            if (exceptionHandlerPathFeature != null) {
                var exception = exceptionHandlerPathFeature.Error;

                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "", "Application Error occurred!") {
                    Exception = exception,
                    Properties = {
                        ["Path"] = request.Path,
                        ["Method"] = request.Method,
                        ["QueryString"] = request.QueryString.ToString(),
                        ["RemoteIp"] = context.Connection.RemoteIpAddress?.ToString(),
                        ["User"] = user.Identity?.Name ?? "Anonymous",
                        ["IsAuthenticated"] = user.Identity?.IsAuthenticated ?? false,
                        ["StatusCode"] = context.Response.StatusCode
                    }
                };

                // ILoggerで構造化ログを出力
                _logger.LogError(exception, "Application Error occurred! {@LogEventProperties}", logEvent.Properties);
            }

            // エラー情報をViewに渡す
            ErrorViewModel errorViewModel = new() {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = id == 0 ? null : id,
                EventAt = dateTime,
                ExceptionHandlerPathFeature = exceptionHandlerPathFeature,
                StatusCodeReExecuteFeature = statusCodeReExecuteFeature
            };

            return View(errorViewModel);
        }
    }
}
