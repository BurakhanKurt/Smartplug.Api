using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Smartplug.Api.Application.Jwt;
using Smartplug.Application.Dtos.Auth;
using Smartplug.Core.Dtos;
using Smartplug.Domain.Entities;
using Smartplug.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace Smartplug.Application.Handlers.Auth.Command
{
    public record RegisterCommand : IRequest<Response<LoginResponse>>
    {
        public string Name { get; init; }
        public string Surname { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }

    public class RegisterCommandHandler(
        UserManager<Users> userManager,
        SignInManager<Users> signInManager,
        JwtGenerator jwtGenerator,
        SmartplugDbContext dbContext,
        ILogger<RegisterCommandHandler> logger
    ) : IRequestHandler<RegisterCommand, Response<LoginResponse>>
    {
        public async Task<Response<LoginResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
            {
                return Response<LoginResponse>.Fail("error.register.usernamealreadyexists", 400);
            }

            var existingEmail = await userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                return Response<LoginResponse>.Fail("error.register.emailalreadyexists", 400);
            }

            var user = new Users
            {
                Name = request.Name,
                Surname = request.Surname,  
                UserName = request.Username,
                Email = request.Email,
                IsDeleted = false,
                SecondPhoneNumber = "",
                EmailConfirmed = true 
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogWarning("User registration failed: {Errors}", errors);
                return Response<LoginResponse>.Fail(errors + errors, 400);
            }

            var accessToken = await jwtGenerator.GenerateJwt(user);
            var refreshToken = await jwtGenerator.GenerateRefreshToken(user);

            return Response<LoginResponse>.Success(new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken
            }, 200);
        }
    }
}
