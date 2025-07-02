using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers;
using Planday.Schedule.Infrastructure.Queries;

namespace Planday.Schedule.Infrastructure.Tests.Queries;

public class GetShiftQueryTests
{
    private const string createTableSql = @"CREATE TABLE Employee (
   Id INTEGER PRIMARY KEY AUTOINCREMENT,
   Name text NOT NULL
);";

    private const string createShiftSql = @"CREATE TABLE Shift (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	EmployeeId INTEGER,
	Start TEXT NOT NULL, --SQLite doesn't support DateTime types
	End TEXT NOT NULL, --SQLite doesn't support DateTime types
	FOREIGN KEY(EmployeeId) REFERENCES Employee(Id)
);";

    [Fact]
    public async Task GetShiftQueryAsync()
    {
        // Arrange
        var connectionStringProvider = new ConnectionStringProvider("Data Source=:memory:");

        await using var sqlConnection = new SqliteConnection(connectionStringProvider.GetConnectionString());

        await sqlConnection.OpenAsync();
        await sqlConnection.ExecuteAsync(createTableSql);
        await sqlConnection.ExecuteAsync(createShiftSql);
        await sqlConnection.ExecuteAsync("INSERT INTO Employee (Name) VALUES ('John Doe');");
        await sqlConnection.ExecuteAsync("INSERT INTO Employee (Name) VALUES ('Jane Doe');");
        await sqlConnection.ExecuteAsync("INSERT INTO Shift (EmployeeId, Start, End) VALUES (1, '2022-06-17 12:00:00.000', '2022-06-17 17:00:00.000');");
        await sqlConnection.ExecuteAsync("INSERT INTO Shift (EmployeeId, Start, End) VALUES (2, '2022-06-17 09:00:00.000', '2022-06-17 15:00:00.000');");


        //var shiftId = await sqlConnection.ExecuteAsync("INSERT INTO Shift (EmployeeId, Start, End) VALUES (@EmployeeID, @Start, @End)",
        //    new { EmployeeID = 1, Start = DateTime.Now.AddHours(-7), End = DateTime.Now.AddHours(3) });

        var query = new GetShiftQuery(sqlConnection);

        var shiftId = 1;
        
        // Act
        var shift = await query.QueryAsync(shiftId);

        // Assert
        Assert.NotNull(shift);
        Assert.Equal(shiftId, shift.Id);
        Assert.Equal(1, shift.EmployeeId);
    }
}