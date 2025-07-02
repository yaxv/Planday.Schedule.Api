using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries;

public class GetOverlappingShiftsQuery : IGetOverlappingShiftsQuery
{
    private readonly SqliteConnection _sqliteConnection;

    public GetOverlappingShiftsQuery(SqliteConnection sqliteConnection)
    {
        _sqliteConnection = sqliteConnection;
    }

    public async Task<IEnumerable<Shift>> QueryAsync(long assignShiftId, long employeeId)
    {
        var shiftDtos = await _sqliteConnection.QueryAsync<ShiftDto>(Sql, new { AssignShiftId = assignShiftId, EmployeeId = employeeId });

        var shifts = shiftDtos.Select(x =>
            new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

        return shifts.ToList();
    }

    private const string Sql =
        @"SELECT AssignedShifts.Id, AssignedShifts.EmployeeId, AssignedShifts.Start, AssignedShifts.End
FROM Shift AS AssignedShifts
CROSS JOIN Shift AS RequestAssignShift
WHERE 
	RequestAssignShift.Id = @AssignShiftId -- shift requested to be assigned to
	AND RequestAssignShift.Id != AssignedShifts.Id -- leave out the requested shift
	AND AssignedShifts.EmployeeId = @EmployeeId -- only compare shifts assigned to employee
AND
(
  (AssignedShifts.Start > RequestAssignShift.Start AND AssignedShifts.Start < RequestAssignShift.End) OR
  (AssignedShifts.Start < RequestAssignShift.Start AND AssignedShifts.End > RequestAssignShift.End) OR 
  (AssignedShifts.End > RequestAssignShift.Start AND AssignedShifts.End < RequestAssignShift.End)
);";

    private record ShiftDto(long Id, long? EmployeeId, string Start, string End);
}