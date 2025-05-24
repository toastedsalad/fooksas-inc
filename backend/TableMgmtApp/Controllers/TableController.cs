using Microsoft.AspNetCore.Mvc;
using TableMgmtApp.Persistence;

namespace TableMgmtApp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TablesController : ControllerBase {
        private readonly ITableRepository _repository;
        private readonly TableManagerService _tm;

        public TablesController(ITableRepository repository, TableManagerService tm) {
            _repository = repository;
            _tm = tm;
        }

        // GET: api/tables
        [HttpGet]
        public async Task<IActionResult> GetAllTables() {
            var tables = await _repository.GetAllAsync();
            return Ok(tables);
        }

        // POST: api/tables
        [HttpPost]
        public async Task<IActionResult> AddTable([FromBody] PoolTable table) {
            if (table == null) {
                return BadRequest("Table is null");
            }

            // TODO add result to repo methods.
            await _repository.AddAsync(table);
            await _repository.SaveAsync();
            await _tm.UpdateTableManagers();
            // This is not recommended use. The return should make a URI with GetById
            return CreatedAtAction(nameof(GetAllTables), new { id = table.Id }, table);
        }

        // PUT: api/tables/{tid}/schedule/{sid}
        [HttpPut("{tid}/schedule/{sid}")]
        public async Task<IActionResult> UpdateTableSchedule(Guid tid, Guid sid) {
            var table = await _repository.GetByIdAsync(tid);

            if (table == null) {
                return NotFound($"No table found with ID {tid}");
            }

            // TODO check if the schedule exists before assigning.
            table.ScheduleId = sid;
            await _repository.SaveAsync();

            // 3. Change live memory object
            // Table manager should find a table with this specific ID
            // And set the Table.ScheduleID to this new ID.
            _tm.UpdateSchedule(tid, sid);

            return Ok(table);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(Guid id) {
            var table = await _repository.GetByIdAsync(id);

            if (table == null) {
                return NotFound($"No table found with ID {id}");
            }

            _repository.Remove(table);
            await _repository.SaveAsync();
            await _tm.UpdateTableManagers();

            return NoContent();
        }
    }
}
    
