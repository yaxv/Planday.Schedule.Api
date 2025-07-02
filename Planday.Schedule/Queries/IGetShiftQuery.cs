
namespace Planday.Schedule.Queries
{
    public interface IGetShiftQuery
    {
        Task<Shift> QueryAsync(long id);
    }
}
