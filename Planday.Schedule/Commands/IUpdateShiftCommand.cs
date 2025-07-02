namespace Planday.Schedule.Commands;

public interface IUpdateShiftCommand
{
    Task<int> HandleAsync(long shiftId, long employeeId);
}