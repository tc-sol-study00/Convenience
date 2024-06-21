﻿using Convenience.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConvenienceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConvenienceContext") ?? throw new InvalidOperationException("Connection string 'ConvenienceContext' not found.")));

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

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

app.MapControllerRoute(
name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();