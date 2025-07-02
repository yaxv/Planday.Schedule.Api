namespace Planday.Schedule.Queries;

public interface IGetOverlappingShiftsQuery
{
    Task<IEnumerable<Shift>> QueryAsync(long assignShiftId, long employeeId);
}