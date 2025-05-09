using System.Text;
using Infrastructure.Messaging.Producers;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMedia;
using SocialMedia.Application;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Mappings;
using SocialMedia.Application.Services;
using SocialMedia.Application.UseCases;
using SocialMedia.Extensions;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var connectionString = config.GetConnectionString("DefaultProduction");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostDev", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost:")) 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
    options.AddPolicy("Production", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("https://socialmediapp.azurewebsites.net")) 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<SocialMediaContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddLogging();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddScoped<ISendMessageUseCase, SendMessageUseCase>();
builder.Services.AddScoped<IEventProducer, KafkaProducerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<IFeedService, FeedService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddTransient<IPasswordService, PasswordService>();
builder.Services.AddSignalR();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true, 
            ValidIssuer = config["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = config["Jwt:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
            ValidateIssuerSigningKey = true
                
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/chathub")))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseLoggerMiddleware();
app.UseCors(app.Environment.IsDevelopment() ? "AllowLocalhostDev" : "Production");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHub<ChatHub>("/chathub");
app.Run();

