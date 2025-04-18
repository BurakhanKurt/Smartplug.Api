﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Smartplug.Api.Application.Jwt;
using Smartplug.Domain.Entities;

namespace Smartplug.Application.Jwt
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;


        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context,JwtGenerator jwtGenerator, UserManager<Users> userManager)
        {
            if (context.Request.Path.Equals("/login", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var validateTokenResult = await jwtGenerator.ValidateToken(token);

            //if (validateTokenResult.IsValid)
            //{
            //    // Kullanıcıyı elde et
            //    var user = context.Items["User"] != null ? (Users)context.Items["User"] : await _userManager.FindByIdAsync(validateTokenResult.UserId);
            //    context.Items["User"] = user;             
            //}

            if (!validateTokenResult.IsValid && token != null)
            {
                context.Response.StatusCode = 401; // Token geçersizse 401 döner
                return;
            }
            await _next(context);
        }
    }
}
