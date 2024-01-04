using Cache.Options;
using Cache;
using Common.Util.Jwt;
using Customers.Center.Service;
using Customers.Domain.Customers;
using Customers.Domain.Seedwork;
using Customers.Infrastructure;
using Customers.Infrastructure.Domain;
using Customers.Infrastructure.Domain.Customers;
using DistributedId;
using NetCore.AutoRegisterDi;
using NSwag;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "��̨����ϵͳ";
    settings.AllowReferencesWithProperties = true;
    settings.AddSecurity("�����֤Token", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
    {
        Scheme = "bearer",
        Description = "Authorization:Bearer {your JWT token}<br/><b>��Ȩ��ַ:/Token/GetToken</b>",
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Type = OpenApiSecuritySchemeType.Http
    });
});
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddDB(builder.Configuration);

builder.Services.AddGrpc();
builder.Services.AddMagicOnion();
//builder.Services.RegisterAssemblyPublicNonGenericClasses();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
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
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// ���CORS����
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
var app = builder.Build();
ApplicationStartup.CreateTable(app.Services);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}
// ����CORS�м��
app.UseCors("AllowSpecificOrigin");
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireAuthorization();
});
//app.MapControllers();
app.MapMagicOnionService();
app.Run();
