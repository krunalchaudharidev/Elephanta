using System;
using System.Threading.Tasks;
using Elephanta.Application.Common;
using Elephanta.Domain.Entities;
using System.Collections.Generic;

namespace Elephanta.Application.Features.Catalog.Interfaces;

public interface IProductService
{
    // Category
    Task<Category> AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task<Category?> GetCategoryByIdAsync(Guid id);
    Task<Category?> GetCategoryByMediaIdAsync(Guid mediaId);
    Task<PagedResult<Category>> GetCategoriesAsync(int pageNumber, int pageSize, string? name, string? sort, bool? isActive);
    Task<Dictionary<Guid, int>> GetProductCountsByCategoryAsync(IEnumerable<Guid> categoryIds);

    // Product
    Task<Product> AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<PagedResult<Product>> GetProductsAsync(int pageNumber, int pageSize);

    // Images
    Task<ProductImage> AddImageAsync(ProductImage img);
    Task UpdateImageAsync(ProductImage img);
    Task<ProductImage?> GetImageByIdAsync(Guid id);
    Task<ProductImage?> GetImageByMediaIdAsync(Guid mediaId);
    Task<List<ProductImage>> GetImagesByProductAsync(Guid productId);
    Task DeleteImageAsync(Guid id);

    // Reviews
    Task<ProductReview> AddReviewAsync(ProductReview review);
    Task UpdateReviewAsync(ProductReview review);
    Task<ProductReview?> GetReviewByIdAsync(Guid id);
    Task<List<ProductReview>> GetReviewsByProductAsync(Guid productId);

    // Search
    Task<PagedResult<Product>> SearchProductsAsync(string? name, decimal? minPrice, decimal? maxPrice, List<Guid>? categoryIds, string? sort, bool? isActive, bool? isFeatured, int? minRating, bool? isStockAvailable, int pageNumber, int pageSize);

    // Category relationship / deletion helpers
    Task<bool> CategoryHasChildrenAsync(Guid categoryId);
    Task<bool> IsCategoryLinkedToProductsAsync(Guid categoryId);
    Task DeleteCategoryAsync(Guid categoryId);
}
