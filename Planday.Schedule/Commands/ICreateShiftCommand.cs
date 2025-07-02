namespace Planday.Schedule.Commands;

public interface ICreateShiftCommand
{
    Task<uint> HandleAsync(CreateShift shift);
}