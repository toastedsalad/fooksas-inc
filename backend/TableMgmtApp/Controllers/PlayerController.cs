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
        var playerDtos = players.Select(p => new PlayerDTO {
            Id = p.Id,
            CreatedAt = p.CreatedAt,
            Name = p.Name,
            Surname = p.Surname,
            Email = p.Email,
            DiscountType = p.Discount?.Type,
            DiscountName = p.Discount?.Name,
            DiscountRate = p.Discount?.Rate,
        });
        return Ok(playerDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayerById(Guid id) {
        var player = await _repository.GetByIdAsync(id);
        var playerDto = new PlayerDTO {
            Id = player.Id,
            CreatedAt = player.CreatedAt,
            Name = player.Name,
            Surname = player.Surname,
            Email = player.Email,
            DiscountType = player.Discount?.Type,
            DiscountName = player.Discount?.Name,
            DiscountRate = player.Discount?.Rate,
        };
        return Ok(playerDto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
            [FromQuery] string? name,
            [FromQuery] string? surname,
            [FromQuery] string? email) {

        var players = await _repository.SearchAsync(name, surname, email);

        var playerDtos = players.Select(s => new PlayerDTO { 
            Id = s.Id,
            CreatedAt = s.CreatedAt,
            Name = s.Name,
            Surname = s.Surname,
            Email = s.Email,
            DiscountType = s.Discount?.Type,
            DiscountName = s.Discount?.Name,
            DiscountRate = s.Discount?.Rate,
        });

        return Ok(playerDtos);
    }

    [HttpPost]
    public async Task<IActionResult> AddPlayer([FromBody] PlayerDTO playerDto) {
        if (playerDto == null) {
            return BadRequest("Player is null");
        }

        var player = new Player(playerDto.Name, playerDto.Surname, playerDto.Email);

        await _repository.AddAsync(player);
        await _repository.SaveAsync();

        return CreatedAtAction(nameof(GetPlayerById), new {id = player.Id }, player);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayer(Guid id, [FromBody] PlayerDTO updatedPlayer) {
        if (id != updatedPlayer.Id)
            return BadRequest("Player ID mismatch");

        var existingPlayer = await _repository.GetByIdAsync(id);
        if (existingPlayer == null)
            return NotFound();

        existingPlayer.Name = updatedPlayer.Name;
        existingPlayer.Surname = updatedPlayer.Surname;
        existingPlayer.Email = updatedPlayer.Email;
        existingPlayer.DiscountId = updatedPlayer.DiscountId;

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


