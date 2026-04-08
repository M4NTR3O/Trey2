using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using Trey2.DB;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text;

namespace Trey2
{
    public class Program
    {  
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("DB/Params.json", optional: true, reloadOnChange: true);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
            builder.Services.AddHttpClient("python", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["PythonApi:BaseUrl"]);
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Trey2 API",
                    Description = "Документация API",
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            var app = builder.Build();
            app.UseCors("AllowAll");
            app.UseMiddleware<RequestLoggingMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Trey2 API V1");
                    options.RoutePrefix = string.Empty; // Стартовая страница = Swagger
                }); // по умолчанию будет на /swagger
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
            
        }
    }
}
