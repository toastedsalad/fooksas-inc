
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

public class Schedule {
    public Dictionary<DayOfWeek, List<TimeRate>> WeeklyRates { get; set; } = new();
    public decimal DefaultRate = 5.0m;

    private ITimeProvider _timeProvider;

    public Schedule (ITimeProvider timeprovider) {
        _timeProvider = timeprovider;
    }

    public decimal GetCurrentRate() {
        var today = _timeProvider.Now.DayOfWeek;

        if (WeeklyRates.TryGetValue(today, out var timeRates)) {
            foreach (var rate in timeRates) {
                if (rate.IsNowInRange(_timeProvider)) {
                    return rate.Price;
                }
            }
        }

        return DefaultRate;
    }
}


