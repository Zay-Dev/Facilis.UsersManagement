using Facilis.Core.Abstractions;
using Facilis.Core.EntityFrameworkCore;
using Facilis.UsersManagement;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;
using Facilis.UsersManagement.SampleApp;
using Facilis.UsersManagement.SampleApp.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

static void ConfigureService(WebApplicationBuilder builder)
{
    builder.Services.AddControllersWithViews();
    builder.Services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/sign-in";
            options.ExpireTimeSpan = TimeSpan.FromHours(1);
            options.SlidingExpiration = true;
        });

    builder.Services
        .AddHttpContextAccessor()
        .AddSingleton<IPasswordHasher, BCryptNetPasswordHasher>()
        .AddScoped<IAuthenticator, Authenticator<User<UserProfile>>>()
        .AddScoped<IOperators>(provider => new Operators()
        {
            SystemOperatorName = nameof(System),
            CurrentOperatorName = provider.GetService<IHttpContextAccessor>()
                .HttpContext?
                .User?
                .Identity?
                .Name,
        })

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
    scope.ServiceProvider.SeedData();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
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