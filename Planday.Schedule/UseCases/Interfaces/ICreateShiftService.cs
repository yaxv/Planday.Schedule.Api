namespace Planday.Schedule.UseCases.Interfaces;

public interface ICreateShiftService
{
    Task<Shift> HandleAsync(CreateShift shift);
}