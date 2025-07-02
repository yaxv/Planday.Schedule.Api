using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries;

public class EmployeeExistsQuery : IEmployeeExistsQuery
{
    private readonly SqliteConnection _sqliteConnection;

    public EmployeeExistsQuery(SqliteConnection sqliteConnection)
    {
        _sqliteConnection = sqliteConnection;
    }


    public async Task<bool> QueryAsync(long id)
    {
        var employeeIdDto = await _sqliteConnection.QueryFirstOrDefaultAsync<EmployeeIdDto>(Sql, new { Id = id });

        return employeeIdDto != null;
    }

    private const string Sql = @"SELECT Id FROM Employee WHERE Id = @Id ";

    private record EmployeeIdDto(long Id);
}