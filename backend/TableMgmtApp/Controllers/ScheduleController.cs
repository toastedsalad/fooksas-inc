// Session
using Microsoft.AspNetCore.Mvc;

namespace TableMgmtApp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase {
        private readonly ScheduleService _scheduleService;

        public SchedulesController(ScheduleService scheduleService) {
            _scheduleService = scheduleService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll() {
            var scheduleDTOs = await _scheduleService.GetAllScheduleDTOs();
            return Ok(scheduleDTOs);
        }
    }
}
    
