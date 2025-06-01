// Session
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TableMgmtApp.Persistence;

namespace TableMgmtApp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase {
        private readonly IPlaySessionRepository _repository;

        public SessionsController(IPlaySessionRepository repository) {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSessions() {
            var sessions = await _repository.GetAllAsync();

            var sessionDtos = sessions.Select(s => new PlaySessionDTO {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    PlayTime = s.PlayTime,
                    Price = s.Price,
                    TableName = s.TableName,
                    TableNumber = s.TableNumber,
                    PlayerId = s.PlayerId ?? Guid.Empty,
                    PlayerName = s.Player?.Name,
                    PlayerSurname = s.Player?.Surname,
                    DiscountId = s.DiscountId ?? Guid.Empty,
                    DiscountType = s.Discount?.Type,
                    DiscountName = s.Discount?.Name,
                    DiscountRate = s.Discount?.Rate,
                    });

            return Ok(sessionDtos);
        }

        [HttpGet("range")]
        public async Task<IActionResult> GetSessionsInRange([FromQuery] DateTime start, [FromQuery] DateTime end) {
            if (start >= end) {
                return BadRequest("Start time must be earlier than end time.");
            }

            var sessions = await _repository.GetSessionsInRangeAsync(start, end);

            var sessionDtos = sessions.Select(s => new PlaySessionDTO {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    PlayTime = s.PlayTime,
                    Price = s.Price,
                    TableName = s.TableName,
                    TableNumber = s.TableNumber,
                    PlayerId = s.PlayerId ?? Guid.Empty,
                    PlayerName = s.Player?.Name,
                    PlayerSurname = s.Player?.Surname,
                    DiscountId = s.DiscountId ?? Guid.Empty,
                    DiscountType = s.Discount?.Type,
                    DiscountName = s.Discount?.Name,
                    DiscountRate = s.Discount?.Rate,
                    });

            return Ok(sessionDtos);
        }

        [HttpGet("range/csv")]
        public async Task<IActionResult> GetSessionsInRangeCsv([FromQuery] DateTime start, [FromQuery] DateTime end) {
            if (start >= end) {
                return BadRequest("Start time must be earlier than end time.");
            }

            var sessions = await _repository.GetSessionsInRangeAsync(start, end);

            var csv = new StringBuilder();

            // Header row
            csv.AppendLine("TableName,TableNumber,PlayerId,StartTime,PlayTime,Price,PlayerName,PlayeSurname,DiscountType,DiscountName,DiscountRate");

            foreach (var s in sessions) {
                // Escape commas and quotes if needed here, or ensure your data doesn't contain them
                csv.AppendLine(
                    $"{s.TableName}," +
                    $"{s.TableNumber}," +
                    $"{EscapeCsv(s.PlayerId.ToString())}," +
                    $"{s.StartTime:yyyy-MM-dd HH:mm}," +
                    $"{s.PlayTime.ToString()}," +
                    $"{s.Price:F2}," +
                    $"{s.Player?.Name}," +
                    $"{s.Player?.Surname}," +
                    $"{s.Discount?.Type}," +
                    $"{s.Discount?.Name}," +
                    $"{s.Discount?.Rate}"
                );
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "sessions.csv");
        }

        private string EscapeCsv(string input) {
            if (input.Contains(",") || input.Contains("\"")) {
                return $"\"{input.Replace("\"", "\"\"")}\"";
            }
            return input;
        }

    }
}
