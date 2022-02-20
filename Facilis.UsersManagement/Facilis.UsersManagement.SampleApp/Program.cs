using Facilis.Core.Abstractions;
using Facilis.Core.EntityFrameworkCore;
using Facilis.UsersManagement.SampleApp;
using Microsoft.EntityFrameworkCore;

static void ConfigureService(WebApplicationBuilder builder)
{
    builder.Services.AddControllersWithViews();
    builder.Services
        .AddDbContext<AppDbContext>(option =>
            option.UseSqlite("Data Source=./local.db;")
        )
        .AddDbContext<DbContext, AppDbContext>()
        .AddScoped(typeof(IEntities<>), typeof(Entities<>));
}

static void Configure(WebApplication app)
{
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    using var scope = app.Services.CreateScope();
    scope.ServiceProvider
        .GetService<DbContext>()
        .Database
        .EnsureCreated();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
}

var builder = WebApplication.CreateBuilder(args);
ConfigureService(builder);

using var app = builder.Build();
Configure(app);
app.Run();