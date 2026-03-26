using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municipality360.Application.Interfaces.Services;
using System.Security.Claims;

namespace Municipality360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsAgentController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsAgentController(INotificationService service)
            => _service = service;

        // ── GET api/notifications ─────────────────────────────────────
        /// <summary>إشعارات العميل المسجّل الدخول</summary>
        [HttpGet]
        public async Task<IActionResult> GetMy([FromQuery] bool seulementNonLues = false)
        {
            var agentId = GetAgentId();
            if (string.IsNullOrEmpty(agentId)) return Unauthorized();

            var items = await _service.GetByAgentAsync(agentId, seulementNonLues);
            return Ok(items);
        }

        // ── GET api/notifications/count ───────────────────────────────
        /// <summary>عدد الإشعارات غير المقروءة</summary>
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var agentId = GetAgentId();
            if (string.IsNullOrEmpty(agentId)) return Unauthorized();

            var count = await _service.GetNombreNonLuesAsync(agentId, estAgent: true);
            return Ok(new { Total = count, NonLues = count });
        }

        // ── PATCH api/notifications/{id}/lue ─────────────────────────
        /// <summary>تحديد إشعار كمقروء</summary>
        [HttpPatch("{id}/lue")]
        public async Task<IActionResult> MarquerLue(int id)
        {
            var agentId = GetAgentId();
            if (string.IsNullOrEmpty(agentId)) return Unauthorized();

            try
            {
                await _service.MarquerLueAsync(id, agentId);
                return Ok();
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // ── PATCH api/notifications/lire-tout ────────────────────────
        /// <summary>تحديد كل الإشعارات كمقروءة</summary>
        [HttpPatch("lire-tout")]
        public async Task<IActionResult> MarquerToutesLues()
        {
            var agentId = GetAgentId();
            if (string.IsNullOrEmpty(agentId)) return Unauthorized();

            await _service.MarquerToutesLuesAsync(agentId, estAgent: true);
            return Ok();
        }

        // ── Helper ───────────────────────────────────────────────────
        private string? GetAgentId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
    }
}
