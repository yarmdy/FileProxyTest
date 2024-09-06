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
        /// <summary>
        /// 注入http客户端依赖
        /// </summary>
        [FromServices]
        public IHttpClientFactory HttpClientFactory { get; set; } = default!;
        /// <summary>
        /// 注入缓存策略配置
        /// </summary>
        [FromServices]
        public IOptionsMonitor<FileCacheOptions> FileCacheOptions { get; set; } = default!;
        /// <summary>
        /// 构建http客户端
        /// </summary>
        /// <returns></returns>
        protected HttpClient GetHttpClient() => HttpClientFactory.CreateClient();
        [HttpGet]
        public async Task<IActionResult> Index(string url)
        {
            // 获取缓存策略当前配置，修改了配置文件后马上生效不用重启服务
            var options = FileCacheOptions.CurrentValue;
            // 启发缓存时间换算，启发过期时间 = 响应头 (Date - LastModified)*10%  ,这里的DateTimeOffset.Now粗略等于Date，不需要太精确
            var time = DateTimeOffset.Now.AddSeconds(-options.ExpiredSeconds*10);
            // 构建etag，加上统一文件过期时间，统一一下缓存策略
            var etag = $"\"{await url.ToSha256StringAsync()}{await options.FileModifiedTime.ToSha256StringAsync()}\"";
            // 获取客户端上次的ETag
            var clientTag = Request.Headers.IfNoneMatch;
            // 判断ETag是否一致
            if(etag == clientTag)
            {
                // 一致说明缓存不过期，返回304状态，让浏览器获取本地缓存，不再返回服务器的文件
                return StatusCode(304);
            }
            
            // 构建http客户端
            var client = GetHttpClient();
            // 获取远程文件
            var res = await client.GetAsync(url);
            // 获取文件流
            var stream = await res.Content.ReadAsStreamAsync();
            // 启发缓存因为每次不一样，需要用强缓存来计算过期时间
            Response.Headers.CacheControl = $"max-age={options.ExpiredSeconds}";
            // 返回远程文件，读取远程头的文件类型，加入启发时间，加入ETag
            return File(stream, res.Content.Headers.ContentType?.ToString() ?? "image/jpg", time, EntityTagHeaderValue.Parse(etag));
        }
    }
}
