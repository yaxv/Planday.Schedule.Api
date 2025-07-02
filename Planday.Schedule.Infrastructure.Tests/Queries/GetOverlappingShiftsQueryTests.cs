using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers;
using Planday.Schedule.Infrastructure.Queries;

namespace Planday.Schedule.Infrastructure.Tests.Queries;

public class GetOverlappingShiftsQueryTests
{
    private const string _createTableSql = @"CREATE TABLE Employee (
   Id INTEGER PRIMARY KEY AUTOINCREMENT,
   Name text NOT NULL
);";

    private const string _createShiftSql = @"CREATE TABLE Shift (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	EmployeeId INTEGER,
	Start TEXT NOT NULL, --SQLite doesn't support DateTime types
	End TEXT NOT NULL, --SQLite doesn't support DateTime types
	FOREIGN KEY(EmployeeId) REFERENCES Employee(Id)
);";

    [Theory]
    [InlineData(3, 1)]
    [InlineData(4, 1)]
    [InlineData(5, 1)]
    [InlineData(6, 0)]
    [InlineData(7, 0)]
    public async Task CheckOverlappingShiftsAsync_Tests(long assignShiftId, int expectedOverlapCount)
    {
        // Arrange
        var connectionStringProvider = new ConnectionStringProvider("Data Source=:memory:");

        await using var sqlConnection = new SqliteConnection(connectionStringProvider.GetConnectionString());

        await sqlConnection.OpenAsync();
        await sqlConnection.ExecuteAsync(_createTableSql);
        await sqlConnection.ExecuteAsync(_createShiftSql);
        await sqlConnection.ExecuteAsync("INSERT INTO Employee (Name) VALUES ('John Doe');");
        await sqlConnection.ExecuteAsync("INSERT INTO Employee (Name) VALUES ('Jane Doe');");
        await sqlConnection.ExecuteAsync("INSERT INTO Shift (EmployeeId, Start, End) VALUES (1, '2022-06-17 12:00:00.000', '2022-06-17 17:00:00.000');");
        await sqlConnection.ExecuteAsync("INSERT INTO Shift (EmployeeId, Start, End) VALUES (2, '2022-06-17 09:00:00.000', '2022-06-17 15:00:00.000');");
        await sqlConnection.ExecuteAsync("INSERT INTO Shift (Start, End) VALUES ('2022-06-17 13:00:00.000', '2022-06-17 14:00:00.000');");
        await sqlConnection.ExecuteAsync("INSERT INTO Shift (Start, End) VALUES ('2022-06-17 15:00:00.000', '2022-06-17 19:00:00.000');");
        await sqlConnection.ExecuteAsync("INSERT INTO Shift (Start, End) VALUES ('2022-06-17 07:00:00.000', '2022-06-17 14:00:00.000');");
        await sqlConnection.ExecuteAsync("INSERT INTO Shift (Start, End) VALUES ('2022-06-17 07:00:00.000', '2022-06-17 11:00:00.000');");
        await sqlConnection.ExecuteAsync("INSERT INTO Shift (Start, End) VALUES ('2022-06-17 18:00:00.000', '2022-06-17 20:00:00.000');");

        // Act
        var validator = new GetOverlappingShiftsQuery(sqlConnection);

        var shifts = await validator.QueryAsync(assignShiftId, 1);

        // Assert
        Assert.Equal(expectedOverlapCount, shifts.Count());
    }
}