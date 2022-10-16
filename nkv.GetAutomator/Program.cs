using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using MySqlConnector;
using nkv.GetAutomator.Data;
using nkv.GetAutomator.Data.DataAccess;
using nkv.GetAutomator.Data.DBContext;
using nkv.GetAutomator.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMudServices();
builder.Services.AddDbContextFactory<MySQLContext>
    (options =>
    {
        var connetionString = builder.Configuration.GetConnectionString("Default");
        options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
    });
builder.Services.AddSingleton<IProductDataAccess, ProductDataAccess>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();