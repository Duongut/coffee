using CafeApp.Core.Entities;
using CafeApp.Core.Interfaces;

namespace CafeApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
        {
            return await _unitOfWork.Products.FindAsync(p => p.IsAvailable);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _unitOfWork.Products.FindAsync(p => p.CategoryId == categoryId && p.IsAvailable);
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _unitOfWork.Products.FindAsync(p => p.IsFeatured && p.IsAvailable);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(int id)
        {
            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _unitOfWork.Products.ExistsAsync(id);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAvailableProductsAsync();

            var lowerSearchTerm = searchTerm.ToLower();
            return await _unitOfWork.Products.FindAsync(p => 
                p.IsAvailable && 
                (p.Name.ToLower().Contains(lowerSearchTerm) || 
                 (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm))));
        }
    }
}
