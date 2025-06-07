using Microsoft.EntityFrameworkCore;

namespace TableMgmtApp.Persistence;

public interface IPlaySessionRepository {
    Task<List<PlaySession>> GetAllAsync();
    Task<PlaySession?> GetByIdAsync(Guid id);
    Task<List<PlaySession>> GetByTableNumber(int tableNumber);
    Task AttachAndUpdate(PlaySession playSession);
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
        return await _context.PlaySessions
                     .Include(p => p.Player)
                     .Include(d => d.Discount)
                     .ToListAsync();
    }

    public async Task<PlaySession?> GetByIdAsync(Guid id) {
        return await _context.PlaySessions.Include(p => p.Player)
                                          .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<PlaySession>> GetByTableNumber(int tableNumber) {
        return await _context.PlaySessions
                             .Include(p => p.Player)
                             .Include(d => d.Discount)
                             .Where(p => p.TableNumber == tableNumber)
                             .ToListAsync();
    }

    public async Task<IEnumerable<PlaySession>> GetSessionsInRangeAsync(DateTime start, DateTime end) {
        return await _context.PlaySessions
                             .Include(p => p.Player)
                             .Include(d => d.Discount)
                             .Where(s => s.StartTime >= start && s.StartTime <= end)
                             .ToListAsync();
    }

    public async Task AttachAndUpdate(PlaySession playsession) {
        _context.Attach(playsession);
        _context.Entry(playsession).State = EntityState.Added;

        if (playsession.Discount != null) {
            _context.Attach(playsession.Discount);
            _context.Entry(playsession.Discount).State = EntityState.Unchanged;
        }

        if (playsession.Player != null) {
            _context.Attach(playsession.Player);
            _context.Entry(playsession.Player).State = EntityState.Unchanged;
        }

        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(PlaySession playSession) {
        await _context.PlaySessions.AddAsync(playSession);
    }

    public async Task SaveAsync() {
        await _context.SaveChangesAsync();
    }
}


