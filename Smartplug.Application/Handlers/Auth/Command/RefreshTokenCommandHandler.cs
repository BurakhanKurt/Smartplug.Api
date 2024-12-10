using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Smartplug.Api.Application.Jwt;
using Smartplug.Application.Dtos.Auth;
using Smartplug.Application.Services;
using Smartplug.Core.Dtos;
using Smartplug.Domain.Entities;
using Smartplug.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Smartplug.Api.Application.Jwt.JwtGenerator;

namespace Smartplug.Application.Handlers.Auth.Command
{
    public class RefreshTokenCommand : IRequest<Response<LoginResponse>>
    {
        public required string RefreshToken { get; set; }
    }
    public class RefreshTokenHandler :
                         IRequestHandler<RefreshTokenCommand, Response<LoginResponse>>
    {
        private readonly JwtGenerator _jwtGenerator;
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly IUserAccessor _userAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthCommandHandler> _logger;
        private readonly SmartplugDbContext _dbContext;
        private readonly IMediator _mediator;
        public RefreshTokenHandler(JwtGenerator jwtGenerator, SignInManager<Users> signInManager, UserManager<Users> userManager, IUserAccessor userAccessor, IHttpContextAccessor httpContextAccessor, ILogger<AuthCommandHandler> logger, SmartplugDbContext dbContext, IMediator mediator)
        {
            _jwtGenerator = jwtGenerator;
            _signInManager = signInManager;
            _userManager = userManager;
            _userAccessor = userAccessor;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<Response<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tenantId = _jwtGenerator.GetClaim(request.RefreshToken, "tenantid");
                _httpContextAccessor.HttpContext.Items["tenantid"] = tenantId;
                var result = await _jwtGenerator.ValidateToken(request.RefreshToken, TokenType.RefreshToken);
                if (!result.IsValid) return Response<LoginResponse>.Fail("Token not valid", 404);
                var userId = _jwtGenerator.GetClaim(request.RefreshToken, "userid");

                var user = await _userManager.Users.FirstOrDefaultAsync(w => w.Id == Guid.Parse(userId) && !w.IsDeleted);

                if (user == null)
                    return Response<LoginResponse>.Fail("error.login.invalidcredentials", 500);

                var token = await _jwtGenerator.GenerateJwt(user);
                var refreshToken = await _jwtGenerator.GenerateRefreshToken(user);

                return Response<LoginResponse>.Success(new LoginResponse { Token = token, RefreshToken = refreshToken },200);
            }
            catch (Exception ex)
            {
                //_logger.SendError(ex, nameof(RefreshTokenCommand));

                return Response<LoginResponse>.Fail("error.unknown",500);
            }
        }
    }
}
