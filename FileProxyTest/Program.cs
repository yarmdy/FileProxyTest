var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.Configure<FileCacheOptions>(builder.Configuration.GetSection("FileCacheOptions"));

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapDefaultControllerRoute();

app.Run();


public class FileCacheOptions
{
    /// <summary>
    /// ͳһ�ļ�����޸�ʱ��
    /// </summary>
    public DateTime FileModifiedTime { get; set; }
    /// <summary>
    /// �����������ʱ��
    /// </summary>
    public int ExpiredSeconds { get; set; }
}