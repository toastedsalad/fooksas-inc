namespace TableMgmtApp;

public enum SwitchState {
    On,
    Off
}

public class TableService {
    public List<TableManager> TableManagers { get; set; } = default!;

    public Result<TableManager> GetTable(int number) {
        try {
            var table = TableManagers.Find(table => table.TableNumber == number);
            if (table == null)
                return Result<TableManager>.Fail($"Could not find table with id {number}");

            return Result<TableManager>.Ok(table);
        }
        catch (Exception ex) {
            return Result<TableManager>.Fail($"An error occurred: {ex.Message}");
        }
    }

    // TODO: perhaps Switch should be part of table? Idk.
    public void SwitchTable(int id, ISwitch customSwitch, SwitchState switchState) {
        var tableResult = GetTable(id);
        if (tableResult.IsSuccess) {
            if (switchState == SwitchState.On) {
                tableResult.Value!.SetStateBySwitch(TableState.Play);
                customSwitch.SetSwitch(switchState);
            } else if (switchState == SwitchState.Off) {
                tableResult.Value!.SetStateBySwitch(TableState.Off);
                customSwitch.SetSwitch(switchState);
            }
        }
        // TODO: What do we do on failure?
        return;
    } 
};

