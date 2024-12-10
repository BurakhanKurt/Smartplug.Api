using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Smartplug.Api.Application.Jwt;
using Smartplug.Application.Dtos.Auth;
using Smartplug.Core.Dtos;
using Smartplug.Domain.Entities;
using Smartplug.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Smartplug.Application.Handlers.Auth.Command
{
    public record LoginCommand : IRequest<Response<LoginResponse>>
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class AuthCommandHandler(JwtGenerator jwtGenerator, SignInManager<Users> signInManager, UserManager<Users> userManager,
        IHttpContextAccessor httpContextAccessor, ILogger<AuthCommandHandler> logger, SmartplugDbContext dbContext, IMediator mediator)
        : IRequestHandler<LoginCommand, Response<LoginResponse>>
    {
        public async Task<Response<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            var user = await signInManager.UserManager.Users
                .Where(u => u.Email == request.Username && !u.IsDeleted)
                .FirstOrDefaultAsync();

            if (user == null || user.IsDeleted)
            {
                return Response<LoginResponse>.Fail("error.login.invalidcredentials",404);
            }

            await userManager.UpdateAsync(user);

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);

            if (result.Succeeded)
            {

                var accessToken = await jwtGenerator.GenerateJwt(user);
                var refreshToken = await jwtGenerator.GenerateRefreshToken(user);

                return Response<LoginResponse>.Success(new LoginResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken
                },200);
            }
            else
            {
                return Response<LoginResponse>.Fail("error.login.invalidcredentials",404);
            }
        }

    }
}
