using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VoteMap.Core.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();


        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // 確保 HTTP Request 可以多次讀取
            context.Request.EnableBuffering();

            context.Request.Headers.Add("requestid", Guid.NewGuid().ToString());

            // 讀取 HTTP Request Body 內容
            // 注意！要設定 leaveOpen 屬性為 true 使 StreamReader 關閉時，HTTP Request 的 Stream 不會跟著關閉
            using (var bodyReader = new StreamReader(stream: context.Request.Body,
                                                    encoding: Encoding.UTF8,
                                                    detectEncodingFromByteOrderMarks: false,
                                                    bufferSize: 1024,
                                                    leaveOpen: true))
            {
                var body = await bodyReader.ReadToEndAsync();

                LogModel m = new LogModel()
                {
                    method = context.Request.Method,
                    URL = context.Request.Path,
                    query = context.Request?.QueryString.Value,
                    body = body
                };

                _logger.Info(JsonSerializer.Serialize(m, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
            }

            // 將 HTTP Request 的 Stream 起始位置歸零
            context.Request.Body.Position = 0;

            await _next.Invoke(context);
        }

    }

    public class LogModel
    {
        public string method { get; set; } = "";
        public string URL { get; set; } = "";
        public string query { get; set; } = "";
        public string body { get; set; } = "";
    }
}