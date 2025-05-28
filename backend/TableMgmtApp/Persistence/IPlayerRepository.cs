using Microsoft.EntityFrameworkCore;

namespace TableMgmtApp.Persistence;

public interface IPlayerRepository {
    Task<List<Player>> GetAllAsync();
    Task<Player?> GetByIdAsync(Guid id);
    Task<List<Player>> GetByNameAsync(string name);
    Task<List<Player>> GetBySurnameAsync(string surname);
    Task<List<Player>> GetByEmail(string email);
    Task<List<Player>> SearchAsync(string? name, string? surname, string? email);
    Task<List<Player>> GetRecentAsync(int count);
    Task AddAsync(Player player);
    void Delete(Player player);
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

    public async Task<List<Player>> GetByEmail(string email) {
        return await _context.Players
                             .Where(p => p.Email.Contains(email))
                             .ToListAsync();
    }

    public async Task<List<Player>> GetByNameAsync(string name) {
        return await _context.Players
                             .Where(p => p.Surname.Contains(name))
                             .ToListAsync();
    }

    public async Task<List<Player>> GetBySurnameAsync(string surname) {
        return await _context.Players
                             .Where(p => p.Surname.Contains(surname))
                             .ToListAsync();
    }

    public async Task<List<Player>> SearchAsync(string? name, string? surname, string? email) {
        var query = _context.Players.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrWhiteSpace(surname))
            query = query.Where(p => p.Surname.ToLower().Contains(surname.ToLower()));

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(p => p.Email.ToLower().Contains(email.ToLower()));

        return await query.ToListAsync();
    }

    public async Task<Player?> GetByIdAsync(Guid id) {
        return await _context.Players.FindAsync(id);
    }

    public async Task AddAsync(Player player) {
        await _context.Players.AddAsync(player);
    }

    public void Delete(Player player) {
        _context.Players.Remove(player);
    }

    public async Task SaveAsync() {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Player>> GetRecentAsync(int count) {
        return await _context.Players
            .OrderByDescending(p => p.CreatedAt) // or .CreatedAt if you have a timestamp
            .Take(count)
            .ToListAsync();
    }
}


