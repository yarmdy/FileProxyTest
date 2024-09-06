using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// ע��http�ͻ�������
builder.Services.AddHttpClient();

// �����ļ���ȡ�������
builder.Services.Configure<FileCacheOptions>(builder.Configuration.GetSection("FileCacheOptions"));

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapDefaultControllerRoute();

app.Run();

/// <summary>
/// �ļ��������������
/// </summary>
public class FileCacheOptions
{
    /// <summary>
    /// ͳһ�ļ�����޸�ʱ��
    /// </summary>
    public DateTime FileModifiedTime { get; set; }
    /// <summary>
    /// �������ʱ��
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