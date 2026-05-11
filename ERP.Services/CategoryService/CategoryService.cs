using ERP.Domain.Models;
using ERP.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _unitOfWork.Categories.GetAllAsync(c => c.Products);

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            var categories = await _unitOfWork.Categories.FindAsync(c => c.Id == id, new[] { "Products" });

            return categories.FirstOrDefault();
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            var existingCategory = await _unitOfWork.Categories.FindAsync(c => c.Name == category.Name);
            if (existingCategory.Any())
                throw new Exception("A category with the same name already exists.");

            await _unitOfWork.Categories.AddAsync(category);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {

            _unitOfWork.Categories.Update(category);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.FindAsync(c => c.Id == id, new[] { "Products" });
            var categoryData = category.FirstOrDefault();

            if (categoryData == null) return false;

            if (categoryData.Products.Any())
                throw new Exception("The category cannot be deleted because it contains related products.");

            _unitOfWork.Categories.Delete(categoryData);
            return await _unitOfWork.CompleteAsync() > 0;
        }


    }
}
