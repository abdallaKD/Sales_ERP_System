using ERP.Domain.Enums;
using ERP.Services.ViewModels.CustomerVM;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace ERP.Services.ViewModels.OrderVM
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public List<OrderItemFormViewModel> Items { get; set; } = new();

        // these are only for the view — never posted back
        public SelectList? Customers { get; set; }
        public SelectList? Products { get; set; }
    }
}
