using Microsoft.EntityFrameworkCore;

namespace TableMgmtApp.Persistence;

public interface IPlaySessionRepository {
    Task<List<PlaySession>> GetAllAsync();
    Task<PlaySession?> GetByIdAsync(Guid id);
    Task<List<PlaySession>> GetByTableNumber(int tableNumber);
    Task AddAsync(PlaySession playSession);
    Task SaveAsync();
    Task<IEnumerable<PlaySession>> GetSessionsInRangeAsync(DateTime start, DateTime end);
}

public class PlaySessionSQLRepository : IPlaySessionRepository {
    private readonly TableMgmtAppDbContext _context;

    public PlaySessionSQLRepository(TableMgmtAppDbContext context) {
        _context = context;
    }

    public async Task<List<PlaySession>> GetAllAsync() {
        return await _context.PlaySessions.ToListAsync();
    }

    public async Task<PlaySession?> GetByIdAsync(Guid id) {
        return await _context.PlaySessions.FindAsync(id);
    }

    public async Task<List<PlaySession>> GetByTableNumber(int tableNumber) {
        return await _context.PlaySessions
                             .Where(p => p.TableNumber == tableNumber)
                             .ToListAsync();
    }

    public async Task<IEnumerable<PlaySession>> GetSessionsInRangeAsync(DateTime start, DateTime end) {
        return await _context.PlaySessions
            .Where(s => s.StartTime >= start && s.StartTime <= end)
            .ToListAsync();
    }

    public async Task AddAsync(PlaySession playSession) {
        await _context.PlaySessions.AddAsync(playSession);
    }

    public async Task SaveAsync() {
        await _context.SaveChangesAsync();
    }

}


