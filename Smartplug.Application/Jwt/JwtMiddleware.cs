using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Smartplug.Application.Jwt
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;


        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var validateTokenResult = await jwtGenerator.ValidateToken(token);

            if (validateTokenResult.IsValid)
            {
                // Gerçek IP adresini al
                var realIp = jwtGenerator.GetIpAddress(context) ?? "LOCAL";

                // Kullanıcıyı elde et
                var user = context.Items["User"] != null ? (Users)context.Items["User"] : await _userManager.FindByIdAsync(validateTokenResult.UserId);
                context.Items["User"] = user;


                if (contextIp.IsNullOrEmpty())
                {
                    context.Items[SessionVariables.IpAddress] = user.LastLoginIp;
                    contextIp = user.LastLoginIp;
                }

                // Kullanıcının doğrulama tarihini kontrol et (24 saat kuralı)
                if (user.LastVerifyingDate.HasValue && user.LastVerifyingDate.Value <= DateTime.UtcNow.AddHours(-24))
                {
                    context.Response.StatusCode = 401; // 24 saatten fazla geçmişse 401 döner
                    return;
                }

                // IP adresi kontrolü
                if (!realIp.IsNullOrEmpty() && string.IsNullOrEmpty(contextIp))
                {
                    // Aynı Ip den istek geliyor
                    if (realIp.Equals(user.LastLoginIp))
                    {
                        context.Items[SessionVariables.IpAddress] = realIp;
                    }
                    else
                    {
                        context.Response.StatusCode = 401; // IP adresi uyuşmuyorsa 401 döner
                        return;
                    }
                }
                else
                {
                    // Session'daki IP ile gerçek IP'yi karşılaştır
                    if (!realIp.Equals(contextIp))
                    {
                        context.Response.StatusCode = 401; // IP adresi uyuşmuyorsa 401 döner
                        return;
                    }
                }
            }

            if (!validateTokenResult.IsValid && token != null)
            {
                context.Response.StatusCode = 401; // Token geçersizse 401 döner
                return;
            }
            await _next(context);
        }

        public string GetIpAddress(HttpContext ctx)
        {
            if (ctx == null)
                return "";
            HttpRequest req = ctx.Request;
            string ip = string.Empty;
            ip = ctx.GetServerVariable("YK-TNEILC-IP");
            if (string.IsNullOrEmpty(ip))
            {
                ip = ctx.GetServerVariable("HTTP_X_FORWARDED_FOR");
                if (string.IsNullOrEmpty(ip))
                {
                    ip = ctx.GetServerVariable("HTTP_FORWARDED");
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = ctx.GetServerVariable("X-FORWARDED-FOR");
                        if (string.IsNullOrEmpty(ip))
                        {
                            ip = ctx.GetServerVariable("X-Forwarded-For");
                            if (string.IsNullOrEmpty(ip))
                            {
                                ip = ctx.GetServerVariable("X-Client-IP");
                                if (string.IsNullOrEmpty(ip))
                                {
                                    ip = req.HttpContext.Connection.RemoteIpAddress.ToString();
                                }
                                else
                                    ip = ip.Split(',')[0];
                            }
                            else
                                ip = ip.Split(',')[0];
                        }
                        else
                            ip = ip.Split(',')[0];
                    }

                    else
                        ip = ip.Split(',')[0];
                }
                else
                    ip = ip.Split(',')[0];
            }
            else
                ip = ip.Split(',')[0];

            return ip;
        }
    }
}
