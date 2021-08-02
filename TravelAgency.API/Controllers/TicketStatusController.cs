using Microsoft.AspNetCore.Mvc;
using TravelAgency.API.RpcServices;
using TravelAgency.ProtocolBuffers;

namespace TravelAgency.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketStatusController : ControllerBase
    {
        private readonly TicketStatusService _ticketStatusService;

        public TicketStatusController(TicketStatusService ticketStatusService)
        {
            _ticketStatusService = ticketStatusService;
        }

        [HttpPost]
        public IActionResult GetTicketStatus(TicketStatusRequest ticketStatusRequest)
        {
            TicketStatusResponse ticketStatusResponse = _ticketStatusService.GetTicketStatus(ticketStatusRequest);
            return Ok(ticketStatusResponse);
        }
    }
}
