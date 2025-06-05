// Trong GameVaultManager.AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

// --- Redis Cache (Bạn đã có) ---
var cache = builder.AddRedis("gvm-cache"); // Đặt tên resource cache rõ ràng hơn

// --- PostgreSQL Database ---
// "pg-gvm" là tên resource cho container PostgreSQL
// "gamevaultdb" là tên database cụ thể sẽ được tạo
var postgres = builder.AddPostgres("pg-gvm")
                      .WithPgAdmin(); // Tùy chọn: thêm PgAdmin
postgres.WithEndpoint(
       name: "gamevaultdb",
       port: 5432,
       targetPort: 5433,
       isExternal: true
       );
var gameVaultDatabase = postgres.AddDatabase("gamevaultdb");

// --- API Service ---
// !!! QUAN TRỌNG: Hãy đảm bảo Projects.GameVaultManager_Server khớp với tên Assembly project API của bạn
// Nếu tên Assembly của bạn vẫn là GameVaultMangager_ApiService, hãy dùng tên đó.
// Tôi sẽ dùng tên đã được sửa lỗi chính tả để làm ví dụ:
var apiService = builder.AddProject<Projects.GameVault_ApiService>("apiservice") // Đã sửa thành Server
                        .WithReference(gameVaultDatabase) // << API Service kết nối tới PostgreSQL
                        .WithReference(cache);          // << API Service cũng có thể dùng Redis cache

// --- Web Frontend (Blazor WASM Client) ---
// !!! QUAN TRỌNG: Hãy đảm bảo Projects.GameVaultManager_Client khớp với tên Assembly project Web của bạn
builder.AddProject<Projects.GameVault_Web>("webfrontend") // Đã sửa thành Client
       .WithExternalHttpEndpoints()
       // Client không nhất thiết phải tham chiếu trực tiếp đến Redis.
       // Nó sẽ lấy dữ liệu đã được cache (nếu có) thông qua apiService.
       .WithReference(apiService)
       .WaitFor(apiService); // Client đợi apiService là hợp lý

builder.Build().Run();