namespace TableMgmtApp;

public class TimerFactory {
    public static ICustomTimer CreateTimer(string timerName = "realTimer") {
        ICustomTimer timer;
        switch (timerName) {
            case "fakeTimer":
                timer = new FakeTimer();
                break;
            case "realTimer":
                timer = new RealTimer(1000);
                break;
            default:
                timer = new RealTimer(1000);
                break;
        }
        return timer;
    }
}
