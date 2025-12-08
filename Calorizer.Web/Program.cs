using Calorizer.Business.Interfaces;
using Calorizer.Business.Services;
using Calorizer.DAL.Models;
using Calorizer.DAL.Repositories;
using Calorizer.Web.Middleware;
using Calorizer.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<CalorizerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CalorizerDb")));

// Register Generic Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Localization Service (Web Layer)
builder.Services.AddSingleton<ILocalizationService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<LocalizationService>>();
    var jsonFilePath = Path.Combine(env.ContentRootPath, "Resources", "Localization.json");
    return new LocalizationService(jsonFilePath, logger);
});

// Register Localization Provider (Business Layer Interface)
builder.Services.AddSingleton<ILocalizationProvider, LocalizationAdapter>();

// Register Localizer - THE MAGIC HAPPENS HERE!
builder.Services.AddScoped<Localizer>();

// Add Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor to access session in services
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // ✅ Only this for development - shows detailed errors
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// Add Language Middleware AFTER UseSession
app.UseMiddleware<LanguageMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();