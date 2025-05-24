// Session
using Microsoft.AspNetCore.Mvc;

namespace TableMgmtApp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService) {
            _scheduleService = scheduleService;
        }

        // GET: api/schedules
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var scheduleDTOs = await _scheduleService.GetAllScheduleDTOs();
            return Ok(scheduleDTOs);
        }

        // GET: api/schedules/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id) {
            var scheduleDTO = await _scheduleService.GetById(id);
            if (scheduleDTO == null) {
                return NotFound($"No schedule found with ID {id}");
            }

            return Ok(scheduleDTO);
        }

        // POST: api/schedules
        [HttpPost]
        public async Task<IActionResult> AddSchedule([FromBody] ScheduleDTO scheduleDTO) {
            if (scheduleDTO == null) {
                return BadRequest("sche is null");
            }
            await _scheduleService.Add(scheduleDTO);
            return CreatedAtAction(nameof(GetById), new { id = scheduleDTO.Id }, scheduleDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id) {
            var scheduleDTO = await _scheduleService.GetById(id);

            if (scheduleDTO == null) {
                return NotFound($"No schedule found with ID {id}");
            }

            await _scheduleService.Delete(scheduleDTO);
            return NoContent();
        }
    }
}
    
