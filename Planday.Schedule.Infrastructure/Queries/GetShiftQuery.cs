using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class GetShiftQuery : IGetShiftQuery
    {
        private readonly SqliteConnection _sqlConnection;

        public GetShiftQuery(SqliteConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public async Task<Shift> QueryAsync(long id)
        {
            var shiftDto = await _sqlConnection.QueryFirstOrDefaultAsync<ShiftDto>(Sql, new { ID = id });
            return shiftDto != null
                ? new Shift(shiftDto.Id, shiftDto.EmployeeId, DateTime.Parse(shiftDto.Start),
                    DateTime.Parse(shiftDto.End))
                : null;
        }

        private const string Sql = @"SELECT Id, EmployeeId, Start, End FROM Shift WHERE Id = @ID;";

        private record ShiftDto(long Id, long? EmployeeId, string Start, string End);
    }
}
