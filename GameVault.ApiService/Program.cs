// Ensure the correct namespace is used for ApplicationDbContext
using Aspire.Npgsql.EntityFrameworkCore.PostgreSQL;
using Application.Service;
using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Domain;
using Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults(); // Giữ lại dòng này, quan trọng cho Aspire

// Add services to the container.
builder.Services.AddProblemDetails();

// --- ĐĂNG KÝ DBCONTEXT ---
// Sử dụng tên database resource bạn đã định nghĩa trong AppHost (ví dụ: "gamevaultdb")
builder.AddNpgsqlDbContext<ApplicationDbContext>("gamevaultdb");

// --- ĐĂNG KÝ SERVICES CỦA BẠN ---
// Register DbContext interface mapping
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddScoped<IUserGameStatusService, UserGameStatusService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Cấu hình Swagger/OpenAPI (Giữ lại nếu bạn muốn dùng Swagger)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthentication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Thêm Controllers nếu bạn dùng (thay vì Minimal APIs)
// builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Map service defaults (quan trọng cho Aspire)
app.MapDefaultEndpoints(); // Giữ lại dòng này

if (app.Environment.IsDevelopment())
{
    app.MapSwagger(); // Sửa từ app.MapOpenApi() nếu bạn dùng AddSwaggerGen()
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// Map Controllers nếu bạn dùng
app.MapControllers();
app.Run();

