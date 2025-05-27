using CafeApp.Core.Entities;
using CafeApp.Core.Interfaces;
using CafeApp.Core.Enums;

namespace CafeApp.Services
{
    public class TableService : ITableService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int MAX_TABLES = 20;
        private const int SESSION_TIMEOUT_MINUTES = 120; // 2 hours

        public TableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsTableAvailableAsync(int tableNumber)
        {
            if (tableNumber < 1 || tableNumber > MAX_TABLES)
                return false;

            var table = await GetOrCreateTableAsync(tableNumber);
            
            // Check if table is occupied and session is still valid
            if (table.IsOccupied && !string.IsNullOrEmpty(table.SessionId))
            {
                // Check if session has expired
                if (table.LastActivityAt.HasValue && 
                    DateTime.UtcNow.Subtract(table.LastActivityAt.Value).TotalMinutes > SESSION_TIMEOUT_MINUTES)
                {
                    // Session expired, release table
                    await ReleaseTableAsync(tableNumber);
                    return true;
                }
                return false;
            }

            return true;
        }

        public async Task<bool> IsTableOccupiedAsync(int tableNumber)
        {
            return !await IsTableAvailableAsync(tableNumber);
        }

        public async Task<string> GenerateTableSessionIdAsync(int tableNumber)
        {
            var sessionId = $"TBL_{tableNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
            return sessionId;
        }

        public async Task<bool> ValidateTableSessionAsync(int tableNumber, string sessionId)
        {
            var table = await GetTableByNumberAsync(tableNumber);
            if (table == null || !table.IsOccupied)
                return false;

            return table.SessionId == sessionId;
        }

        public async Task OccupyTableAsync(int tableNumber, string sessionId)
        {
            var table = await GetOrCreateTableAsync(tableNumber);
            
            table.IsOccupied = true;
            table.SessionId = sessionId;
            table.OccupiedAt = DateTime.UtcNow;
            table.LastActivityAt = DateTime.UtcNow;
            table.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReleaseTableAsync(int tableNumber)
        {
            var table = await GetTableByNumberAsync(tableNumber);
            if (table != null)
            {
                table.IsOccupied = false;
                table.SessionId = null;
                table.OccupiedAt = null;
                table.LastActivityAt = DateTime.UtcNow;
                table.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<int>> GetAvailableTablesAsync()
        {
            var availableTables = new List<int>();
            
            for (int i = 1; i <= MAX_TABLES; i++)
            {
                if (await IsTableAvailableAsync(i))
                {
                    availableTables.Add(i);
                }
            }
            
            return availableTables;
        }

        public async Task<IEnumerable<int>> GetOccupiedTablesAsync()
        {
            var occupiedTables = new List<int>();
            
            for (int i = 1; i <= MAX_TABLES; i++)
            {
                if (await IsTableOccupiedAsync(i))
                {
                    occupiedTables.Add(i);
                }
            }
            
            return occupiedTables;
        }

        public async Task<int> GetNextAvailableTableAsync()
        {
            var availableTables = await GetAvailableTablesAsync();
            return availableTables.FirstOrDefault();
        }

        public async Task<bool> HasActiveOrdersAsync(int tableNumber)
        {
            var activeStatuses = new[] { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.Preparing, OrderStatus.Ready };
            var orders = await _unitOfWork.Orders.FindAsync(o => o.TableNumber == tableNumber && activeStatuses.Contains(o.Status));
            return orders.Any();
        }

        private async Task<Table> GetOrCreateTableAsync(int tableNumber)
        {
            var table = await GetTableByNumberAsync(tableNumber);
            if (table == null)
            {
                table = new Table
                {
                    TableNumber = tableNumber,
                    IsOccupied = false,
                    IsActive = true,
                    Capacity = 4,
                    CreatedAt = DateTime.UtcNow
                };
                
                // Add table to database (we'll need to add this to UnitOfWork)
                // For now, we'll create a simple in-memory tracking
            }
            return table;
        }

        private async Task<Table?> GetTableByNumberAsync(int tableNumber)
        {
            // This would normally query the database
            // For now, we'll implement a simple solution
            return null;
        }
    }
}
