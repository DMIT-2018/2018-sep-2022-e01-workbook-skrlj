using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

#region Additional Namespaces
using ChinookSystem;
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Supplied database connection due to the fact that we've created this web app to use Individual Accounts
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add another GetConncetionString to reference our own database connection string - in this case Chinook
var connectionStringChinook = builder.Configuration.GetConnectionString("ChinookDB");

// Supplied for the DB connection to DefaultConnectionString
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Code the DBConnection to the application DB context for Chinook
// The implementation of the connect and registration of the Chinook system services will be done in the ChinookSystem class library
// To accomplish this task, we will be using an "extension method"
// The extension method will extend the IServiceCollection Class
// The extension method require a parameter options.UseSqlServer(XXX). XXX is the connection string variable

builder.Services.ChinookSystemBackendDependencies(options => options.UseSqlServer(connectionStringChinook));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
