using NSwag;
using Common.Util.Jwt;
using DistributedId;
using Ordering.WebApi.Filters;
using MagicOnion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthorizationFilter>();
});

builder.Services.AddJwt(builder.Configuration);
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
    settings.AddSecurity("�����֤Token", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
    {
        Scheme = "bearer",
        Description = "Authorization:Bearer {your JWT token}<br/><b>��Ȩ��ַ:/Token/GetToken</b>",
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Type = OpenApiSecuritySchemeType.Http
    });
});
#region ѩ��id �ֲ�ʽ
builder.Services.AddDistributedId(new DistributedIdOptions
{
    Distributed = true
});
#endregion
//builder.Services.AddJwt(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}
app.UseCors("AllowSpecificOrigin");

app.UseRouting();
app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireAuthorization();
});
app.Run();