using System.Text.Json;
using TableMgmtApp.Persistence;

namespace TableMgmtApp;

public class ScheduleService {
    private readonly IScheduleRepository _repo;

    public ScheduleService(IScheduleRepository repo) {
        _repo = repo;
    }

    public async Task<List<ScheduleDTO>> GetAllScheduleDTOs() {
        return await _repo.GetAllAsync();
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
        scheduleDTO.WeeklyRates = JsonSerializer.Serialize(schedule.WeeklyRates);
        scheduleDTO.DefaultRate = schedule.DefaultRate;

        return scheduleDTO;
    }

    public static Schedule FromScheduleDTO(ScheduleDTO scheduleDTO) {
        var schedule = new Schedule();
        schedule.Id = scheduleDTO.Id;
        schedule.Name = scheduleDTO.Name;
        schedule.WeeklyRates = JsonSerializer.Deserialize<Dictionary<DayOfWeek, List<TimeRate>>>(scheduleDTO.WeeklyRates);
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
        return JsonSerializer.Serialize(schedule, new JsonSerializerOptions {WriteIndented = true});
    }

    public static Schedule FromJson(string json) {
        return JsonSerializer.Deserialize<Schedule>(json);
    }
}

