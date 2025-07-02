using System;

namespace Planday.Schedule
{
    public class CreateShift
    {
        public CreateShift(DateTime start, DateTime end)
        {
            Start = start; 
            End = end;
        }

        public DateTime Start { get; }
        public DateTime End { get; }
    }

    public class Shift : CreateShift
    {
        public Shift(long id, long? employeeId, DateTime start, DateTime end) 
            : base(start, end)
        {
            Id = id;
            EmployeeId = employeeId;
        }

        public long Id { get; }
        public long? EmployeeId { get; }
    }

    public class ShiftEmployee : Shift
    {
        public ShiftEmployee(long id, long? employeeId, DateTime start, DateTime end, string employeeEmail) 
            : base(id, employeeId, start, end)
        {
            EmployeeEmail = employeeEmail;
        }

        public string EmployeeEmail { get; }
    }
}

