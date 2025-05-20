using Microsoft.AspNetCore.Mvc;

namespace TableMgmtApp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TableManagerController : ControllerBase {
        private readonly TableManagerService _tableManagerService;

        public TableManagerController(TableManagerService tableManagerService) {
            _tableManagerService = tableManagerService;
        }

        // PUT: api/tablemanager/{id}/play
        [HttpPut("{id}/play")]
        public async Task<IActionResult> SetPlay(Guid id, [FromQuery] int timedSeconds = 0) {
            var manager = _tableManagerService.GetTableManager(id);
            if (manager == null) return NotFound($"Table {id} not found");
            
            manager.SetPlay(timedSeconds);

            await Task.Run(() => manager.SetPlay());
            return Ok($"Table {id} is now playing");
        }

        // PUT: api/tablemanager/{id}/standby
        [HttpPut("{id}/standby")]
        public async Task<IActionResult> SetStandby(Guid id) {
            var manager = _tableManagerService.GetTableManager(id);
            if (manager == null) return NotFound($"Table {id} not found");

            await Task.Run(() => manager.SetStandby());
            return Ok($"Table {id} is now in standby mode");
        }

        // PUT: api/tablemanager/{id}/off
        [HttpPut("{id}/off")]
        public async Task<IActionResult> SetOff(Guid id) {
            var manager = _tableManagerService.GetTableManager(id);
            if (manager == null) return NotFound($"Table {id} not found");

            await Task.Run(() => manager.SetOff());
            return Ok($"Table {id} is now off");
        }

        [HttpGet("all")]
        public IActionResult GetAllTableManagersWithSessions() {
            var tableData = _tableManagerService.GetAllTableManagersWithSessions();
            return Ok(tableData);
        }

    }
}

