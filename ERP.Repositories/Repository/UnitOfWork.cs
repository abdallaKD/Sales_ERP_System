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
        GenericRepository<Product> productRepo;
        GenericRepository<Category> categoriesRepo;
        GenericRepository<Order> ordersRepo;
        GenericRepository<InventoryLog> inventoryLogsRepo;
        GenericRepository<Customer> customersRepo;
        GenericRepository<OrderItem> orderItemsRepo;
        GenericRepository<Purchase> purchasesRepo;
        GenericRepository<PurchaseItem> purchaseItemsRepo;
        GenericRepository<Supplier> suppliersRepo;
        GenericRepository<Payment> paymentsRepo;
        GenericRepository<ApplicationUser> applicationUsersRepo;

        public UnitOfWork(ERPDBContext context)
        {
            _context = context;
        }
            

        public IGenericRepository<Product> Products
        {
            get
            {
                if (productRepo == null)
                    productRepo = new GenericRepository<Product>(_context);
                return productRepo;
            }
        }

        public IGenericRepository<Category> Categories
        {
            get
            {
                if (categoriesRepo == null)
                    categoriesRepo = new GenericRepository<Category>(_context);
                return categoriesRepo;
            }
        }

        public IGenericRepository<Order> Orders
        {
            get
            {
                if (ordersRepo == null)
                    ordersRepo = new GenericRepository<Order>(_context);
                return ordersRepo;
            }
        }

        public IGenericRepository<InventoryLog> InventoryLogs
        {
            get
            {
                if (inventoryLogsRepo == null)
                    inventoryLogsRepo = new GenericRepository<InventoryLog>(_context);
                return inventoryLogsRepo;
            }
        }

        public IGenericRepository<Customer> Customers
        {
            get
            {
                if (customersRepo == null)
                    customersRepo = new GenericRepository<Customer>(_context);
                return customersRepo;
            }
        }

        public IGenericRepository<OrderItem> OrderItems
        {
            get
            {
                if (orderItemsRepo == null)
                    orderItemsRepo = new GenericRepository<OrderItem>(_context);
                return orderItemsRepo;
            }
        }

        public IGenericRepository<Purchase> Purchases
        {
            get
            {
                if (purchasesRepo == null)
                    purchasesRepo = new GenericRepository<Purchase>(_context);
                return purchasesRepo;
            }
        }

        public IGenericRepository<PurchaseItem> PurchaseItems
        {
            get
            {
                if (purchaseItemsRepo == null)
                    purchaseItemsRepo = new GenericRepository<PurchaseItem>(_context);
                return purchaseItemsRepo;
            }
        }

        public IGenericRepository<Supplier> Suppliers
        {
            get
            {
                if (suppliersRepo == null)
                    suppliersRepo = new GenericRepository<Supplier>(_context);
                return suppliersRepo;
            }
        }

        public IGenericRepository<Payment> Payments
        {
            get
            {
                if (paymentsRepo == null)
                    paymentsRepo = new GenericRepository<Payment>(_context);
                return paymentsRepo;
            }
        }

        public IGenericRepository<ApplicationUser> ApplicationUsers
        {
            get
            {
                if (applicationUsersRepo == null)
                    applicationUsersRepo = new GenericRepository<ApplicationUser>(_context);
                return applicationUsersRepo;
            }
        }


        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
