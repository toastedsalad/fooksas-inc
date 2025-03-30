using Microsoft.EntityFrameworkCore;

namespace TableMgmtApp.Persistence;

public interface ITableRepository {
    Task<List<PoolTable>> GetAllAsync();
    Task AddAsync(PoolTable table);
    Task SaveAsync();
}

public class TableSQLRepository : ITableRepository {
    private readonly TableMgmtAppDbContext _context;

    public TableSQLRepository(TableMgmtAppDbContext context) {
        _context = context;
    }

    public async Task<List<PoolTable>> GetAllAsync() {
        return await _context.PoolTables.ToListAsync();
    }

    public async Task AddAsync(PoolTable table) {
        await _context.PoolTables.AddAsync(table);
    }

    public async Task SaveAsync() {
        await _context.SaveChangesAsync();
    }
}


