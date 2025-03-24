namespace TableMgmtApp;

public class TableService {
    // Table service has a list of tables.
    // Table service can interact with switches...
    // It can take a virtual switch or a different implementation.
    // It can also take in API calls.
    //
    //
    // So for inputs
    // Table service can select a table from a list by Id
    // It can set state for that specific table
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
};
