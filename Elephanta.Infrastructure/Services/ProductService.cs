using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;

namespace Elephanta.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public Task<Category> AddCategoryAsync(Category category) => _repo.AddCategoryAsync(category);
    public Task UpdateCategoryAsync(Category category) => _repo.UpdateCategoryAsync(category);
    public Task<Category?> GetCategoryByIdAsync(Guid id) => _repo.GetCategoryByIdAsync(id);
    public Task<Elephanta.Application.Common.PagedResult<Category>> GetCategoriesAsync(int pageNumber, int pageSize) => _repo.GetCategoriesAsync(pageNumber, pageSize);

    public Task<Product> AddProductAsync(Product product) => _repo.AddProductAsync(product);
    public Task UpdateProductAsync(Product product) => _repo.UpdateProductAsync(product);
    public Task<Product?> GetProductByIdAsync(Guid id) => _repo.GetProductByIdAsync(id);
    public Task<Elephanta.Application.Common.PagedResult<Product>> GetProductsAsync(int pageNumber, int pageSize) => _repo.GetProductsAsync(pageNumber, pageSize);

    public Task<ProductImage> AddImageAsync(ProductImage img) => _repo.AddImageAsync(img);
    public Task UpdateImageAsync(ProductImage img) => _repo.UpdateImageAsync(img);
    public Task<ProductImage?> GetImageByIdAsync(Guid id) => _repo.GetImageByIdAsync(id);
    public Task<List<ProductImage>> GetImagesByProductAsync(Guid productId) => _repo.GetImagesByProductAsync(productId);

    public Task<ProductReview> AddReviewAsync(ProductReview review) => _repo.AddReviewAsync(review);
    public Task UpdateReviewAsync(ProductReview review) => _repo.UpdateReviewAsync(review);
    public Task<ProductReview?> GetReviewByIdAsync(Guid id) => _repo.GetReviewByIdAsync(id);
    public Task<List<ProductReview>> GetReviewsByProductAsync(Guid productId) => _repo.GetReviewsByProductAsync(productId);

    public Task<Elephanta.Application.Common.PagedResult<Product>> SearchProductsAsync(string? name, decimal? minPrice, decimal? maxPrice, Guid? categoryId, string? sort, bool? isActive, int pageNumber, int pageSize)
        => _repo.SearchProductsAsync(name, minPrice, maxPrice, categoryId, sort, isActive, pageNumber, pageSize);
}
