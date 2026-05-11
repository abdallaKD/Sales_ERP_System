using ERP.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Repositories.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ERPDBContext _context;
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IGenericRepository<InventoryLog> InventoryLogs { get; private set; }
        public IGenericRepository<Customer> Customers { get; private set; }
        public IGenericRepository<OrderItem> OrderItems { get; private set; }
        public IGenericRepository<Purchase> Purchases { get; private set; }
        public IGenericRepository<PurchaseItem> PurchaseItems { get; private set; }
        public IGenericRepository<Supplier> Suppliers { get; private set; }
        public IGenericRepository<Payment> Payments { get; private set; }
        public IGenericRepository<ApplicationUser> ApplicationUsers { get; private set; }

        public UnitOfWork(ERPDBContext context, IGenericRepository<Product> products, IGenericRepository<Category> categories, IGenericRepository<Order> orders, IGenericRepository<InventoryLog> inventoryLogs, IGenericRepository<Customer> customers, IGenericRepository<OrderItem> orderItems, IGenericRepository<Purchase> purchases, IGenericRepository<PurchaseItem> purchaseItems, IGenericRepository<Supplier> suppliers, IGenericRepository<Payment> payments, IGenericRepository<ApplicationUser> applicationUsers)
        {
            _context = context;
            Products = products;
            Categories = categories;
            Orders = orders;
            InventoryLogs = inventoryLogs;
            Customers = customers;
            OrderItems = orderItems;
            Purchases = purchases;
            PurchaseItems = purchaseItems;
            Suppliers = suppliers;
            Payments = payments;
            ApplicationUsers = applicationUsers;
        }
        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
