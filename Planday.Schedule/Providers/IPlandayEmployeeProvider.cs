namespace Planday.Schedule.Providers;

public interface IPlandayEmployeeProvider
{
    Task<(string Name, string Email)> GetEmployeeAsync(long id);
}