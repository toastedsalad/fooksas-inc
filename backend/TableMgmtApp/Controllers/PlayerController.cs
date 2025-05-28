
using Microsoft.AspNetCore.Mvc;
using TableMgmtApp.Persistence;

namespace TableMgmtApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase {
    private readonly IPlayerRepository _repository;

    public PlayerController(IPlayerRepository repository) {
        _repository = repository;
    }

    // GET: api/player/all
    [HttpGet("all")]
    public async Task<IActionResult> GetRecentPlayers() {
        var players = await _repository.GetRecentAsync(10);
        return Ok(players);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayerById(Guid id) {
        var players = await _repository.GetByIdAsync(id);
        return Ok(players);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
            [FromQuery] string? name,
            [FromQuery] string? surname,
            [FromQuery] string? email) {
        var players = await _repository.SearchAsync(name, surname, email);
        return Ok(players);
    }

    [HttpPost]
    public async Task<IActionResult> AddPlayer([FromBody] Player player) {
        if (player == null) {
            return BadRequest("Player is null");
        }

        await _repository.AddAsync(player);
        await _repository.SaveAsync();

        return CreatedAtAction(nameof(GetPlayerById), new {id = player.Id }, player);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayer(Guid id, [FromBody] Player updatedPlayer)
    {
        if (id != updatedPlayer.Id)
            return BadRequest("Player ID mismatch");

        var existingPlayer = await _repository.GetByIdAsync(id);
        if (existingPlayer == null)
            return NotFound();

        existingPlayer.Name = updatedPlayer.Name;
        existingPlayer.Surname = updatedPlayer.Surname;
        existingPlayer.Email = updatedPlayer.Email;
        existingPlayer.Discount = updatedPlayer.Discount;

        await _repository.SaveAsync();

        return NoContent(); // 204 success
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) {
        var player = await _repository.GetByIdAsync(id);

        if (player == null) {
            return NotFound($"No player found with ID {id}");
        }

        _repository.Delete(player);
        await _repository.SaveAsync();
        return NoContent();
    }
}













