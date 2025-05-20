using Microsoft.AspNetCore.Mvc;
using TableMgmtApp.Persistence;

namespace TableMgmtApp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TablesController : ControllerBase {
        private readonly ITableRepository _repository;

        public TablesController(ITableRepository repository) {
            _repository = repository;
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
            return CreatedAtAction(nameof(GetAllTables), new { id = table.Id }, table);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(Guid id) {
            var table = await _repository.GetByIdAsync(id);

            if (table == null) {
                return NotFound($"No table found with ID {id}");
            }

            _repository.Remove(table);
            await _repository.SaveAsync();

            return NoContent();
        }
    }
}

