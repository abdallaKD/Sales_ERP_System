using ERP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.InventoryLogService
{
    public interface IInventoryLogService
    {
        Task<IEnumerable<InventoryLog>> GetAllInventoryLogsAsync();
        Task<InventoryLog> GetInventoryLogByIdAsync(int id);
        Task<bool> CreateInventoryLogAsync<T>(T log, bool IsAdjusted = false);

        Task<bool> UpdateInventoryLogAsync<T>(T entity);

        //Task<bool> UpdateInventoryLogAsync(InventoryLog log);
        //Task<bool> DeleteInventoryLogAsync(int id);
    }
}
