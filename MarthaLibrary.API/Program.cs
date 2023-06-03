using Azure.Storage.Blobs;
using Hangfire;
using Hangfire.Dashboard;
using MarthaLibrary.Application.Interface;
using MarthaLibrary.Application.Services;
using MarthaLibrary.Infrastructure.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(configuration =>
{
    configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("DbConnection"));
});
builder.Services.AddHangfireServer();
builder.Services.AddDbContext<MarthaLibraryDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
builder.Services.AddTransient<IBookService, BookService>();
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
app.UseAuthorization();

app.MapControllers();

app.Run();
public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}