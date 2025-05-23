using AkilliSaglikRaporu.Data;
using AkilliSaglikRaporu.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Register Services
builder.Services.AddHttpClient<IIpfsService, IpfsService>();
builder.Services.AddHttpClient<ISmartContractService, SmartContractService>();

builder.Services.AddScoped<IIpfsService, IpfsService>();
builder.Services.AddScoped<ISmartContractService, SmartContractService>();

// Configure Smart Contract Options
builder.Services.Configure<SmartContractOptions>(
    builder.Configuration.GetSection("SmartContractOptions"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
