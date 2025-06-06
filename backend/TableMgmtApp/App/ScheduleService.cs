using Newtonsoft.Json;
using TableMgmtApp.Persistence;

namespace TableMgmtApp;

public interface IScheduleService {
    Task Add(ScheduleDTO schedule);
    Task Delete(ScheduleDTO scheduleDTO);
    Task<List<ScheduleDTO>> GetAllScheduleDTOs();
    Task<List<Schedule>> GetAllSchedules();
    Task<ScheduleDTO?> GetById(Guid id);
}

public class ScheduleService : IScheduleService {
    private readonly IScheduleRepository _repo;

    public ScheduleService(IScheduleRepository repo) {
        _repo = repo;
    }

    public async Task<ScheduleDTO?> GetById(Guid id) {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<List<ScheduleDTO>> GetAllScheduleDTOs() {
        return await _repo.GetAllAsync();
    }

    public async Task Add(ScheduleDTO schedule) {
        if (schedule.Id == null || schedule.Id == Guid.Empty) {
            schedule.Id = Guid.NewGuid();
        }

        var existing = await GetById(schedule.Id);

        if (existing == null) {
            await _repo.AddAsync(schedule);
        }
        else {
            existing.Name = schedule.Name;
            existing.WeeklyRates = schedule.WeeklyRates;
            existing.DefaultRate = schedule.DefaultRate;
        }

        await _repo.SaveAsync();
    }

    public async Task Delete(ScheduleDTO scheduleDTO) {
        _repo.Remove(scheduleDTO);
        await _repo.SaveAsync();
    }

    public async Task<List<Schedule>> GetAllSchedules() {
        var scheduleDTOs = await _repo.GetAllAsync();
        return GetSchedulesFromDTOs(scheduleDTOs);
    }

    public static decimal GetCurrentRate(Schedule schedule, ITimeProvider timeProvider) {
        var today = timeProvider.Now.DayOfWeek;

        if (schedule.WeeklyRates.TryGetValue(today, out var timeRates)) {
            foreach (var rate in timeRates) {
                if (rate.IsNowInRange(timeProvider)) {
                    return rate.Price;
                }
            }
        }

        return schedule.DefaultRate;
    }

    public static ScheduleDTO ToScheduleDTO(Schedule schedule) {
        var scheduleDTO = new ScheduleDTO();
        scheduleDTO.Id = schedule.Id;
        scheduleDTO.Name = schedule.Name;
        scheduleDTO.WeeklyRates = JsonConvert.SerializeObject(schedule.WeeklyRates);
        scheduleDTO.DefaultRate = schedule.DefaultRate;

        return scheduleDTO;
    }

    public static Schedule FromScheduleDTO(ScheduleDTO scheduleDTO) {
        var schedule = new Schedule();
        schedule.Id = scheduleDTO.Id;
        schedule.Name = scheduleDTO.Name;
        schedule.WeeklyRates = JsonConvert.DeserializeObject<Dictionary<DayOfWeek, List<TimeRate>>>(scheduleDTO.WeeklyRates);
        schedule.DefaultRate = scheduleDTO.DefaultRate;

        return schedule;
    }

    private static List<Schedule> GetSchedulesFromDTOs(List<ScheduleDTO> scheduleDTOs) {
        var schedules = new List<Schedule>();
        foreach (var dto in scheduleDTOs) {
            Schedule schedule = ScheduleService.FromScheduleDTO(dto);
            schedules.Add(schedule);
        }

        return schedules;
    }

    public static string ToJson(Schedule schedule) {
        return JsonConvert.SerializeObject(schedule, Formatting.Indented);
    }

    public static Schedule FromJson(string json) {
        return JsonConvert.DeserializeObject<Schedule>(json);
    }
}

