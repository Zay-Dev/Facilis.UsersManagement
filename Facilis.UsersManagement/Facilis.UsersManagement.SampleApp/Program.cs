using Facilis.Core.Abstractions;
using Facilis.Core.EntityFrameworkCore.Abstractions;
using Facilis.Extensions.EntityFrameworkCore;
using Facilis.UsersManagement;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Helpers;
using Facilis.UsersManagement.SampleApp;
using Facilis.UsersManagement.SampleApp.Helpers;
using Facilis.UsersManagement.SampleApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

static void ConfigureService(WebApplicationBuilder builder)
{
    builder.Services
        .AddControllersWithViews();

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
        .AddDefaultPasswordHasher()
        .AddDefaultAuthenticationHistories()

        .AddPasswordBased<PasswordBasedAuthenticator<User>, User>()
        .AddTokenBased<TokenBasedAuthenticator<UserToken, User>, User>()

        .AddScoped<IEntityStampsBinder>(provider => new EntityStampsBinder()
        {
            SystemOperatorIdentifier = nameof(System),
            CurrentUserIdentifier = provider.GetService<IHttpContextAccessor>()
                .HttpContext?
                .User?
                .GetIdentifier()?
                .Value
        })

        .AddDbContext<AppDbContext>(option =>
            option.UseSqlite("Data Source=./local.db;")
        )
        .AddDefaultEntities()
        .UseProfileAttributesBuilder<AppDbContext, ProfileAttributesBinder>()

        .AddScoped<UserOtpService>();
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

    JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };
}

var builder = WebApplication.CreateBuilder(args);
ConfigureService(builder);

using var app = builder.Build();
Configure(app);
app.Run();