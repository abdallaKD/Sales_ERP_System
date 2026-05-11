using ERP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Repositories.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<InventoryLog> InventoryLogs { get; }
        IGenericRepository<Customer> Customers { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<Purchase> Purchases { get; }
        IGenericRepository<PurchaseItem> PurchaseItems { get; }
        IGenericRepository<Supplier> Suppliers { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<ApplicationUser> ApplicationUsers { get; }

        Task<int> CompleteAsync();
    }
}
