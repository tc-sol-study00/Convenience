using Convenience.Data;
using Convenience.Models.Services;
using Convenience.Models.Properties;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Identity;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConvenienceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConvenienceContext") ?? throw new InvalidOperationException("Connection string 'ConvenienceContext' not found.")));

//
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//認証機能
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ConvenienceContext>();

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

//DIコンテナのサービス登録
//Dependency Injection（依存性の注入）
//サービス
builder.Services.AddScoped<IChumonService, ChumonService>();
builder.Services.AddScoped<IShiireService, ShiireService>();
builder.Services.AddScoped<IZaikoService, ZaikoService>();
builder.Services.AddScoped<ITentoHaraidashiService, TentoHaraidashiService>();
builder.Services.AddScoped<IKaikeiService, KaikeiService>();
builder.Services.AddScoped<ITentoZaikoService, TentoZaikoService>();
builder.Services.AddScoped<IKaikeiJissekiService, KaikeiJissekiService>();
builder.Services.AddScoped<IChumonJissekiService, ChumonJissekiService>();
builder.Services.AddScoped<IShiireJissekiService, ShiireJissekiService>();
builder.Services.AddScoped<INaigaiClassMasterService, NaigaiClassMasterService>();
builder.Services.AddScoped<IShiireMasterService, ShiireMasterService>();
builder.Services.AddScoped<IShiireSakiMasterService, ShiireSakiMasterService>();
builder.Services.AddScoped<IShohinMasterService, ShohinMasterService>();

//プロパティ
builder.Services.AddScoped<IChumon, Chumon>();
builder.Services.AddScoped<IShiire, Shiire>();
builder.Services.AddScoped<IZaiko, Zaiko>();
builder.Services.AddScoped<ITentoHaraidashi, TentoHaraidashi>();
builder.Services.AddScoped<IKaikei, Kaikei>();
builder.Services.AddScoped<IConvertObjectToCsv, ConvertObjectToCsv>();

//ServiceでTempDataを使うためのＤＩ
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddRazorPages();

builder.Services.AddSession();

WebApplication app = builder.Build();

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

//認証画面用
app.MapRazorPages();

app.Run();
