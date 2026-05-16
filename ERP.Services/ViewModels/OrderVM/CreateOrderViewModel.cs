using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ViewModels.OrderVM
{
    public class CreateOrderViewModel
    {
        public int CustomerId { get; set; }

        public List<OrderItemFormViewModel> Items { get; set; } = new();

        public IEnumerable<SelectListItem> Customers { get; set; }
        public IEnumerable<SelectListItem> Products { get; set; }
    }
}
