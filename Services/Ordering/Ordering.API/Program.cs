using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extencions;

var builder = WebApplication.CreateBuilder(args);

//add services to container
builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);


var app = builder.Build();

//configure the http request pipeline

app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    await app.InitialDatabaseAsync();
}

app.Run();
