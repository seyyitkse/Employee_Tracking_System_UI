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

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtIssuer,
//        ValidAudience = jwtAudience,
//        //IssuerSigningKey = new SymmetricSecurityKey(key)
//    };
//})
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//{
//    options.Events = new CookieAuthenticationEvents
//    {
//        OnValidatePrincipal = async context =>
//        {
//            var token = context.Request.Cookies["AuthenticationToken"];
//            if (token != null)
//            {
//                var handler = new JwtSecurityTokenHandler();
//                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

//                if (jsonToken != null)
//                {
//                    var claimsIdentity = new ClaimsIdentity(jsonToken.Claims, "AuthenticationToken");
//                    context.Principal = new ClaimsPrincipal(claimsIdentity);
//                }
//            }
//        }
//    };
//});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/LoginAdmin/LoginAdmin";
            options.AccessDeniedPath = "/ErrorPage/AccessDenied";
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["AuthenticationToken"];
    if (!string.IsNullOrEmpty(token))
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Headers.Remove("Authorization");
        }
        context.Request.Headers.Add("Authorization", "Bearer " + token);
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
