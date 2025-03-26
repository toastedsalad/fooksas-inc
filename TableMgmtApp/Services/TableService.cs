namespace TableMgmtApp;

public enum SwitchState {
    On,
    Off
}

public class TableService {
    public List<Table> Tables { get; set; } = default!;

    public Result<Table> GetTable(int id) {
        try {
            var table = Tables.Find(table => table.Id == id);
            if (table == null)
                return Result<Table>.Fail($"Could not find table with id {id}");

            return Result<Table>.Ok(table);
        }
        catch (Exception ex) {
            return Result<Table>.Fail($"An error occurred: {ex.Message}");
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

