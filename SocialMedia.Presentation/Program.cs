using System.Text;
using Infrastructure;
using Infrastructure.Contracts;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Mappings;
using SocialMedia.Application.Services;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var connectionString = config.GetConnectionString("Default");
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SocialMediaContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddLogging();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IPasswordService, PasswordService>();
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
    });

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

