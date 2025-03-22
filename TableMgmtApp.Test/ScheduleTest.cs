namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class ScheduleTest {
    [Test]
    public void DailyRateHasStartEndAndPrice(){
        var timeRangeRate = new TimeRate(new TimeSpan(9, 0, 0),
                                         new TimeSpan(10, 0, 0),
                                         10.50m);

        Assert.That(timeRangeRate.Price, Is.EqualTo(10.50m));
    }

    [Test]
    public void DailyRateCanTellIfNowIsInRange(){
        var timeRangeRate = new TimeRate(new TimeSpan(9, 0, 0),
                                         new TimeSpan(10, 0, 0),
                                         10.50m);

        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.Now = new DateTime(2025, 12, 28, 9, 30, 0);

        var isInRate = timeRangeRate.IsNowInRange(fakeTimeProvider);

        Assert.That(isInRate, Is.True);
    }

    [Test]
    public void ScheduleHoldsADictOfDailyRatesForWeekdays() {
        var timeRangeRate = new TimeRate(new TimeSpan(9, 0, 0),
                                         new TimeSpan(10, 0, 0),
                                         10.50m);
        
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.Now = new DateTime(2025, 12, 28, 9, 30, 0);
        var schedule = new Schedule(fakeTimeProvider);
        var mondaySchedule = new List<TimeRate> {
            timeRangeRate
        };

        schedule.WeeklyRates.Add(DayOfWeek.Monday, mondaySchedule);

        Assert.That(schedule.WeeklyRates.ContainsKey(DayOfWeek.Monday), Is.True);
    }

    [Test]
    public void ScheduleCanReturnTheRateOfNow() {
        var timeRangeRate = new TimeRate(new TimeSpan(9, 0, 0),
                                         new TimeSpan(10, 0, 0),
                                         10.50m);
        
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.Now = new DateTime(2025, 03, 17, 9, 30, 0);
        var schedule = new Schedule(fakeTimeProvider);
        var mondaySchedule = new List<TimeRate> {
            timeRangeRate
        };

        schedule.WeeklyRates.Add(DayOfWeek.Monday, mondaySchedule);

        var currentRate = schedule.GetCurrentRate();

        Assert.That(schedule.WeeklyRates.ContainsKey(DayOfWeek.Monday), Is.True);
        Assert.That(currentRate, Is.EqualTo(10.50m));

        fakeTimeProvider.Now = new DateTime(2025, 03, 17, 12, 30, 0);
        currentRate = schedule.GetCurrentRate();
        Assert.That(currentRate, Is.EqualTo(5.0m));
    }

    [Test]
    public void ScheduleCanSerializeIntoJson() {
        var timeRangeRate11 = new TimeRate(new TimeSpan(9, 0, 0),
                                           new TimeSpan(22, 0, 0),
                                           8.50m);
        
        var timeRangeRate21 = new TimeRate(new TimeSpan(10, 0, 0),
                                           new TimeSpan(23, 0, 0),
                                           12.50m);

        var timeRangeRate31 = new TimeRate(new TimeSpan(9, 0, 0),
                                           new TimeSpan(14, 0, 0),
                                           10.50m);

        var timeRangeRate32 = new TimeRate(new TimeSpan(14, 0, 1),
                                           new TimeSpan(23, 59, 59),
                                           10.50m);

        var fakeTimeProvider = new FakeTimeProvider();
        var schedule = new Schedule(fakeTimeProvider);

        var mondaySchedule = new List<TimeRate> {
            timeRangeRate11
        };

        var tuesdaySchedule = new List<TimeRate> {
            timeRangeRate21
        };

        var wednesdaySchedule = new List<TimeRate> {
            timeRangeRate31,
            timeRangeRate32
        };

        schedule.WeeklyRates.Add(DayOfWeek.Monday, mondaySchedule);
        schedule.WeeklyRates.Add(DayOfWeek.Tuesday, tuesdaySchedule);
        schedule.WeeklyRates.Add(DayOfWeek.Wednesday, wednesdaySchedule);

        var currentRate = schedule.GetCurrentRate();

        Assert.That(schedule.WeeklyRates.ContainsKey(DayOfWeek.Monday), Is.True);
        Assert.That(currentRate, Is.EqualTo(10.50m));
    }
}

