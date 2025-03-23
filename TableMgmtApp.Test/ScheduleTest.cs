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
        var schedule = new Schedule();
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
        var schedule = new Schedule();
        var mondaySchedule = new List<TimeRate> {
            timeRangeRate
        };

        schedule.WeeklyRates.Add(DayOfWeek.Monday, mondaySchedule);

        var currentRate = ScheduleService.GetCurrentRate(schedule, fakeTimeProvider);

        Assert.That(schedule.WeeklyRates.ContainsKey(DayOfWeek.Monday), Is.True);
        Assert.That(currentRate, Is.EqualTo(10.50m));

        fakeTimeProvider.Now = new DateTime(2025, 03, 17, 12, 30, 0);
        currentRate = ScheduleService.GetCurrentRate(schedule, fakeTimeProvider);
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
                                           new TimeSpan(13, 59, 59),
                                           10.50m);

        var timeRangeRate32 = new TimeRate(new TimeSpan(14, 0, 0),
                                           new TimeSpan(23, 59, 59),
                                           10.50m);

        var fakeTimeProvider = new FakeTimeProvider();
        var schedule = new Schedule();

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

        string expectedJson = """
        {
          "WeeklyRates": {
            "Monday": [
              {
                "Start": "09:00:00",
                "End": "21:59:59",
                "Price": 8.50
              }
            ],
            "Tuesday": [
              {
                "Start": "10:00:00",
                "End": "22:59:59",
                "Price": 12.50
              }
            ],
            "Wednesday": [
              {
                "Start": "09:00:00",
                "End": "13:59:59",
                "Price": 10.50
              },
              {
                "Start": "14:00:00",
                "End": "23:59:59",
                "Price": 10.50
              }
            ]
          },
          "DefaultRate": 5.0
        }
        """;

        string actualJson = ScheduleService.ToJson(schedule);

        Assert.That(actualJson, Is.EqualTo(expectedJson));
    }

    [Test]
    public void ScheduleCanDeserializeFromJson() {
        string actualJson = """
        {
          "WeeklyRates": {
            "Monday": [
              {
                "Start": "09:00:00",
                "End": "22:00:00",
                "Price": 8.50
              }
            ],
            "Tuesday": [
              {
                "Start": "10:00:00",
                "End": "23:00:00",
                "Price": 12.50
              }
            ],
            "Wednesday": [
              {
                "Start": "09:00:00",
                "End": "13:59:59",
                "Price": 10.50
              },
              {
                "Start": "14:00:00",
                "End": "23:59:59",
                "Price": 13.50
              }
            ]
          }
        }
        """;

        var actualSchedule = ScheduleService.FromJson(actualJson);

        Assert.That(actualSchedule.WeeklyRates.Count(), Is.EqualTo(3));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Monday][0].Start, Is.EqualTo(new TimeSpan(9, 0, 0)));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Monday][0].End, Is.EqualTo(new TimeSpan(21, 59, 59)));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Monday][0].Price, Is.EqualTo(8.50m));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Tuesday][0].Start, Is.EqualTo(new TimeSpan(10, 0, 0)));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Tuesday][0].End, Is.EqualTo(new TimeSpan(22, 59, 59)));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Tuesday][0].Price, Is.EqualTo(12.50m));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Wednesday][0].Start, Is.EqualTo(new TimeSpan(9, 0, 0)));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Wednesday][0].End, Is.EqualTo(new TimeSpan(13, 59, 59)));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Wednesday][0].Price, Is.EqualTo(10.50m));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Wednesday][1].Start, Is.EqualTo(new TimeSpan(14, 0, 0)));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Wednesday][1].End, Is.EqualTo(new TimeSpan(23, 59, 59)));
        Assert.That(actualSchedule.WeeklyRates[DayOfWeek.Wednesday][1].Price, Is.EqualTo(13.50m));

        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.Now = new DateTime(2025, 03, 26, 14, 30, 0);
        var currentRate = ScheduleService.GetCurrentRate(actualSchedule, fakeTimeProvider);

        Assert.That(currentRate, Is.EqualTo(13.50m));
    }

    [Test]
    public void WhenUserEntersEqualHourAsEndTimeTimeRateSubtractsOneSecond(){
        var timeRangeRate = new TimeRate(new TimeSpan(9, 0, 0),
                                         new TimeSpan(10, 0, 0),
                                         10.50m);

        Assert.That(timeRangeRate.End, Is.EqualTo(new TimeSpan(9, 59, 59)));
    }

    [Test]
    public void WhenUserEntersEqualHourAsEndTimeTimeRateTheRateAtEqualHourIsNextRate(){
        string scheduleJson = """
        {
          "WeeklyRates": {
            "Wednesday": [
              {
                "Start": "09:00:00",
                "End": "14:00:00",
                "Price": 10.50
              },
              {
                "Start": "14:00:00",
                "End": "23:59:59",
                "Price": 15.50
              }
            ]
          }
        }
        """;

        var schedule = ScheduleService.FromJson(scheduleJson);

        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.Now = new DateTime(2025, 03, 26, 14, 0, 0);

        var currentRate = ScheduleService.GetCurrentRate(schedule, fakeTimeProvider);

        Assert.That(currentRate, Is.EqualTo(15.50));
    }

    [Test]
    public void WhenEndTimeForCurrentDayIsEnteredAs0000ThenItIsSetAs2359(){
        string scheduleJson = """
        {
          "WeeklyRates": {
            "Wednesday": [
              {
                "Start": "09:00:00",
                "End": "00:00:00",
                "Price": 10.50
              }
            ]
          }
        }
        """;

        var schedule = ScheduleService.FromJson(scheduleJson);

        Assert.That(schedule.WeeklyRates[DayOfWeek.Wednesday][0].End, Is.EqualTo(new TimeSpan(23, 59, 59)));
    }
}
