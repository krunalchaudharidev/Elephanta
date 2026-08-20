using Elephanta.Application.Common;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ElephantaDbContext _db;

    public ProductRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    // Category
    public async Task<Category> AddCategoryAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        return await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PagedResult<Category>> GetCategoriesAsync(int pageNumber, int pageSize)
    {
        var q = _db.Categories.AsQueryable();
        var total = await q.CountAsync();
        var items = await q.OrderBy(c => c.DisplayOrder).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new Elephanta.Application.Common.PagedResult<Category> { Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    // Product
    public async Task<Product> AddProductAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _db.Products.Include(p => p.Images).Include(p => p.Reviews).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PagedResult<Product>> GetProductsAsync(int pageNumber, int pageSize)
    {
        var q = _db.Products.Include(p => p.Images).Include(p => p.Reviews).AsQueryable();
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(p => p.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Product> { Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    // Images
    public async Task<ProductImage> AddImageAsync(ProductImage img)
    {
        _db.ProductImages.Add(img);
        await _db.SaveChangesAsync();
        return img;
    }

    public async Task UpdateImageAsync(ProductImage img)
    {
        _db.ProductImages.Update(img);
        await _db.SaveChangesAsync();
    }

    public async Task<ProductImage?> GetImageByIdAsync(Guid id)
    {
        return await _db.ProductImages.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<ProductImage>> GetImagesByProductAsync(Guid productId)
    {
        return await _db.ProductImages.Where(i => i.ProductId == productId).ToListAsync();
    }

    // Reviews
    public async Task<ProductReview> AddReviewAsync(ProductReview review)
    {
        _db.ProductReviews.Add(review);
        await _db.SaveChangesAsync();
        return review;
    }

    public async Task UpdateReviewAsync(ProductReview review)
    {
        _db.ProductReviews.Update(review);
        await _db.SaveChangesAsync();
    }

    public async Task<ProductReview?> GetReviewByIdAsync(Guid id)
    {
        return await _db.ProductReviews.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<ProductReview>> GetReviewsByProductAsync(Guid productId)
    {
        return await _db.ProductReviews.Where(r => r.ProductId == productId).ToListAsync();
    }

    // Search
    public async Task<PagedResult<Product>> SearchProductsAsync(string? name, decimal? minPrice, decimal? maxPrice, Guid? categoryId, string? sort, bool? isActive, int pageNumber, int pageSize)
    {
        var q = _db.Products.Include(p => p.Images).Include(p => p.Reviews).AsQueryable();

        if (!string.IsNullOrWhiteSpace(name)) q = q.Where(p => p.Name.Contains(name));
        if (minPrice.HasValue) q = q.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) q = q.Where(p => p.Price <= maxPrice.Value);
        if (categoryId.HasValue) q = q.Where(p => p.CategoryId == categoryId.Value);
        if (isActive.HasValue) q = q.Where(p => p.IsActive == isActive.Value);

        // sorting
        q = sort?.ToLower() switch
        {
            "priceasc" => q.OrderBy(p => p.Price),
            "pricedesc" => q.OrderByDescending(p => p.Price),
            "nameasc" => q.OrderBy(p => p.Name),
            "namedesc" => q.OrderByDescending(p => p.Name),
            _ => q.OrderByDescending(p => p.CreatedAt)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Product> { Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
    }
}
