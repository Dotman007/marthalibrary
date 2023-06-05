using Azure.Storage.Blobs;
using Hangfire;
using Hangfire.Dashboard;
using IdentityServer4.AccessTokenValidation;
using MarthaLibrary.Application.Helper;
using MarthaLibrary.Application.Interface;
using MarthaLibrary.Application.Services;
using MarthaLibrary.Domain.Constant;
using MarthaLibrary.Domain.Entities;
using MarthaLibrary.Infrastructure.DataAccessLayer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHangfire(configuration =>
        {
            configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("DbConnection"));
        });
        builder.Services.AddHangfireServer();
        builder.Services.AddAuthentication(e =>
        {
            e.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        builder.Services.AddAuthorization();
        builder.Services.AddDbContext<MarthaLibraryDb>(options => options.UseInMemoryDatabase("MarthaDb"));
        builder.Services.AddTransient<IBookService, BookService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IJwtUtils, JwtUtils>();
        builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetSection("BlobStorageSettings:ConnectionString").Value));
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseHangfireDashboard("/backjob", new DashboardOptions()
        {
            AppPath = null,
            DashboardTitle = "Hangfire Dashboard",
            Authorization = new[] { new HangFireAuthorizationFilter() }
        });
       
        //Check Reservation Expiry
        RecurringJob.AddOrUpdate<IBookService>(e => e.CheckReservedExpiry(), Cron.Minutely());
        app.UseAuthentication();
        app.UseAuthorization();

        {
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // global error handler

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.MapControllers();
        }
        {
            var testUsers = new List<User>
    {
        new User { Id = Guid.NewGuid().ToString(), FullName = "Martha Lucas", UserName = "martha909", PasswordHash = BCrypt.Net.BCrypt.HashPassword("@@OLUWAseun10"), Role = Role.Admin },
    };

            using var scope = app.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<MarthaLibraryDb>();
            dataContext.Users.AddRange(testUsers);
            dataContext.SaveChanges();
        }
        app.Run();
    }
}

public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}