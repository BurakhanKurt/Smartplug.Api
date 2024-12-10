using MediatR;
using Microsoft.Extensions.Configuration;
using Smartplug.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Smartplug.Application.Handlers.Auth.Command
{
    public class LoginCommand : IRequest<Response<TokenResponse>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<TokenResponse>>
    {
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Response<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Örnek kullanıcı kontrolü (Veritabanı bağlantısı yerine statik kontrol)
            if (request.Username != "admin@yurticikargo.com" || request.Password != "P@ssw0rd")
            {
                return Response<TokenResponse>.Fail("Kullanıcı adı veya şifre hatalı.", 401);
            }

            // JWT token oluşturma
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, "Admin") // Kullanıcı rolü örnek
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenResponse = new TokenResponse
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = tokenDescriptor.Expires.Value
            };

            return Response<TokenResponse>.Success(tokenResponse, 200);
        }
    }
}
