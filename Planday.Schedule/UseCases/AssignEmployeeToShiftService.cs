using Planday.Schedule.Commands;
using Planday.Schedule.Queries;
using Planday.Schedule.UseCases.Interfaces;

namespace Planday.Schedule.UseCases;

public class AssignEmployeeToShiftService : IAssignEmployeeToShiftService
{
    private readonly IUpdateShiftCommand _updateShiftCommand;
    private readonly IEmployeeExistsQuery _employeeExistsQuery;
    private readonly IGetOverlappingShiftsQuery _getOverlappingShiftsQuery;
    public AssignEmployeeToShiftService(IUpdateShiftCommand updateShiftCommand, 
        IEmployeeExistsQuery employeeExistsQuery, 
        IGetOverlappingShiftsQuery getOverlappingShiftsQuery)
    {
        _updateShiftCommand = updateShiftCommand;
        _employeeExistsQuery = employeeExistsQuery;
        _getOverlappingShiftsQuery = getOverlappingShiftsQuery;
    }

    public async Task HandleAsync(long shiftId, long employeeId)
    {
        if (!await _employeeExistsQuery.QueryAsync(employeeId))
        {
            throw new ApplicationException("The employee does not exist");
        }

        var overlappingShifts = await _getOverlappingShiftsQuery.QueryAsync(shiftId, employeeId);
        if (overlappingShifts != null && overlappingShifts.Any())
        {
            throw new ApplicationException("The requested shift is in conflict with other shifts");
        }

        var affectedRows = await _updateShiftCommand.HandleAsync(shiftId, employeeId);
        if (affectedRows == 0)
        {
            throw new ApplicationException("The shift does not exist");
        }
    }
}