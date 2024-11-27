using App.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using App.Services;
using App.Services.Abstract;
using App.Services.Concrete;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new InvalidOperationException("SqlServer connection string is missing");

builder.Services.AddDataLayer(connectionString);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = false,
        SignatureValidator = (token, parameters) => new JwtSecurityToken(token)
    };
});

// Add Dependency Injection 
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductCommentService, ProductCommentService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DbContext>();
    await context.Database.EnsureCreatedAsync();
}

await app.RunAsync();
