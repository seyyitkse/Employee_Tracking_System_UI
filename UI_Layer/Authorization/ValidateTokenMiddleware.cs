using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UI_Layer.Authorization
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ValidateTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("AuthenticationToken", out var token))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null)
                {
                    var identity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                }
            }

            await _next(context);
        }
    }

}
