using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using Trey2.DB;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Trey2.Services;
using MySqlConnector;
using Trey2.DB.IFunction;
using DotNetEnv;

namespace Trey2
{
    public class Program
    {  
        public static void Main(string[] args)
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            

            builder.Configuration.AddJsonFile("appsettings.json", optional: true).AddEnvironmentVariables();
            var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("frontend", policy =>
                {
                    policy.WithOrigins(origins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
            builder.Services.AddScoped<JwtService>();
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
            var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"]);

            builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["access_token"];
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:ISSUER"],
                    ValidAudience = builder.Configuration["JWT:AUDIENCE"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"]))
                };
            });


            builder.Services.AddAuthorization();
            var app = builder.Build();
            app.UseCors("frontend");
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
            
        }
    }
}
