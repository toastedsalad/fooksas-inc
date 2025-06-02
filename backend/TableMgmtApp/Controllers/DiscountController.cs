using Microsoft.AspNetCore.Mvc;
using TableMgmtApp.Persistence;

namespace TableMgmtApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountController : ControllerBase {
    private readonly IDiscountRepository _repository;

    public DiscountController(IDiscountRepository repository) {
        _repository = repository;
    }

    // GET: api/discount/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAllDiscounts() {
        var discounts = await _repository.GetAllAsync();
        return Ok(discounts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDiscountById(Guid id) {
        var discount = await _repository.GetByIdAsync(id);

        return Ok(discount);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
            [FromQuery] string? type,
            [FromQuery] string? name) {

        var discounts = await _repository.SearchAsync(type, name);

        return Ok(discounts);
    }

    [HttpPost]
    public async Task<IActionResult> AddDiscount([FromBody] Discount discount) {
        if (discount == null) {
            return BadRequest("Discount is null");
        }

        await _repository.AddAsync(discount);
        await _repository.SaveAsync();

        return CreatedAtAction(nameof(GetDiscountById), new {id = discount.Id }, discount);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiscount(Guid id, [FromBody] Discount updatedDiscount) {
        if (id != updatedDiscount.Id)
            return BadRequest("Discount ID mismatch");

        var existingDiscount = await _repository.GetByIdAsync(id);
        if (existingDiscount == null)
            return NotFound();

        existingDiscount.Type = updatedDiscount.Type;
        existingDiscount.Name = updatedDiscount.Name;
        existingDiscount.Rate = updatedDiscount.Rate;

        await _repository.SaveAsync();

        return NoContent(); // 204 success
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) {
        var discount = await _repository.GetByIdAsync(id);

        if (discount == null) {
            return NotFound($"No discount found with ID {id}");
        }

        _repository.Delete(discount);
        await _repository.SaveAsync();
        return NoContent();
    }
}


