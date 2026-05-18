using ERP.Domain.Enums;
using ERP.Domain.Models;
using ERP.Repositories.Repository;
using ERP.Services.InventoryLogService;
using ERP.Services.ViewModels.OrderVM;

namespace ERP.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryLogService inventoryLogService;

        public OrderService(IUnitOfWork unitOfWork, IInventoryLogService inventoryLogService)
        {
            _unitOfWork = unitOfWork;
            this.inventoryLogService = inventoryLogService;
        }


        // Index
        public async Task<IEnumerable<OrderViewModel>> GetAllOrdersAsync()
        {
            // جرب بدون أي Include أول شيء
            var orders = await _unitOfWork.Orders.GetAllAsync();

            var customerIds = orders.Select(o => o.CustomerId).Distinct().ToList();
            var customers = await _unitOfWork.Customers
                                  .FindAsync(c => customerIds.Contains(c.Id));
            var customerDict = customers.ToDictionary(c => c.Id);

            return orders.Select(o =>
            {
                customerDict.TryGetValue(o.CustomerId, out var customer);
                return new OrderViewModel
                {
                    Id = o.Id,
                    CustomerName = customer?.Name ?? "Unknown",
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    PaidAmount = o.PaidAmount,
                    RemainingAmount = o.TotalAmount - o.PaidAmount
                };
            });
        }



        // ── DETAILS ────────────────────────────────────────────
        public async Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return null;

            var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId);
            var items = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == id);

            var itemList = new List<OrderItemFormViewModel>();

            if (items != null && items.Any())
            {
                var productIds = items.Select(i => i.ProductId).Distinct().ToList();
                var products = await _unitOfWork.Products.FindAsync(p => productIds.Contains(p.Id));
                var productDict = products.ToDictionary(p => p.Id);

                itemList = items.Select(oi =>
                {
                    productDict.TryGetValue(oi.ProductId, out var product);
                    return new OrderItemFormViewModel
                    {
                        ProductId = oi.ProductId,
                        ProductName = product?.Name ?? "Unknown",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                    };
                }).ToList();
            }

            return new OrderDetailsViewModel
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = customer?.Name ?? "Unknown",
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                PaidAmount = order.PaidAmount,
                RemainingAmount = order.RemainingAmount,
                Items = itemList
            };
        }


        // ── CREATE ─────────────────────────────────────────────
        public async Task CreateOrderAsync(OrderDetailsViewModel model, string userId)
        {
            if (model.Items == null || !model.Items.Any())
                throw new Exception("Cannot create an order with no items.");

            var order = new Order
            {
                CustomerId = model.CustomerId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                CreatedByUserId = userId
            };

            await _unitOfWork.Orders.AddAsync(order);

            decimal total = 0;

            foreach (var item in model.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId)
                    ?? throw new Exception($"Product {item.ProductId} not found.");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Not enough stock for: {product.Name}. Available: {product.StockQuantity}");

                product.StockQuantity -= item.Quantity;
                _unitOfWork.Products.Update(product);

                await _unitOfWork.OrderItems.AddAsync(new OrderItem
                {
                    Order = order,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.SellingPrice
                });

                total += item.Quantity * product.SellingPrice;
            }

            order.TotalAmount = total;

            await _unitOfWork.CompleteAsync();
            await inventoryLogService.CreateInventoryLogAsync(order);
        }




        // ── UPDATE ─────────────────────────────────────────────
        public async Task UpdateOrderAsync(OrderDetailsViewModel model)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(model.OrderId)
                ?? throw new Exception($"Order {model.OrderId} not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Cannot edit a cancelled order.");

            if (model.Items == null || !model.Items.Any())
                throw new Exception("Cannot update an order with no items.");

            //Inventory log
            await inventoryLogService.CreateInventoryLogAsync(order, true);

            // 1. Restore old stock and remove old items
            var oldItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == order.Id);

            foreach (var old in oldItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(old.ProductId);
                if (product != null)
                {
                    product.StockQuantity += old.Quantity;
                    _unitOfWork.Products.Update(product);
                }
                _unitOfWork.OrderItems.Delete(old);
            }

            // 2. Add new items and deduct stock
            decimal total = 0;

            foreach (var item in model.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId)
                    ?? throw new Exception($"Product {item.ProductId} not found.");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Not enough stock for: {product.Name}. Available: {product.StockQuantity}");

                product.StockQuantity -= item.Quantity;
                _unitOfWork.Products.Update(product);

                await _unitOfWork.OrderItems.AddAsync(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.SellingPrice
                });

                total += item.Quantity * product.SellingPrice;
            }

            // 3. Update the order — no UpdatedByUserId since model is fixed
            order.TotalAmount = total;
            order.UpdatedAt = DateTime.Now;

            _unitOfWork.Orders.Update(order);

            await _unitOfWork.CompleteAsync();
            await inventoryLogService.CreateInventoryLogAsync(order);
        }



        // ── CANCEL ─────────────────────────────────────────────
        public async Task CancelOrderAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id)
                ?? throw new Exception($"Order {id} not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Order is already cancelled.");

            if (order.PaidAmount > 0)
                throw new Exception("Cannot cancel an order with payments. Please process a refund first.");

            var items = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == id);

            foreach (var item in items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _unitOfWork.Products.Update(product);
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.Now;

            _unitOfWork.Orders.Update(order);

            await _unitOfWork.CompleteAsync();
        }






    }
}