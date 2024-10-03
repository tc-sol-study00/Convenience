using Convenience.Data;
using Convenience.Models.Services;
using Convenience.Models.Properties;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConvenienceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConvenienceContext") ?? throw new InvalidOperationException("Connection string 'ConvenienceContext' not found.")));

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

//DIコンテナのサービス登録
builder.Services.AddScoped<IChumonService,ChumonService>();
builder.Services.AddScoped<IShiireService,ShiireService>();
builder.Services.AddScoped<IZaikoService, ZaikoService>();
builder.Services.AddScoped<IChumon,Chumon>();
builder.Services.AddScoped<IShiire,Shiire>();
builder.Services.AddScoped<IZaiko, Zaiko>();

//builder.Services.AddRazorPages()
//                    .AddSessionStateTempDataProvider();

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

//TimeZone(JST)でDB更新ができるように
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.MapControllerRoute(
name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();