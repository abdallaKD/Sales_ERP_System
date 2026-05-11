using ERP.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Total Products")]
        public int ProductsCount { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
