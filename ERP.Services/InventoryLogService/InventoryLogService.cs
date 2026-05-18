using ERP.Domain.Enums;
using ERP.Domain.Models;
using ERP.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.InventoryLogService
{
    public class InventoryLogService : IInventoryLogService
    {
        private readonly IUnitOfWork unitOfWork;

        public InventoryLogService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<InventoryLog>> GetAllInventoryLogsAsync()
        {
            return await unitOfWork.InventoryLogs.GetAllAsync();
        }

        public async Task<InventoryLog> GetInventoryLogByIdAsync(int id)
        {
            return await unitOfWork.InventoryLogs.GetByIdAsync(id);
        }

        public async Task<bool> CreateInventoryLogAsync<T>(T log, bool IsAdjusted = false)
        {
            if (log == null) return false;
            var cons = IsAdjusted ? -1 : 1;


            if (log.GetType() == typeof(Order))
            {
                Order? order = log as Order;
                //List<OrderItem> items = (await unitOfWork.OrderItems.GetAllAsync(i => i.OrderId == order.Id)).ToList();
                // Do this — fetch all, filter in memory with LINQ:
                List<OrderItem> items = (await unitOfWork.OrderItems.GetAllAsync())
                                            .Where(i => i.OrderId == order.Id)
                                            .ToList();
                if (items == null || items.Count <= 0) return false;

                foreach (OrderItem item in items)
                {
                    InventoryLog inventoryLog = new InventoryLog()
                    {
                        Quantity = item.Quantity * -1 * cons,
                        Type = IsAdjusted ? InventoryMovementType.Adjustment : InventoryMovementType.Out,
                        OrderId = order.Id,
                        PurchaseId = null,
                        ProductId = item.ProductId,
                        CreatedByUserId = order.CreatedByUserId
                    };

                    await unitOfWork.InventoryLogs.AddAsync(inventoryLog);
                }

            }
            else if (log.GetType() == typeof(Purchase))
            {
                Purchase? purchase = log as Purchase;
                //List<PurchaseItem> items = (await unitOfWork.PurchaseItems.GetAllAsync(i => i.PurchaseId == purchase.Id)).ToList();

                List<PurchaseItem> items = (await unitOfWork.PurchaseItems.GetAllAsync()).Where(i => i.PurchaseId == purchase.Id).ToList();

                if (items == null || items.Count <= 0) return false;

                foreach (PurchaseItem item in items)
                {
                    InventoryLog inventoryLog = new InventoryLog()
                    {
                        Quantity = item.Quantity * cons,
                        Type = IsAdjusted ? InventoryMovementType.Adjustment : InventoryMovementType.In,
                        OrderId = null,
                        PurchaseId = item.PurchaseId,
                        ProductId = item.ProductId,
                        CreatedByUserId = purchase.CreatedByUserId
                    };

                    await unitOfWork.InventoryLogs.AddAsync(inventoryLog);
                }
            }

            await unitOfWork.CompleteAsync();
            return true;
        }

        //public async Task<bool> DeleteInventoryLogAsync(int id)
        //{
        //    InventoryLog? log = await unitOfWork.InventoryLogs.GetByIdAsync(id);

        //    if (log == null) return false;


        //}

        public async Task<bool> UpdateInventoryLogAsync<T>(T entity)
        {
            if (entity == null) return false;

            if (entity.GetType() == typeof(Order))
            {
                var order = entity as Order;
                await CreateInventoryLogAsync(order, true);
                await CreateInventoryLogAsync(order);
            }
            else if (entity.GetType() == typeof(Purchase))
            {
                var purchase = entity as Purchase;
                await CreateInventoryLogAsync(purchase, true);
                await CreateInventoryLogAsync(purchase);
            }

            await unitOfWork.CompleteAsync();
            return true;
        }




    }
}
