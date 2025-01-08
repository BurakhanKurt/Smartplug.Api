using System.Security.Cryptography.Xml;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smartplug.Application.Dtos.Auth.Device;
using Smartplug.Application.Handlers.Auth.Command;
using Smartplug.Application.Handlers.Auth.Command.Device;
using Smartplug.Core.ControllerBases;

namespace Smartplug.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : CustomControllerBase
    {
        

        /// <summary>
        /// Login
        /// </summary>
        ///<remarks>
        /// 
        ///Example Request:
        ///
        ///     {
        ///         "Username":"admin@smartplug.com",
        ///         "Password":"P@ssw0rd",
        ///     }
        ///     
        /// </remarks>
        /// <returns>
        /// </returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var response = await mediator.Send(command);
            return CreateActionResultInstance(response);
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromQuery] RefreshTokenCommand command)
        {
            var response = await mediator.Send(command);
            return CreateActionResultInstance(response);
        }
        
        [HttpPost("/callback/signature_device")]
        [Authorize]
        public async Task<IActionResult> SignatureDevice([FromBody] SignatureDeviceRequest request)
        {
            var response = await mediator.Send(new SignatureDeviceCommand
            {
                Data = request
            });
            return CreateActionResultInstance(response);
        }
    }
}
