using Microsoft.EntityFrameworkCore;

namespace TableMgmtApp.Persistence;

public interface ITableRepository {
    Task<List<Table>> GetAllAsync();
    Task AddAsync(Table table);
    Task SaveAsync();
}

public class TableSQLRepository : ITableRepository {
    private readonly TableMgmtAppDbContext _context;

    public TableSQLRepository(TableMgmtAppDbContext context) {
        _context = context;
    }

    public async Task<List<Table>> GetAllAsync() {
        return await _context.Tables.ToListAsync();
    }

    public async Task AddAsync(Table table) {
        await _context.Tables.AddAsync(table);
    }

    public async Task SaveAsync() {
        await _context.SaveChangesAsync();
    }
}


