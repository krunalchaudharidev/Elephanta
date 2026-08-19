using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Catalog.Interfaces;

public interface IProductRepository
{
    // Category
    Task<Category> AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task<Category?> GetCategoryByIdAsync(Guid id);
    Task<Elephanta.Application.Common.PagedResult<Category>> GetCategoriesAsync(int pageNumber, int pageSize);

    // Product
    Task<Product> AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Elephanta.Application.Common.PagedResult<Product>> GetProductsAsync(int pageNumber, int pageSize);

    // Images
    Task<ProductImage> AddImageAsync(ProductImage img);
    Task UpdateImageAsync(ProductImage img);
    Task<ProductImage?> GetImageByIdAsync(Guid id);
    Task<List<ProductImage>> GetImagesByProductAsync(Guid productId);

    // Reviews
    Task<ProductReview> AddReviewAsync(ProductReview review);
    Task UpdateReviewAsync(ProductReview review);
    Task<ProductReview?> GetReviewByIdAsync(Guid id);
    Task<List<ProductReview>> GetReviewsByProductAsync(Guid productId);

    // Search / filter
    Task<Elephanta.Application.Common.PagedResult<Product>> SearchProductsAsync(string? name, decimal? minPrice, decimal? maxPrice, Guid? categoryId, string? sort, bool? isActive, int pageNumber, int pageSize);
}
