using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using UI_Layer.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

var jwtIssuer = builder.Configuration["AuthSettings:Issuer"];
var jwtKey = builder.Configuration["AuthSettings:Key"];
var jwtAudience = builder.Configuration["AuthSettings:Audience"];
var key = Encoding.ASCII.GetBytes(jwtKey);

// Cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/LoginAdmin/LoginAdmin";
        options.AccessDeniedPath = "/ErrorPage/AccessDenied";
    });

// Authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EmployeePolicy", policy => policy.RequireRole("Employee"));
});

// Cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/ErrorPage/ErrorPage"); // Özel hata sayfanýz
    app.UseHsts();
}

// Durum kodu sayfalarýný yeniden yönlendirme
app.UseStatusCodePagesWithReExecute("/ErrorPage/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();

// Middleware to add Authorization header
app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["AuthenticationToken"];
    if (!string.IsNullOrEmpty(token))
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Headers.Remove("Authorization");
        }
        context.Request.Headers.Add("Authorization", $"Bearer {token}");
    }
    await next();
});

app.UseMiddleware<ValidateTokenMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HomeScreen}/{action=HomePage}/{id?}");

app.Run();
