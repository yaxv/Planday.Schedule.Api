using Planday.Schedule.Providers;
using Planday.Schedule.Queries;
using Planday.Schedule.UseCases.Interfaces;

namespace Planday.Schedule.UseCases;

public class GetShiftService : IGetShiftService
{
    private readonly IGetShiftQuery _getShiftQuery;
    private readonly IPlandayEmployeeProvider _plandayEmployeeProvider;

    public GetShiftService(IGetShiftQuery getShiftQuery, IPlandayEmployeeProvider plandayEmployeeProvider)
    {
        _getShiftQuery = getShiftQuery;
        _plandayEmployeeProvider = plandayEmployeeProvider;
    }

    public async Task<ShiftEmployee> GetShiftByIdAsync(long id)
    {
        var shift = await _getShiftQuery.QueryAsync(id);
        if (shift == null)
            return null;

        if (shift.EmployeeId.HasValue)
        {
            var employeeDetail = await _plandayEmployeeProvider.GetEmployeeAsync(shift.EmployeeId.Value);
            if (employeeDetail != default)
                return new ShiftEmployee(shift.Id, shift.EmployeeId, shift.Start, shift.End, employeeDetail.Email);
        }

        return new ShiftEmployee(shift.Id, shift.EmployeeId, shift.Start, shift.End, null);

    }
}