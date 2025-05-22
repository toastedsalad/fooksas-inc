using Microsoft.EntityFrameworkCore;

namespace TableMgmtApp.Persistence;

public interface IScheduleRepository {
    Task<List<ScheduleDTO>> GetAllAsync();
    Task<ScheduleDTO?> GetByIdAsync(Guid id);
    Task<List<ScheduleDTO>> GetBySchedulesName(string scheduleName);
    Task AddAsync(ScheduleDTO scheduleDTO);
    void Remove(ScheduleDTO scheduleDTO);
    Task SaveAsync();
}

public class ScheduleSQLRepository : IScheduleRepository {
    private readonly TableMgmtAppDbContext _context;

    public ScheduleSQLRepository(TableMgmtAppDbContext context) {
        _context = context;
    }

    public async Task<List<ScheduleDTO>> GetAllAsync() {
        var scheduleDTOs = await _context.Schedules.ToListAsync();
        return scheduleDTOs;
    }

    public async Task<ScheduleDTO?> GetByIdAsync(Guid id) {
        var scheduleDTO = await _context.Schedules.FindAsync(id);
        return scheduleDTO;
    }

    public async Task<List<ScheduleDTO>> GetBySchedulesName(string scheduleName) {
        var scheduleDTOs = await _context.Schedules
                                         .Where(s => s.Name == scheduleName)
                                         .ToListAsync();

        return scheduleDTOs;
    }

    public async Task AddAsync(ScheduleDTO scheduleDTO) {
        await _context.Schedules.AddAsync(scheduleDTO);
    }

    public void Remove(ScheduleDTO scheduleDTO) {
        _context.Schedules.Remove(scheduleDTO);
    }

    public async Task SaveAsync() {
        await _context.SaveChangesAsync();
    }
}

