using Microsoft.AspNetCore.Mvc;
using TableMgmtApp.Persistence;

namespace TableMgmtApp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TableController : ControllerBase {
        private readonly ITableRepository _repository;

        public TableController(ITableRepository repository) {
            _repository = repository;
        }

        // GET: api/table
        [HttpGet]
        public async Task<IActionResult> GetAllTables() {
            var tables = await _repository.GetAllAsync();
            return Ok(tables);
        }

        // POST: api/table
        [HttpPost]
        public async Task<IActionResult> AddTable([FromBody] PoolTable table) {
            if (table == null) {
                return BadRequest("Table is null");
            }

            await _repository.AddAsync(table);
            await _repository.SaveAsync();
            return CreatedAtAction(nameof(GetAllTables), new { id = table.Id }, table);
        }
    }
}

