// Session
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
    }
}
