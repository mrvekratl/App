using Microsoft.AspNetCore.Authentication.JwtBearer;
using App.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný baðlantýsýný ekleyin
var connectionString = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddDataLayer(connectionString); // Veritabaný yapýlandýrmasý

builder.Services.AddControllers();

// JWT Authentication için gerekli ayarlarý ekleyin
var key = builder.Configuration["Jwt:Key"];  // appsettings.json dosyasýndaki key'i alýyoruz
var issuer = builder.Configuration["Jwt:Issuer"]; // appsettings.json dosyasýndaki issuer'ý alýyoruz
var audience = builder.Configuration["Jwt:Audience"]; // appsettings.json dosyasýndaki audience'ý alýyoruz

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

    // IFormFile desteði için binary format ekleyin
    c.MapType<IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    // FormData için destek ekleyin
    c.OperationFilter<SwaggerFileOperationFilter>();
});



var app = builder.Build();

// HTTP request pipeline'ýný yapýlandýrýn
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
