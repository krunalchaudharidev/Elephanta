using Elephanta.Application.Common;
using System.IO;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Application.Features.Catalog.DTOs;
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
        return await _db.Categories.Include(c => c.ParentCategory).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PagedResult<Category>> GetCategoriesAsync(int pageNumber, int pageSize, string? name, string? sort, bool? isActive)
    {
        var q = _db.Categories
            .Include(c => c.ParentCategory)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name)) q = q.Where(p => EF.Functions.ILike(p.Name, $"%{name}%"));
        if (isActive.HasValue) q = q.Where(c => c.IsActive == isActive.Value);

        // sorting
        q = sort?.ToLower() switch
        {
            "nameasc" => q.OrderBy(c => c.Name),
            "namedesc" => q.OrderByDescending(c => c.Name),
            "orderasc" => q.OrderBy(c => c.DisplayOrder),
            "orderdesc" => q.OrderByDescending(c => c.DisplayOrder),
            "createdatasc" => q.OrderBy(c => c.CreatedAt),
            "createdatdesc" => q.OrderByDescending(c => c.CreatedAt),
            _ => q.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Category> { Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    public async Task<Dictionary<Guid, int>> GetProductCountsByCategoryAsync(IEnumerable<Guid> categoryIds)
    {
        var ids = categoryIds.ToList();
        if (ids.Count == 0) return new Dictionary<Guid, int>();

        return await _db.Products
            .Where(p => !p.IsDeleted && ids.Contains(p.CategoryId))
            .GroupBy(p => p.CategoryId)
            .Select(group => new { CategoryId = group.Key, ProductCount = group.Count() })
            .ToDictionaryAsync(item => item.CategoryId, item => item.ProductCount);
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
        return await _db.Products
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PagedResult<Product>> GetProductsAsync(int pageNumber, int pageSize)
    {
        // include related data and exclude soft-deleted products so pagination and counts are correct
        var q = _db.Products
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.Category)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(p => p.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Product> { Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    public async Task<List<ProductRecommendationResponse>> GetYouMayAlsoLikeAsync(Guid productId, Guid categoryId, decimal price, int count)
    {
        if (count <= 0) return new List<ProductRecommendationResponse>();

        var minPrice = price * 0.9m;
        var maxPrice = price * 1.1m;
        var products = new List<Product>();
        var selectedIds = new HashSet<Guid>();

        IQueryable<Product> ActiveProducts() => _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.Id != productId && !p.IsDeleted && p.IsActive && p.Category.IsActive);

        var sameCategoryProducts = await ActiveProducts()
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => Math.Abs(p.Price - price))
            .ThenByDescending(p => p.CreatedAt)
            .Take(count)
            .AsSplitQuery()
            .ToListAsync();

        products.AddRange(sameCategoryProducts);
        selectedIds.UnionWith(sameCategoryProducts.Select(p => p.Id));

        var remainingCount = count - products.Count;
        if (remainingCount > 0)
        {
            var otherCategoryProducts = await ActiveProducts()
                .Where(p => p.CategoryId != categoryId && p.Price >= minPrice && p.Price <= maxPrice)
                .OrderBy(p => Math.Abs(p.Price - price))
                .ThenByDescending(p => p.CreatedAt)
                .Take(remainingCount)
                .AsSplitQuery()
                .ToListAsync();

            products.AddRange(otherCategoryProducts);
            selectedIds.UnionWith(otherCategoryProducts.Select(p => p.Id));
        }

        remainingCount = count - products.Count;
        if (remainingCount > 0)
        {
            var randomProducts = await ActiveProducts()
                .Where(p => !selectedIds.Contains(p.Id))
                .OrderBy(_ => EF.Functions.Random())
                .Take(remainingCount)
                .AsSplitQuery()
                .ToListAsync();

            products.AddRange(randomProducts);
            selectedIds.UnionWith(randomProducts.Select(p => p.Id));
        }

        var imageRows = await _db.ProductImages
            .AsNoTracking()
            .Where(image => selectedIds.Contains(image.ProductId) && image.MediaId.HasValue)
            .OrderBy(image => image.DisplayOrder)
            .ToListAsync();
        var imagesByProductId = imageRows
            .GroupBy(image => image.ProductId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var reviewRows = await _db.ProductReviews
            .AsNoTracking()
            .Where(review => selectedIds.Contains(review.ProductId))
            .Select(review => new
            {
                review.Id,
                review.ProductId,
                review.Rating,
                review.Comment,
                review.UserId,
            })
            .ToListAsync();

        var selectedReviewIds = reviewRows.Select(review => review.Id).ToHashSet();

        var reviewsByProductId = reviewRows
            .GroupBy(review => review.ProductId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(review => new ProductReviewResponse
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    UserId = review.UserId,
                }).ToList());

        return products.Select(product =>
        {
            var productImages = imagesByProductId.GetValueOrDefault(product.Id) ?? new List<ProductImage>();
            var productReviews = reviewsByProductId.GetValueOrDefault(product.Id) ?? new List<ProductReviewResponse>();

            return new ProductRecommendationResponse
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                SKU = product.SKU,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Price = product.Price,
                CompareAtPrice = product.CompareAtPrice,
                StockQuantity = product.StockQuantity,
                ReservedQuantity = product.ReservedQuantity,
                SoldQuantity = product.SoldQuantity,
                IsActive = product.IsActive,
                IsFeatured = product.IsFeatured,
                CategoryId = product.CategoryId,
                ImageIds = productImages.Select(image => image.MediaId!.Value).ToList(),
                ReviewCount = productReviews.Count,
                AverageRating = productReviews.Count > 0 ? (decimal)productReviews.Average(review => review.Rating) : 0,
                CategoryName = product.Category?.Name,
                PrimaryImageId = productImages.FirstOrDefault(image => image.IsPrimary)?.MediaId ?? productImages.FirstOrDefault()?.MediaId,
                IsCategoryActive = product.Category?.IsActive ?? false,
                Reviews = productReviews
            };
        }).ToList();
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

    public async Task DeleteImageAsync(Guid id)
    {
        var img = await _db.ProductImages.FirstOrDefaultAsync(i => i.Id == id);
        if (img == null) return;
        _db.ProductImages.Remove(img);
        await _db.SaveChangesAsync();
    }

    public async Task<Category?> GetCategoryByMediaIdAsync(Guid mediaId)
    {
        return await _db.Categories.FirstOrDefaultAsync(c => c.MediaId == mediaId);
    }

    public async Task<ProductImage?> GetImageByMediaIdAsync(Guid mediaId)
    {
        return await _db.ProductImages.FirstOrDefaultAsync(i => i.MediaId == mediaId);
    }

    // Reviews
    public async Task<ProductReview> AddReviewAsync(ProductReview review)
    {
        _db.ProductReviews.Add(review);
        await _db.SaveChangesAsync();
        return review;
    }

    public async Task UpdateReviewAsync(ProductReview review, List<Guid>? imageIds)
    {
        if (imageIds != null)
        {
            var desiredImageIds = imageIds.Distinct().ToList();
            var displayOrders = desiredImageIds
                .Select((mediaId, index) => new { mediaId, index })
                .ToDictionary(x => x.mediaId, x => x.index);
            var retainedMediaIds = new HashSet<Guid>();
            var currentImages = await _db.ProductReviewImages
                .Where(image => image.ProductReviewId == review.Id)
                .ToListAsync();

            foreach (var image in currentImages)
            {
                if (!image.MediaId.HasValue ||
                    !displayOrders.TryGetValue(image.MediaId.Value, out var displayOrder) ||
                    !retainedMediaIds.Add(image.MediaId.Value))
                {
                    _db.ProductReviewImages.Remove(image);
                    continue;
                }

                image.DisplayOrder = displayOrder;
            }

            foreach (var (mediaId, displayOrder) in displayOrders)
            {
                if (retainedMediaIds.Contains(mediaId)) continue;

                _db.ProductReviewImages.Add(new ProductReviewImage
                {
                    Id = Guid.NewGuid(),
                    ProductReviewId = review.Id,
                    MediaId = mediaId,
                    DisplayOrder = displayOrder,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<ProductReview?> GetReviewByIdAsync(Guid id)
    {
        return await _db.ProductReviews
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<PagedResult<ProductReviewResponse>> GetReviewsByProductAsync(Guid productId, int pageNumber, int pageSize)
    {
        var query = _db.ProductReviews.Where(r => r.ProductId == productId);
        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .ThenByDescending(r => r.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ProductReviewResponse
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                UserId = r.UserId,
                UserFirstName = _db.Users
                    .Where(u => (Guid?)u.Id == r.UserId)
                    .Select(u => u.FirstName)
                    .FirstOrDefault(),
                UserLastName = _db.Users
                    .Where(u => (Guid?)u.Id == r.UserId)
                    .Select(u => u.LastName)
                    .FirstOrDefault(),
                ImageIds = r.Images
                    .Where(image => image.MediaId.HasValue)
                    .OrderBy(image => image.DisplayOrder)
                    .Select(image => image.MediaId!.Value)
                    .ToList()
            })
            .ToListAsync();

        return new PagedResult<ProductReviewResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    // Search
    public async Task<PagedResult<Product>> SearchProductsAsync(string? name, decimal? minPrice, decimal? maxPrice, List<Guid>? categoryIds, string? sort, bool? isActive, bool? isFeatured, int? minRating, bool? isStockAvailable, int pageNumber, int pageSize)
    {
        // include related data and exclude soft-deleted products so pagination and counts are correct
        var q = _db.Products
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.Category)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name)) q = q.Where(p => EF.Functions.ILike(p.Name, $"%{name}%"));
        if (minPrice.HasValue) q = q.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) q = q.Where(p => p.Price <= maxPrice.Value);
        if (categoryIds is { Count: > 0 }) q = q.Where(p => categoryIds.Contains(p.CategoryId));
        if (isActive.HasValue)
        {
            // Only include products where the product's IsActive matches and, when a category exists,
            // the category's IsActive also matches the requested value.
            q = q.Where(p => p.IsActive == isActive.Value && (p.Category == null || p.Category.IsActive == isActive.Value));
        }
        if (isFeatured.HasValue) q = q.Where(p => p.IsFeatured == isFeatured.Value);
        if (minRating.HasValue)
        {
            var ratingThreshold = minRating.Value;
            q = q.Where(p => p.Reviews.Any() && p.Reviews.Average(r => (double)r.Rating) >= ratingThreshold);
        }
        if (isStockAvailable == true) q = q.Where(p => p.StockQuantity > 0);

        // sorting
        q = sort?.Replace(" ", "").ToLowerInvariant() switch
        {
            "priceasc" => q.OrderBy(p => p.Price),
            "pricedesc" => q.OrderByDescending(p => p.Price),
            "nameasc" => q.OrderBy(p => p.Name),
            "namedesc" => q.OrderByDescending(p => p.Name),
            "createdatasc" => q.OrderBy(p => p.CreatedAt),
            "createdatdesc" => q.OrderByDescending(p => p.CreatedAt),
            "toprated" => q.OrderByDescending(p => p.Reviews.Select(r => (double?)r.Rating).Average() ?? 0),
            "featured" => q.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.UpdatedAt ?? p.CreatedAt),
            _ => q.OrderByDescending(p => p.CreatedAt)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Product> { Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    // Category relationship helpers
    public async Task<bool> CategoryHasChildrenAsync(Guid categoryId)
    {
        return await _db.Categories.AnyAsync(c => c.ParentCategoryId == categoryId);
    }

    public async Task<bool> IsCategoryLinkedToProductsAsync(Guid categoryId)
    {
        return await _db.Products.AnyAsync(p => p.CategoryId == categoryId);
    }

    public async Task DeleteCategoryAsync(Guid categoryId)
    {
        var c = await _db.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
        if (c == null) return;
        // If category has an associated media, let MediaRepository handle file + record deletion
        if (c.MediaId.HasValue)
        {
            try
            {
                var mediaRepo = new MediaRepository(_db);
                await mediaRepo.DeleteByIdAsync(c.MediaId.Value);
            }
            catch
            {
                // ignore and proceed with category deletion
            }
        }

        _db.Categories.Remove(c);
        await _db.SaveChangesAsync();
    }
}
