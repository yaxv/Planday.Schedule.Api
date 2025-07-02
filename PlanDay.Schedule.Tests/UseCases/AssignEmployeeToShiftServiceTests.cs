using Moq;
using Planday.Schedule;
using Planday.Schedule.Commands;
using Planday.Schedule.Queries;
using Planday.Schedule.UseCases;

namespace PlanDay.Schedule.Tests.UseCases;

public class AssignEmployeeToShiftServiceTests
{
    private readonly Mock<IUpdateShiftCommand> _updateShiftCommandMock = new();
    private readonly Mock<IEmployeeExistsQuery> _employeeExistsQueryMock = new();
    private readonly Mock<IGetOverlappingShiftsQuery> _getOverlappingShiftsQueryMock = new();

    [Fact]
    public async Task AssignEmployeeToShiftService_Success()
    {
        // Arrange
        long shiftId = 1;
        long employeeId = 1;

        _employeeExistsQueryMock.Setup(client => client.QueryAsync(employeeId))
            .ReturnsAsync(true);

        _getOverlappingShiftsQueryMock.Setup(client => client.QueryAsync(shiftId, employeeId))
            .ReturnsAsync(new List<Shift>());

        _updateShiftCommandMock.Setup(client => client.HandleAsync(shiftId, employeeId))
            .ReturnsAsync(1);

        var service = new AssignEmployeeToShiftService(_updateShiftCommandMock.Object,
            _employeeExistsQueryMock.Object, _getOverlappingShiftsQueryMock.Object);

        // Act
        await service.HandleAsync(shiftId, employeeId);
    }

    [Fact]
    public async Task AssignEmployeeToShiftService_EmployeeDoesNotExist_Failed()
    {
        // Arrange
        long shiftId = 1;
        long employeeId = 1;

        _employeeExistsQueryMock.Setup(client => client.QueryAsync(employeeId))
            .ReturnsAsync(false);

        var service = new AssignEmployeeToShiftService(_updateShiftCommandMock.Object,
            _employeeExistsQueryMock.Object, _getOverlappingShiftsQueryMock.Object);

        // Act
        var exception = await Assert.ThrowsAsync<ApplicationException>(() => service.HandleAsync(shiftId, employeeId));

        //Assert

        Assert.NotNull(exception);
        Assert.Equal("The employee does not exist", exception.Message);
    }
}