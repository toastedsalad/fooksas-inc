
using Microsoft.EntityFrameworkCore;

namespace TableMgmtApp.Persistence;

public interface IScheduleRepository {
    Task<List<Schedule>> GetAllAsync();
    Task<Schedule?> GetByIdAsync(Guid id);
    Task<List<Schedule>> GetBySchedulesName(string scheduleName);
    Task AddAsync(Schedule schedule);
    Task SaveAsync();
}

public class ScheduleSQLRepository : IScheduleRepository {
    private readonly TableMgmtAppDbContext _context;

    public ScheduleSQLRepository(TableMgmtAppDbContext context) {
        _context = context;
    }

    public async Task<List<Schedule>> GetAllAsync() {
        var scheduleDTOs = await _context.Schedules.ToListAsync();
        return GetSchedulesFromDTOs(scheduleDTOs);
    }


    public async Task<Schedule> GetByIdAsync(Guid id) {
        var scheduleDTO = await _context.Schedules.FindAsync(id);
        return ScheduleService.FromScheduleDTO(scheduleDTO);
    }

    public async Task<List<Schedule>> GetBySchedulesName(string scheduleName) {
        var scheduleDTOs = await _context.Schedules
                                        .Where(s => s.Name == scheduleName)
                                        .ToListAsync();

        return GetSchedulesFromDTOs(scheduleDTOs);
    }

    public async Task AddAsync(Schedule schedule) {
        var scheduleDTO = ScheduleService.ToScheduleDTO(schedule);
        await _context.Schedules.AddAsync(scheduleDTO);
    }

    public async Task SaveAsync() {
        await _context.SaveChangesAsync();
    }

    private static List<Schedule> GetSchedulesFromDTOs(List<ScheduleDTO> scheduleDTOs) {
        var schedules = new List<Schedule>();
        foreach (var dto in scheduleDTOs) {
            Schedule schedule = ScheduleService.FromScheduleDTO(dto);
            schedules.Add(schedule);
        }

        return schedules;
    }
}

