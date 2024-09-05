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
    /// �����������ʱ��
    /// </summary>
    public int ExpiredSeconds { get; set; }
}