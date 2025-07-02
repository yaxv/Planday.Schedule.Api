using Moq;
using Planday.Schedule;
using Planday.Schedule.Providers;
using Planday.Schedule.Queries;
using Planday.Schedule.UseCases;

namespace PlanDay.Schedule.Tests.UseCases
{
    public class GetShiftServiceTests
    {
        private readonly Mock<IGetShiftQuery> _getShiftQueryStub = new();
        private readonly Mock<IPlandayEmployeeProvider> _planDayEmployeeProviderStub = new();

        [Fact]
        public async Task GetShiftById_EmployeeIdIsNull()
        {
            // Arrange
            long shiftId = 12;
            long? employeeId = null;

            var shift = new Shift(shiftId, employeeId, DateTime.Parse("2025-06-30"), DateTime.Parse("2025-07-01"));

            _getShiftQueryStub.Setup(stub => stub.QueryAsync(shiftId))
                .ReturnsAsync(shift);

            _planDayEmployeeProviderStub.Verify(stub => stub.GetEmployeeAsync(It.IsAny<long>()), Times.Never());

            var service = new GetShiftService(_getShiftQueryStub.Object, _planDayEmployeeProviderStub.Object);

            // Act
            var shiftEmployee = await service.GetShiftByIdAsync(shiftId);

            // Assert
            Assert.NotNull(shiftEmployee);
            Assert.Null(shiftEmployee.EmployeeEmail);
            Assert.Null(shiftEmployee.EmployeeId);
            Assert.Equal(shift.Start, shiftEmployee.Start);
            Assert.Equal(shift.End, shiftEmployee.End);
        }

        [Fact]
        public async Task GetShiftById_Success()
        {
            // Arrange
            long shiftId = 12;
            long employeeId = 25;

            var shift = new Shift(shiftId, employeeId, DateTime.Parse("2025-06-30"), DateTime.Parse("2025-07-01"));

            _getShiftQueryStub.Setup(stub => stub.QueryAsync(shiftId))
                .ReturnsAsync(shift);

            var employee = (Name: "Michael Jordan", Email: "jordan@gmail.com");

            _planDayEmployeeProviderStub.Setup(stub => stub.GetEmployeeAsync(employeeId))
                .ReturnsAsync(employee);

            var service = new GetShiftService(_getShiftQueryStub.Object, _planDayEmployeeProviderStub.Object);

            // Act
            var shiftEmployee = await service.GetShiftByIdAsync(shiftId);

            // Assert
            Assert.NotNull(shiftEmployee);
            Assert.Equal(employee.Email, shiftEmployee.EmployeeEmail);
            Assert.Equal(shift.Id, shiftEmployee.Id);
            Assert.Equal(shift.EmployeeId, shiftEmployee.EmployeeId.Value);
            Assert.Equal(shift.Start, shiftEmployee.Start);
            Assert.Equal(shift.End, shiftEmployee.End);
        }
    }
}
