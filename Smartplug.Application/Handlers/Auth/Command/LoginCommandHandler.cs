using MediatR;
using Microsoft.Extensions.Configuration;
using Smartplug.Application.Dtos.Auth;
using Smartplug.Core.Dtos;
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
        public string? CaptchaToken { get; set; }
        public string? CaptchaKey { get; set; }
        public string Language { get; set; }
        public string? SmsCode { get; set; }
        public string? PhoneNumber { get; set; }
    }

}
