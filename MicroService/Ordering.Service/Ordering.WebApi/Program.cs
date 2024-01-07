using NSwag;
using Common.Util.Jwt;
using DistributedId;
using Ordering.WebApi.Filters;
using MagicOnion;
using Ordering.WebApi.Services;
using Ordering.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
builder.Services.AddTransient<IOrderService, OrderService>();
var app = builder.Build();

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