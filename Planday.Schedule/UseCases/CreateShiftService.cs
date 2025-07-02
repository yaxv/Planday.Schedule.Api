using System.ComponentModel.DataAnnotations;
using Planday.Schedule.Commands;
using Planday.Schedule.UseCases.Interfaces;

namespace Planday.Schedule.UseCases;

public class CreateShiftService : ICreateShiftService
{
    private readonly ICreateShiftCommand _createShiftCommand;

    public CreateShiftService(ICreateShiftCommand createShiftCommand)
    {
        _createShiftCommand = createShiftCommand;
    }

    public async Task<Shift> HandleAsync(CreateShift shift)
    {
        if (shift.Start > shift.End)
            throw new ValidationException($"Input {nameof(shift.Start)} cannot be later than {nameof(shift.End)}");

        if (shift.End.Day != shift.Start.Day)
            throw new ValidationException("A shift must be scheduled within the same day");

        var shiftId = await _createShiftCommand.HandleAsync(shift);

        return new (shiftId, null, shift.Start, shift.End);
    }
}