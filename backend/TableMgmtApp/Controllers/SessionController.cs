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
            return Ok(sessions);
        }

        [HttpGet("range")]
        public async Task<IActionResult> GetSessionsInRange([FromQuery] DateTime start, [FromQuery] DateTime end) {
            if (start >= end) {
                return BadRequest("Start time must be earlier than end time.");
            }

            var sessions = await _repository.GetSessionsInRangeAsync(start, end);
            return Ok(sessions);
        }

        [HttpGet("range/csv")]
        public async Task<IActionResult> GetSessionsInRangeCsv([FromQuery] DateTime start, [FromQuery] DateTime end) {
            if (start >= end) {
                return BadRequest("Start time must be earlier than end time.");
            }

            var sessions = await _repository.GetSessionsInRangeAsync(start, end);

            var csv = new StringBuilder();

            // Header row
            csv.AppendLine("TableNumber,PlayerId,StartTime,PlayTime,Price");

            foreach (var s in sessions) {
                // Escape commas and quotes if needed here, or ensure your data doesn't contain them
                csv.AppendLine(
                    $"{s.TableNumber}," +
                    $"{EscapeCsv(s.PlayerId.ToString())}," +
                    $"{s.StartTime:yyyy-MM-dd HH:mm}," +
                    $"{s.PlayTime.ToString()}," +
                    $"{s.Price:F2}"
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
