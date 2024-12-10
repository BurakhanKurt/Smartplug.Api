using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smartplug.Core.ControllerBases;

namespace Smartplug.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : CustomControllerBase
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
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateActionResultInstance(response);
        }
    }
}
