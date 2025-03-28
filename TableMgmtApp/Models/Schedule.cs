using System.Text.Json;

namespace TableMgmtApp;

public class TimeRate {
    public TimeSpan Start { get; set; }
    public TimeSpan End { 
        get => _end; 
        set {
            if (value.Hours == 0 && value.Minutes == 0 && value.Seconds == 0) {
                _end = new TimeSpan(23, 59, 59);
            }
            else if (value.Minutes == 0 && value.Seconds == 0) {
                 _end = value - TimeSpan.FromSeconds(1);
            }
            else {
                _end = value;
            }
        } 
    }
    public decimal Price { get; set; }

    private TimeSpan _end;

    public TimeRate(TimeSpan start, TimeSpan end, decimal price) {
        Start = start;
        End = end;
        Price = price;
    }

    public bool IsNowInRange(ITimeProvider timeProvider) {
        TimeSpan now = timeProvider.Now.TimeOfDay;
        return now >= Start && now <= End;
    }
}

public record Schedule {
    public Dictionary<DayOfWeek, List<TimeRate>> WeeklyRates { get; set; } = new();
    public decimal DefaultRate { get; set; } = 5.0m;
}

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

    public static string ToJson(Schedule schedule) {
        return JsonSerializer.Serialize(schedule, new JsonSerializerOptions {WriteIndented = true});
    }

    public static Schedule FromJson(string json) {
        return JsonSerializer.Deserialize<Schedule>(json);
    }
}


