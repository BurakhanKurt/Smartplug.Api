using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smartplug.Application.Handlers.Plug.Command;
using Smartplug.Core.ControllerBases;

namespace Smartplug.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlugController(IMediator mediator) : CustomControllerBase
    {
        /// <summary>
        /// Prizi açma veya kapatma
        /// </summary>
        /// <param name="command">Güç durumu ve cihaz IP adresi</param>
        /// <returns>Başarılı olup olmadığı</returns>
        [HttpPost("set-power")]
        public async Task<IActionResult> SetPower([FromBody] SetPowerCommand command)
        {
            return Ok();
        }

        /// <summary>
        /// Güç durumunu sorgula
        /// </summary>
        /// <param name="deviceIp">Cihaz IP adresi</param>
        [HttpGet("get-power-state")]
        public async Task<IActionResult> GetPowerState([FromQuery] string deviceIp)
        {
            // Priz durumu alındı
            return Ok(new { DeviceIp = deviceIp, PowerState = true });
        }

        /// <summary>
        /// Zamanlayıcı ayarla
        /// </summary>
        /// <param name="command">Zamanlayıcı bilgisi</param>
        [HttpPost("set-timer")]
        public async Task<IActionResult> SetTimer([FromBody] SetTimerCommand command)
        {
            // Zamanlayıcı bilgisi kaydedildi
            return Ok("Zamanlayıcı ayarlandı.");
        }

        /// <summary>
        /// Zamanlayıcıları listele
        /// </summary>
        /// <param name="deviceIp">Cihaz IP adresi</param>
        [HttpGet("list-timers")]
        public async Task<IActionResult> ListTimers([FromQuery] string deviceIp)
        {
            // Örnek zamanlayıcılar
            var timers = new[]
            {
            new { TimerId = 1, DeviceIp = deviceIp, PowerState = true, ScheduledTime = "2024-12-10T10:00:00Z" },
            new { TimerId = 2, DeviceIp = deviceIp, PowerState = false, ScheduledTime = "2024-12-10T22:00:00Z" }
        };

            return Ok(timers);
        }

        /// <summary>
        /// Enerji tüketimini al
        /// </summary>
        /// <param name="deviceIp">Cihaz IP adresi</param>
        [HttpGet("get-energy-consumption")]
        public async Task<IActionResult> GetEnergyConsumption([FromQuery] string deviceIp)
        {
            // Enerji tüketimi bilgisi döndü
            return Ok(new { DeviceIp = deviceIp, EnergyConsumption = "5.2 kWh" });
        }

        /// <summary>
        /// Cihaz listesini al
        /// </summary>
        [HttpGet("list-devices")]
        public async Task<IActionResult> ListDevices()
        {
            // Örnek cihazlar
            var devices = new[]
            {
            new { DeviceIp = "192.168.1.100", Name = "Salon Prizi" },
            new { DeviceIp = "192.168.1.101", Name = "Mutfak Prizi" }
        };

            return Ok(devices);
        }
    }
}
