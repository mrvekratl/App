using Microsoft.AspNetCore.Authentication.JwtBearer;
using App.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritaban� ba�lant�s�n� ekleyin
var connectionString = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddDataLayer(connectionString); // Veritaban� yap�land�rmas�

builder.Services.AddControllers();

// JWT Authentication i�in gerekli ayarlar� ekleyin
var key = builder.Configuration["Jwt:Key"];  // appsettings.json dosyas�ndaki key'i al�yoruz
var issuer = builder.Configuration["Jwt:Issuer"]; // appsettings.json dosyas�ndaki issuer'� al�yoruz
var audience = builder.Configuration["Jwt:Audience"]; // appsettings.json dosyas�ndaki audience'� al�yoruz

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "App.Api.Data",
        Version = "v1"
    });

    // IFormFile deste�i i�in binary format ekleyin
    c.MapType<IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    // FormData i�in destek ekleyin
    c.OperationFilter<SwaggerFileOperationFilter>();
});



var app = builder.Build();

// HTTP request pipeline'�n� yap�land�r�n
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "App.Api.Data v1");
    });

}

app.UseHttpsRedirection();

// Authentication ve Authorization ekleyin
app.UseAuthentication();  // JWT Authentication
app.UseAuthorization();   // Yetkilendirme

app.MapControllers();

app.Run();
