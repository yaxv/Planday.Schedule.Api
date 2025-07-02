using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Commands;

namespace Planday.Schedule.Infrastructure.Commands;

public class CreateShiftCommand : ICreateShiftCommand
{
    private readonly SqliteConnection _sqliteConnection;
    private const string _dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

    public CreateShiftCommand(SqliteConnection sqliteConnection)
    {
        _sqliteConnection = sqliteConnection;
    }

    public async Task<uint> HandleAsync(CreateShift shift)
    {
        var id = await _sqliteConnection.QuerySingleAsync<uint>(Sql, 
            new { StartDate = shift.Start.ToString(_dateTimeFormat), EndDate = shift.End.ToString(_dateTimeFormat) }
            );

        return id;
    }

    private const string Sql = @"INSERT INTO Shift (Start, End) VALUES (@StartDate, @EndDate) RETURNING Id;";
}