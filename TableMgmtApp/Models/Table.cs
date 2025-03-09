namespace TableMgmtApp;

public enum TableState {
    PlayOn,
    Off,
    Paused,
    Standby
}

public class Table {
    public int Id { get; private set; }
    public TableState State { get; private set; }
    public int PauseTimer { get; private set; }

    public Table(int id, int pauseTimer = 1) {
        Id = id;
        PauseTimer = pauseTimer;
    }

    public void SetState(TableState newState) {
        if (State == TableState.Off) {
            State = newState;
        }

        if (State == TableState.PlayOn && newState == TableState.Off) {
            State = TableState.Paused;
            StartPauseTimer();
        }

        if (State == TableState.Paused && newState == TableState.PlayOn) {
            State = TableState.PlayOn;
        }
    }

    private async void StartPauseTimer() {
        await Task.Delay(PauseTimer * 1000);
        if (State == TableState.Paused) {
            State = TableState.Standby;
        }
    }
}


