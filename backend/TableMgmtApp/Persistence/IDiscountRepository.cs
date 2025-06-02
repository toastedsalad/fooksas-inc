
using Microsoft.EntityFrameworkCore;

namespace TableMgmtApp.Persistence;

public interface IDiscountRepository {
    Task<List<Discount>> GetAllAsync();
    Task<Discount?> GetByIdAsync(Guid id);
    Task<List<Discount>> GetByTypeAsync(string type);
    Task<List<Discount>> GetByNameAsync(string name);
    Task<List<Discount>> SearchAsync(string? type, string? name);
    Task AddAsync(Discount discount);
    void Delete(Discount discount);
    Task SaveAsync();
}

public class DiscountSQLRepository : IDiscountRepository {
    private readonly TableMgmtAppDbContext _context;

    public DiscountSQLRepository(TableMgmtAppDbContext context) {
        _context = context;
    }

    public async Task<List<Discount>> GetAllAsync() {
        return await _context.Discounts.ToListAsync();
    }

    public async Task<List<Discount>> GetByNameAsync(string name) {
        return await _context.Discounts
                             .Where(d => d.Name.Contains(name))
                             .ToListAsync();
    }

    public async Task<List<Discount>> GetByTypeAsync(string type) {
        return await _context.Discounts
                             .Where(p => p.Type.Contains(type))
                             .ToListAsync();
    }

    public async Task<List<Discount>> SearchAsync(string? type, string? name) {
        var query = _context.Discounts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(p => p.Type.ToLower().Contains(type.ToLower()));

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));

        return await query.ToListAsync();
    }

    public async Task<Discount?> GetByIdAsync(Guid id) {
        return await _context.Discounts.FindAsync(id);
    }

    public async Task AddAsync(Discount discount) {
        await _context.Discounts.AddAsync(discount);
    }

    public void Delete(Discount discount) {
        _context.Discounts.Remove(discount);
    }

    public async Task SaveAsync() {
        await _context.SaveChangesAsync();
    }
}


