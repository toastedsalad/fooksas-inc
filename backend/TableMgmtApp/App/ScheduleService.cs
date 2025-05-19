using System.Text.Json;

namespace TableMgmtApp;

public class ScheduleService {
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

    public static string ToJson(Schedule schedule) {
        return JsonSerializer.Serialize(schedule, new JsonSerializerOptions {WriteIndented = true});
    }

    public static Schedule FromJson(string json) {
        return JsonSerializer.Deserialize<Schedule>(json);
    }
}



