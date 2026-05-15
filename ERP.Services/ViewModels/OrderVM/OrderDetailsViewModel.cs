using ERP.Services.ViewModels.CustomerVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ViewModels.OrderVM
{
    public class OrderDetailsViewModel
    {
        //i also used it for EDIT /DElete / create

        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount { get; set; }

         public List<OrderItemFormViewModel> Items { get; set; } = new();
         public IEnumerable<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
        public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();

    }
}
