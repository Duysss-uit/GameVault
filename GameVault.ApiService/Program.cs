// Ensure the correct namespace is used for ApplicationDbContext
using Infrastructure.Persistence;
using GameVault.Application;
using Microsoft.EntityFrameworkCore;
using Aspire.Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults(); // Giữ lại dòng này, quan trọng cho Aspire

// Add services to the container.
builder.Services.AddProblemDetails();

// --- ĐĂNG KÝ DBCONTEXT ---
// Sử dụng tên database resource bạn đã định nghĩa trong AppHost (ví dụ: "gamevaultdb")
builder.AddNpgsqlDbContext<ApplicationDbContext>("gamevaultdb", optionsBuilder =>
{
    // Tùy chọn: Bạn có thể thêm các cấu hình Npgsql hoặc EF Core cụ thể ở đây nếu cần
    // Ví dụ: optionsBuilder.EnableRetryOnFailure();
    // optionsBuilder.UseSnakeCaseNamingConvention(); // Nếu bạn muốn tên bảng/cột theo snake_case
});

// --- ĐĂNG KÝ SERVICES CỦA BẠN (Sẽ làm ở bước sau) ---
// Ví dụ:
// builder.Services.AddScoped<IUserGameStatusService, UserGameStatusService>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Cấu hình Swagger/OpenAPI (Giữ lại nếu bạn muốn dùng Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Thêm Controllers nếu bạn dùng (thay vì Minimal APIs)
// builder.Services.AddControllers();
// TEMPORARY: Log the connection string
var tempServices = builder.Services.BuildServiceProvider();
var config = tempServices.GetService<IConfiguration>();
var connectionString = config.GetConnectionString("gamevaultdb");
Console.WriteLine($"DEBUG: Connection string 'gamevaultdb' = {connectionString}");
// END TEMPORARY
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

// Map Controllers nếu bạn dùng
// app.MapControllers();
app.Run();

