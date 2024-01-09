using NSwag;
using Common.DistributedId;
using Ordering.WebApi.Filters;
using MagicOnion;
using Ordering.Infrastructure.Extensions;
using Ordering.WebApi.OutBoxMessageServices;
using Common.MessageMiddleware.Extensions;
using Ordering.Infrastructure;
using Common.Redis.Extensions.Configuration;
using Common.Redis.Extensions.Serializer;
using Common.Redis.Extensions;
using Ordering.IGrain;
using Orleans.Configuration;
using Ordering.GrainService;
using Orleans.Serialization;
using YamlDotNet.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.AllowAnyOrigin() // ����������Դ
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "��̨����ϵͳ";
    settings.AllowReferencesWithProperties = true;
});
#region ѩ��id �ֲ�ʽ
builder.Services.AddDistributedId(new DistributedIdOptions
{
    Distributed = true
});
#endregion
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddMq(builder.Configuration);
//redis
builder.Services.AddSingleton<IRedisCache>(obj =>
{
    var config = builder.Configuration.GetSection("Redis").Get<RedisConfiguration>();
    var serializer = new MsgPackSerializer();
    var connection = new PooledConnectionMultiplexer(config.ConfigurationOptions);
    return new RedisCache(obj.GetService<ILoggerFactory>().CreateLogger<RedisCache>(), connection, config, serializer);
});
builder.Services.AddSerializer(sb=>
{
    sb.AddJsonSerializer(
       isSupported: type => type.Namespace.StartsWith("Ordering"));
}); // �����ú����л�������Ҫ��������,��grpcһ����
builder.Host.UseOrleans(clientBuilder =>
{
    clientBuilder
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "order";
            options.ServiceId = "ordering.webapi";
        });
});
builder.Services.AddHostedService<CreateOrderBackgroundService>();

var app = builder.Build();
ApplicationStartup.CreateTable(app.Services);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}
app.UseCors("AllowSpecificOrigin");

app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();