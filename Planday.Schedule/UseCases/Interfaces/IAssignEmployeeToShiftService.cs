namespace Planday.Schedule.UseCases.Interfaces;

public interface IAssignEmployeeToShiftService
{
    Task HandleAsync(long shiftId, long employeeId);
}