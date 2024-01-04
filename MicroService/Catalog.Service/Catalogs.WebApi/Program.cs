using Microsoft.Extensions.DependencyInjection.Extensions;
using Catalogs.Infrastructure;
using Catalogs.Infrastructure.Database;
using Cache.Options;
using DistributedId;
using Cache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDB(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<CatalogContextSeed>();
// ���CORS����
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.AllowAnyOrigin() // �����ض�����Դ
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
#region ѩ��id �ֲ�ʽ
builder.Services.AddCache(new CacheOptions
{
    CacheType = CacheTypes.Redis,
    RedisConnectionString = builder.Configuration["DistributedRedis:ConnectionString"] ?? throw new Exception("$δ�ܻ�ȡdistributedredis�����ַ���")
}).AddDistributedId(new DistributedIdOptions
{
    Distributed = true
});
#endregion

var app = builder.Build();
ApplicationStartup.CreateTable(app.Services);
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seedService = services.GetService<CatalogContextSeed>();
    seedService.SeedAsync().Wait();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// ����CORS�м��
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();
