namespace Planday.Schedule.Queries;

public interface IEmployeeExistsQuery
{
    Task<bool> QueryAsync(long id);

}