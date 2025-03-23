using System.Text.Json;
using System.Text.Json.Serialization;

namespace TableMgmtApp;

public class TimeRate {
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public decimal Price { get; set; }

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

// Make Schedule a record.
// Have Schedule service to get current rate and serialize.
public record Schedule {
    public Dictionary<DayOfWeek, List<TimeRate>> WeeklyRates { get; set; } = new();
    public decimal DefaultRate { get; set; } = 5.0m;

    public Schedule () {}
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


