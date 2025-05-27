using CafeApp.Core.Entities;

namespace CafeApp.Core.Interfaces
{
    public interface ITableService
    {
        Task<bool> IsTableAvailableAsync(int tableNumber);
        Task<bool> IsTableOccupiedAsync(int tableNumber);
        Task<string> GenerateTableSessionIdAsync(int tableNumber);
        Task<bool> ValidateTableSessionAsync(int tableNumber, string sessionId);
        Task OccupyTableAsync(int tableNumber, string sessionId);
        Task ReleaseTableAsync(int tableNumber);
        Task<IEnumerable<int>> GetAvailableTablesAsync();
        Task<IEnumerable<int>> GetOccupiedTablesAsync();
        Task<int> GetNextAvailableTableAsync();
        Task<bool> HasActiveOrdersAsync(int tableNumber);
    }
}
