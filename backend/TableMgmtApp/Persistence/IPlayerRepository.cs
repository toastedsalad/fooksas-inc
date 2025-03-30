using Microsoft.EntityFrameworkCore;

namespace TableMgmtApp.Persistence;

public interface IPlayerRepository {
    Task<List<Player>> GetAllAsync();
    Task<Player?> GetByIdAsync(Guid id);
    Task<List<Player>> GetByNameAsync(string name);
    Task<List<Player>> GetBySurnameAsync(string surname);
    Task AddAsync(Player player);
    Task SaveAsync();
}

public class PlayerSQLRepository : IPlayerRepository {
    private readonly TableMgmtAppDbContext _context;

    public PlayerSQLRepository(TableMgmtAppDbContext context) {
        _context = context;
    }

    public async Task<List<Player>> GetAllAsync() {
        return await _context.Players.ToListAsync();
    }

    public async Task<List<Player>> GetByNameAsync(string name) {
        return await _context.Players
                             .Where(p => p.Surname == name)
                             .ToListAsync();
    }

    public async Task<List<Player>> GetBySurnameAsync(string surname) {
        return await _context.Players
                             .Where(p => p.Surname == surname)
                             .ToListAsync();
    }

    public async Task<Player?> GetByIdAsync(Guid id) {
        return await _context.Players.FindAsync(id);
    }

    public async Task AddAsync(Player player) {
        await _context.Players.AddAsync(player);
    }

    public async Task SaveAsync() {
        await _context.SaveChangesAsync();
    }
}


