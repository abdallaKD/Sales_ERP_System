using ERP.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Product Image")]
        public string? Image { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "SKU")]
        public string SKU { get; set; } = string.Empty;

        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost price must be zero or greater.")]
        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Selling price must be zero or greater.")]
        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Category ID")]
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }

        public IEnumerable<Category>? Categories { get; set; }


    }
}
