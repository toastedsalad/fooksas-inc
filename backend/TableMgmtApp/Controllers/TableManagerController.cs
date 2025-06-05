using Microsoft.AspNetCore.Mvc;
using TableMgmtApp.Persistence;

namespace TableMgmtApp; 

[ApiController]
[Route("api/[controller]")]
public class TableManagerController : ControllerBase {
    private readonly TableManagerService _tableManagerService;
    private readonly IDiscountRepository _discountRepo;

    public TableManagerController(TableManagerService tableManagerService,
                                  IDiscountRepository discountRepository) {
        _tableManagerService = tableManagerService;
        _discountRepo = discountRepository;
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

    // Need a new endpoint here that takes table id and DiscountDTO and updates
    // the in memmory session with that discount.
    [HttpPut("{tableId}/session/discount/{discountId}/update")]
    public async Task<IActionResult> UpdateSessionDiscount(Guid tableId, Guid discountId) {

        Console.WriteLine($"Session: {tableId}");
        Console.WriteLine($"Discount: {discountId}");

        var session = _tableManagerService.GetPlaySessionForTableId(tableId);
        if (session == null) {
            return NotFound();
        }

        var discountEntity = await _discountRepo.GetByIdAsync(discountId);

        if (discountEntity == null) {
            return NotFound();
        }

        session.Discount = discountEntity;
        session.DiscountId = discountEntity.Id;

        return NoContent(); 
    }
}

