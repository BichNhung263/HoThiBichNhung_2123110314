using HoThiBichNhung_2123110314.Data;
using Microsoft.EntityFrameworkCore;

namespace HoThiBichNhung_2123110314
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Đăng ký SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ✅ CORS (cho phép gọi API từ frontend khác domain như React, Render,...)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Tự động chạy Migration khi khởi động
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = "swagger"; // Swagger chạy ở /swagger
            });

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            // ✅ Chỉ dùng PORT khi deploy (Render)
            if (!app.Environment.IsDevelopment())
            {
                var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
                app.Urls.Add($"http://0.0.0.0:{port}");
            }

            app.Run();
        }
    }
}