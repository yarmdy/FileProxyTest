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
    /// 启发缓存过期时间
    /// </summary>
    public int ExpiredSeconds { get; set; }
}