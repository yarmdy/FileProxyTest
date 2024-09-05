using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Web;

namespace FileProxyTest.Controllers
{
    [ApiController]
    [Route("/api/{controller}")]
    public class ProxyController : ControllerBase
    {
        [FromServices]
        public IHttpClientFactory HttpClientFactory { get; set; } = default!;
        [FromServices]
        public IOptionsMonitor<FileCacheOptions> FileCacheOptions { get; set; } = default!;
        protected HttpClient GetHttpClient() => HttpClientFactory.CreateClient();
        [HttpGet]
        public async Task<IActionResult> Index(string url)
        {
            var options = FileCacheOptions.CurrentValue;
            var time = DateTimeOffset.Now.AddSeconds(-options.ExpiredSeconds*10);
            
            var etag = $"\"{string.Join(string.Empty, BitConverter.GetBytes(url.GetHashCode()).Select(a => a.ToString("x2")))}{(uint)options.FileModifiedTime.GetHashCode()}\"";
            var clientTime = Request.Headers.IfModifiedSince;
            var clientTag = Request.Headers.IfNoneMatch;

            if(etag == clientTag)
            {
                return StatusCode(304);
            }
            
            
            var client = GetHttpClient();
            var res = await client.GetAsync(url);
            var stream = await res.Content.ReadAsStreamAsync();
            return File(stream, res.Content.Headers.ContentType?.ToString() ?? "image/jpg", time, EntityTagHeaderValue.Parse(etag));
        }
    }
}
