using Infrastructure;
using Scalar.AspNetCore;
using SocialMedia.Application;
using SocialMedia.Extensions;
using SocialMedia.Middleware;
using SocialMedia.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseLoggerMiddleware();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();

