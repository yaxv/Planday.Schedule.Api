using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class GetAllShiftsQuery : IGetAllShiftsQuery
    {
        private readonly SqliteConnection _sqliteConnection;

        public GetAllShiftsQuery(SqliteConnection sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
        }
    
        public async Task<IReadOnlyCollection<Shift>> QueryAsync()
        {
            var shiftDtos = await _sqliteConnection.QueryAsync<ShiftDto>(Sql);

            var shifts = shiftDtos.Select(x => 
                new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));
        
            return shifts.ToList();
        }

        private const string Sql = @"SELECT Id, EmployeeId, Start, End FROM Shift;";
    
        private record ShiftDto(long Id, long? EmployeeId, string Start, string End);
    }    
}

