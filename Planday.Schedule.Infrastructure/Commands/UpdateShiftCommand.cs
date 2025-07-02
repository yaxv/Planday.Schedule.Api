using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Commands;

namespace Planday.Schedule.Infrastructure.Commands;

public class UpdateShiftCommand : IUpdateShiftCommand
{
    private readonly SqliteConnection _sqliteConnection;

    public UpdateShiftCommand(SqliteConnection sqliteConnection)
    {
        _sqliteConnection = sqliteConnection;
    }

    public async Task<int> HandleAsync(long shiftId, long employeeId)
    {
        var affectedRows = await _sqliteConnection.ExecuteAsync(Sql, new { EmployeeId = employeeId, ID = shiftId });
        return affectedRows;
    }

    private const string Sql = @"UPDATE Shift SET EmployeeId = @EmployeeId WHERE ID = @ID;";
}