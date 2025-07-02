namespace Planday.Schedule.UseCases.Interfaces;

public interface IGetShiftService
{
    Task<ShiftEmployee> GetShiftByIdAsync(long id);
}