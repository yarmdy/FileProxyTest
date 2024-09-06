using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 注入http客户端依赖
builder.Services.AddHttpClient();

// 配置文件读取缓存策略
builder.Services.Configure<FileCacheOptions>(builder.Configuration.GetSection("FileCacheOptions"));

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapDefaultControllerRoute();

app.Run();

/// <summary>
/// 文件缓存策略配置类
/// </summary>
public class FileCacheOptions
{
    /// <summary>
    /// 统一文件最后修改时间
    /// </summary>
    public DateTime FileModifiedTime { get; set; }
    /// <summary>
    /// 缓存过期时间
    /// </summary>
    public int ExpiredSeconds { get; set; }
}

public static class Md5StringExtensions
{
    public static async Task<string> ToSha256StringAsync(this object obj)
    {
        var data = Encoding.UTF8.GetBytes(obj?.ToString()??string.Empty);
        var ms = new MemoryStream(data);
        return string.Join(string.Empty, (await MD5.HashDataAsync(ms)).Select(x => x.ToString("x2")));
    }

    public static string ToSha256String(this object obj)
    {
        return ToSha256StringAsync(obj).Result;
    }
}