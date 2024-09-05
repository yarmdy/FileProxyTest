using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace FileProxyTest.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // 生成图片url
            var url = HttpUtility.UrlEncode("https://i01piccdn.sogoucdn.com/e6ca035adb754816");
            return Content($"""<img src="/api/proxy?url={url}" />""","text/html");
        }
    }
}
