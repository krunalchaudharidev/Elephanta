using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Elephanta.Domain.Constants;
using Elephanta.Application.Features.Catalog.DTOs;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Application.Features.ProductFaqs.DTOs;
using Elephanta.Application.Features.ProductFaqs.Interfaces;
using Elephanta.Application.Common;
using Elephanta.Application.Features.Media.Interfaces;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IProductFaqService _faqService;
    private readonly IMediaService _mediaService;

    public ProductController(IProductService service, IProductFaqService faqService, IMediaService mediaService)
    {
        _service = service;
        _faqService = faqService;
        _mediaService = mediaService;
    }

    // Categories
    /// <summary>
    /// Requires Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPost("categories")]
    public async Task<IActionResult> AddCategory([FromBody] CategoryRequest req)
    {
        var c = new Category
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Slug = req.Slug,
            Description = req.Description,
            MediaId = req.MediaId,
            DisplayOrder = req.DisplayOrder,
            IsActive = req.IsActive,
            ParentCategoryId = req.ParentCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddCategoryAsync(c);

        var resp = new CategoryResponse
        {
            Id = added.Id,
            Name = added.Name,
            Slug = added.Slug,
            Description = added.Description,
            MediaId = added.MediaId,
            DisplayOrder = added.DisplayOrder,
            IsActive = added.IsActive,
            ParentCategoryId = added.ParentCategoryId
        };

        return Ok(new ApiResponse(true, "Category added successfully", added.Id));
    }

    /// <summary>
    /// Requires Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryRequest req)
    {
        var existing = await _service.GetCategoryByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "Category not found"));

        existing.Name = req.Name;
        existing.Slug = req.Slug;
        existing.Description = req.Description;
        existing.MediaId = req.MediaId;
        existing.DisplayOrder = req.DisplayOrder;
        existing.IsActive = req.IsActive;
        existing.ParentCategoryId = req.ParentCategoryId;
        existing.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateCategoryAsync(existing);
        
        return Ok(new ApiResponse(true, "Category updated successfully", existing.Id));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetCategoriesAsync(pageNumber, pageSize);
        var items = paged.Items.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            MediaId = c.MediaId,
            DisplayOrder = c.DisplayOrder,
            IsActive = c.IsActive,
            ParentCategoryId = c.ParentCategoryId,
            ParentCategoryName = null
        }).ToList();

        // Populate ParentCategoryName for items that have a parent
        var parentIds = items.Where(i => i.ParentCategoryId.HasValue).Select(i => i.ParentCategoryId!.Value).Distinct().ToList();
        var parentMap = new Dictionary<Guid, string?>();
        foreach (var pid in parentIds)
        {
            var parent = await _service.GetCategoryByIdAsync(pid);
            parentMap[pid] = parent?.Name;
        }

        foreach (var it in items)
        {
            if (it.ParentCategoryId.HasValue && parentMap.TryGetValue(it.ParentCategoryId.Value, out var pname))
            {
                it.ParentCategoryName = pname;
            }
        }

        var result = new PagedResult<CategoryResponse>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var c = await _service.GetCategoryByIdAsync(id);
        if (c == null) return NotFound();
        var resp = new CategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            MediaId = c.MediaId,
            DisplayOrder = c.DisplayOrder,
            IsActive = c.IsActive,
            ParentCategoryId = c.ParentCategoryId
        };
        if (resp.ParentCategoryId.HasValue)
        {
            var parent = await _service.GetCategoryByIdAsync(resp.ParentCategoryId.Value);
            resp.ParentCategoryName = parent?.Name;
        }

        return Ok(resp);
    }

    /// <summary>
    /// Requires Admin role. Deletes a category after validating it has no child categories or linked products.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var existing = await _service.GetCategoryByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "Category not found"));

        // Check if this category is used as a parent for other categories
        if (await _service.CategoryHasChildrenAsync(id))
        {
            return BadRequest(new ApiResponse(false, "This category is assigned as a parent category. Please remove or reassign its child categories before deleting it."));
        }

        // Check if any product links to this category
        if (await _service.IsCategoryLinkedToProductsAsync(id))
        {
            return BadRequest(new ApiResponse(false, "This category is currently associated with one or more products. Please remove the category from those products before deleting it."));
        }

        // Safe to delete
        await _service.DeleteCategoryAsync(id);
        return Ok(new ApiResponse(true, "Category deleted successfully", id));
    }

    // FAQs
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPost("products/{productId}/faqs")]
    public async Task<IActionResult> AddFaq(Guid productId, [FromBody] ProductFaqRequest req)
    {
        var f = new ProductFaq
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Question = req.Question,
            Answer = req.Answer,
            IsActive = req.IsActive,
            DisplayOrder = req.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _faqService.AddFaqAsync(f);
        var resp = new ProductFaqResponse
        {
            Id = added.Id,
            ProductId = added.ProductId,
            Question = added.Question,
            Answer = added.Answer,
            IsActive = added.IsActive,
            DisplayOrder = added.DisplayOrder
        };

        return CreatedAtAction(nameof(GetFaq), new { id = resp.Id }, resp);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPut("products/faqs/{id}")]
    public async Task<IActionResult> UpdateFaq(Guid id, [FromBody] ProductFaqRequest req)
    {
        var existing = await _faqService.GetFaqByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "FAQ not found"));

        existing.Question = req.Question;
        existing.Answer = req.Answer;
        existing.IsActive = req.IsActive;
        existing.DisplayOrder = req.DisplayOrder;
        existing.UpdatedAt = DateTime.UtcNow;

        await _faqService.UpdateFaqAsync(existing);
        return Ok(new ApiResponse(true, "FAQ updated successfully", existing.Id));
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpDelete("products/faqs/{id}")]
    public async Task<IActionResult> DeleteFaq(Guid id)
    {
        var existing = await _faqService.GetFaqByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "FAQ not found"));

        await _faqService.DeleteFaqAsync(id);
        return Ok(new ApiResponse(true, "FAQ deleted successfully", id));
    }

    [HttpGet("products/{productId}/faqs")]
    public async Task<IActionResult> GetFaqs(Guid productId)
    {
        var list = await _faqService.GetFaqsByProductAsync(productId);
        var resp = list.Select(f => new ProductFaqResponse
        {
            Id = f.Id,
            ProductId = f.ProductId,
            Question = f.Question,
            Answer = f.Answer,
            IsActive = f.IsActive,
            DisplayOrder = f.DisplayOrder
        }).ToList();

        return Ok(resp);
    }

    [HttpGet("products/faqs/{id}")]
    public async Task<IActionResult> GetFaq(Guid id)
    {
        var f = await _faqService.GetFaqByIdAsync(id);
        if (f == null) return NotFound();

        var resp = new ProductFaqResponse
        {
            Id = f.Id,
            ProductId = f.ProductId,
            Question = f.Question,
            Answer = f.Answer,
            IsActive = f.IsActive,
            DisplayOrder = f.DisplayOrder
        };

        return Ok(resp);
    }

    // Products
    /// <summary>
    /// Requires Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPost("products")]
    public async Task<IActionResult> AddProduct([FromBody] ProductRequest req)
    {
        var p = new Product
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Slug = req.Slug,
            SKU = req.SKU,
            ShortDescription = req.ShortDescription,
            Description = req.Description,
            Price = req.Price,
            CompareAtPrice = req.CompareAtPrice,
            StockQuantity = req.StockQuantity,
            IsActive = req.IsActive,
            IsFeatured = req.IsFeatured,
            CategoryId = req.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddProductAsync(p);

        // If request contains ImageIds, create ProductImage records
        if (req.ImageIds != null && req.ImageIds.Count > 0)
        {
            int order = 0;
            foreach (var imgId in req.ImageIds)
            {
                var pi = new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = added.Id,
                    MediaId = imgId,
                    IsPrimary = (req.PrimaryImageId.HasValue && req.PrimaryImageId.Value == imgId) || order == 0,
                    DisplayOrder = order,
                    CreatedAt = DateTime.UtcNow
                };

                await _service.AddImageAsync(pi);
                order++;
            }
        }

        var resp = new ProductResponse
        {
            Id = added.Id,
            Name = added.Name,
            Slug = added.Slug,
            SKU = added.SKU,
            ShortDescription = added.ShortDescription,
            Description = added.Description,
            Price = added.Price,
            CompareAtPrice = added.CompareAtPrice,
            StockQuantity = added.StockQuantity,
            IsActive = added.IsActive,
            IsFeatured = added.IsFeatured,
            CategoryId = added.CategoryId,
            ImageIds = (req.ImageIds != null && req.ImageIds.Count > 0) ? req.ImageIds : (added.Images?.Select(i => i.Id).ToList() ?? new List<Guid>()),
            ReviewCount = added.Reviews?.Count ?? 0,
            PrimaryImageId = req.PrimaryImageId.HasValue ? req.PrimaryImageId : added.Images?.FirstOrDefault(i => i.IsPrimary && i.MediaId.HasValue)?.MediaId
        };

        return CreatedAtAction(nameof(GetProduct), new { id = resp.Id }, resp);
    }

    /// <summary>
    /// Requires Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductUpdateRequest req)
    {
        var existing = await _service.GetProductByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "Product not found"));

        // Apply partial updates for scalar fields
        if (req.Name != null) existing.Name = req.Name;
        if (req.Slug != null) existing.Slug = req.Slug;
        if (req.SKU != null) existing.SKU = req.SKU;
        if (req.ShortDescription != null) existing.ShortDescription = req.ShortDescription;
        if (req.Description != null) existing.Description = req.Description;
        if (req.Price.HasValue) existing.Price = req.Price.Value;
        if (req.CompareAtPrice.HasValue) existing.CompareAtPrice = req.CompareAtPrice;
        if (req.StockQuantity.HasValue) existing.StockQuantity = req.StockQuantity.Value;
        if (req.IsActive.HasValue) existing.IsActive = req.IsActive.Value;
        if (req.IsFeatured.HasValue) existing.IsFeatured = req.IsFeatured.Value;
        if (req.CategoryId.HasValue) existing.CategoryId = req.CategoryId.Value;

        // Handle image additions: create ProductImage records linking this product to media
        if (req.ImageIds != null && req.ImageIds.Count > 0)
        {
            var currentImages = await _service.GetImagesByProductAsync(existing.Id);
            var order = currentImages?.Count ?? 0;
            var existingMediaIds = currentImages?.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value).ToHashSet() ?? new HashSet<Guid>();

            foreach (var mediaId in req.ImageIds.Distinct())
            {
                if (existingMediaIds.Contains(mediaId)) continue;

                var pi = new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = existing.Id,
                    MediaId = mediaId,
                    IsPrimary = order == 0,
                    DisplayOrder = order,
                    CreatedAt = DateTime.UtcNow
                };

                await _service.AddImageAsync(pi);
                existingMediaIds.Add(mediaId);
                order++;
            }

            // Handle primary image selection by media id
            if (req.PrimaryImageId.HasValue)
            {
                // First, unset any existing primary images for this product
                var pImg = currentImages.FirstOrDefault(i => i.IsPrimary);
                if (pImg != null)
                {
                    pImg.IsPrimary = false;
                    pImg.UpdatedAt = DateTime.UtcNow;
                    await _service.UpdateImageAsync(pImg);
                }

                // Then set the selected image (by media id) as primary
                var img = await _service.GetImageByMediaIdAsync(req.PrimaryImageId.Value);
                if (img != null)
                {
                    img.IsPrimary = true;
                    img.UpdatedAt = DateTime.UtcNow;
                    await _service.UpdateImageAsync(img);
                }
            }
        }

        existing.UpdatedAt = DateTime.UtcNow;
        await _service.UpdateProductAsync(existing);

        return Ok(new ApiResponse(true, "Product updated successfully", existing.Id));
    }

    /// <summary>
    /// Requires Admin role. Soft-delete a product by setting IsDeleted and DeletedAt.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpDelete("products/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _service.GetProductByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "Product not found"));

        existing.IsDeleted = true;
        existing.DeletedAt = DateTime.UtcNow;

        await _service.UpdateProductAsync(existing);

        return Ok(new ApiResponse(true, "Product deleted successfully", id));
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetProductsAsync(pageNumber, pageSize);
        var items = paged.Items.Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            SKU = p.SKU,
            ShortDescription = p.ShortDescription,
            Description = p.Description,
            Price = p.Price,
            CompareAtPrice = p.CompareAtPrice,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            IsFeatured = p.IsFeatured,
            CategoryId = p.CategoryId,
            ImageIds = p.Images?.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value).ToList() ?? new List<Guid>(),
            ReviewCount = p.Reviews?.Count ?? 0,
            CategoryName = p.Category?.Name,
            PrimaryImageId = p.Images?.FirstOrDefault(i => i.IsPrimary && i.MediaId.HasValue)?.MediaId
        }).ToList();

        var result = new PagedResult<ProductResponse>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }

    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var p = await _service.GetProductByIdAsync(id);
        if (p == null) return NotFound();
        var resp = new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            SKU = p.SKU,
            ShortDescription = p.ShortDescription,
            Description = p.Description,
            Price = p.Price,
            CompareAtPrice = p.CompareAtPrice,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            IsFeatured = p.IsFeatured,
            CategoryId = p.CategoryId,
            ImageIds = p.Images?.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value).ToList() ?? new List<Guid>(),
            ReviewCount = p.Reviews?.Count ?? 0,
            CategoryName = p.Category?.Name,
            PrimaryImageId = p.Images?.FirstOrDefault(i => i.IsPrimary && i.MediaId.HasValue)?.MediaId
        };

        return Ok(resp);
    }

    // Images
    /// <summary>
    /// Requires Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPost("products/{productId}/images")]
    public async Task<IActionResult> AddImage(Guid productId, [FromBody] ProductImageRequest req)
    {
        var img = new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            MediaId = req.MediaId,
            IsPrimary = req.IsPrimary,
            DisplayOrder = req.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddImageAsync(img);
        var resp = new ProductImageResponse { Id = added.Id, MediaId = added.MediaId, IsPrimary = added.IsPrimary, DisplayOrder = added.DisplayOrder };
        return CreatedAtAction(nameof(GetImage), new { id = resp.Id }, resp);
    }

    /// <summary>
    /// Requires Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPut("products/images/{id}")]
    public async Task<IActionResult> UpdateImage(Guid id, [FromBody] ProductImageRequest req)
    {
        var existing = await _service.GetImageByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "Image not found"));
        existing.MediaId = req.MediaId;
        existing.IsPrimary = req.IsPrimary;
        existing.DisplayOrder = req.DisplayOrder;
        existing.UpdatedAt = DateTime.UtcNow;
        await _service.UpdateImageAsync(existing);
        return Ok(new ApiResponse(true, "Image updated successfully", existing.Id));
    }

    [HttpGet("products/{productId}/images")]
    public async Task<IActionResult> GetImages(Guid productId)
    {
        var list = await _service.GetImagesByProductAsync(productId);
        var resp = list.Select(i => new ProductImageResponse { Id = i.Id, MediaId = i.MediaId, IsPrimary = i.IsPrimary, DisplayOrder = i.DisplayOrder }).ToList();
        return Ok(resp);
    }

    [HttpGet("products/images/{id}")]
    public async Task<IActionResult> GetImage(Guid id)
    {
        var i = await _service.GetImageByIdAsync(id);
        if (i == null) return NotFound();
        var resp = new ProductImageResponse { Id = i.Id, MediaId = i.MediaId, IsPrimary = i.IsPrimary, DisplayOrder = i.DisplayOrder };
        return Ok(resp);
    }

    // Reviews
    /// <summary>
    /// Requires authenticated User or Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [ApiExplorerSettings(GroupName = "User")]
    [HttpPost("products/{productId}/reviews")]
    public async Task<IActionResult> AddReview(Guid productId, [FromBody] ProductReviewRequest req)
    {
        var r = new ProductReview { Id = Guid.NewGuid(), ProductId = productId, Rating = req.Rating, Comment = req.Comment, CreatedAt = DateTime.UtcNow };
        var added = await _service.AddReviewAsync(r);
        var resp = new ProductReviewResponse { Id = added.Id, Rating = added.Rating, Comment = added.Comment, UserId = added.UserId };
        return CreatedAtAction(nameof(GetReview), new { id = resp.Id }, resp);
    }

    /// <summary>
    /// Requires authenticated User or Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [ApiExplorerSettings(GroupName = "User")]
    [HttpPut("products/reviews/{id}")]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] ProductReviewRequest req)
    {
        var existing = await _service.GetReviewByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "Review not found"));
        existing.Rating = req.Rating;
        existing.Comment = req.Comment;
        existing.UpdatedAt = DateTime.UtcNow;
        await _service.UpdateReviewAsync(existing);
        return Ok(new ApiResponse(true, "Review updated successfully", existing.Id));
    }

    [HttpGet("products/{productId}/reviews")]
    public async Task<IActionResult> GetReviews(Guid productId)
    {
        var list = await _service.GetReviewsByProductAsync(productId);
        var resp = list.Select(r => new ProductReviewResponse { Id = r.Id, Rating = r.Rating, Comment = r.Comment, UserId = r.UserId }).ToList();
        return Ok(resp);
    }

    [HttpGet("products/reviews/{id}")]
    public async Task<IActionResult> GetReview(Guid id)
    {
        var r = await _service.GetReviewByIdAsync(id);
        if (r == null) return NotFound();
        var resp = new ProductReviewResponse { Id = r.Id, Rating = r.Rating, Comment = r.Comment, UserId = r.UserId };
        return Ok(resp);
    }

    // Search
    [HttpGet("products/search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] Guid? categoryId, [FromQuery] bool? isActive, [FromQuery] string? sort, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.SearchProductsAsync(name, minPrice, maxPrice, categoryId, sort, isActive, pageNumber, pageSize);
        var items = paged.Items.Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            SKU = p.SKU,
            ShortDescription = p.ShortDescription,
            Description = p.Description,
            Price = p.Price,
            CompareAtPrice = p.CompareAtPrice,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            IsFeatured = p.IsFeatured,
            CategoryId = p.CategoryId,
            ImageIds = p.Images?.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value).ToList() ?? new List<Guid>(),
            ReviewCount = p.Reviews?.Count ?? 0,
            CategoryName = p.Category?.Name,
            PrimaryImageId = p.Images?.FirstOrDefault(i => i.IsPrimary && i.MediaId.HasValue)?.MediaId
        }).ToList();

        var result = new PagedResult<ProductResponse>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }
}
