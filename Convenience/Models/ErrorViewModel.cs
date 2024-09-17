using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Convenience.Models
{
    public class ErrorViewModel  {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public int? StatusCode { get; set; }

        public DateTime? EventAt { get; set; }

        public IExceptionHandlerPathFeature? ExceptionHandlerPathFeature;

        public IStatusCodeReExecuteFeature? StatusCodeReExecuteFeature;

    }
}
