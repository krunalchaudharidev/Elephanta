using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Elephanta.Application.Features.Catalog.DTOs;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Domain.Entities;
using System.Collections.Generic;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    // Categories
    [Authorize]
    [HttpPost("categories")]
    public async Task<IActionResult> AddCategory([FromBody] CategoryRequest req)
    {
        var c = new Category
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Slug = req.Slug,
            Description = req.Description,
            ImageUrl = req.ImageUrl,
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
            ImageUrl = added.ImageUrl,
            DisplayOrder = added.DisplayOrder,
            IsActive = added.IsActive,
            ParentCategoryId = added.ParentCategoryId
        };

        return CreatedAtAction(nameof(GetCategory), new { id = resp.Id }, resp);
    }

    [Authorize]
    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryRequest req)
    {
        var existing = await _service.GetCategoryByIdAsync(id);
        if (existing == null) return NotFound();

        existing.Name = req.Name;
        existing.Slug = req.Slug;
        existing.Description = req.Description;
        existing.ImageUrl = req.ImageUrl;
        existing.DisplayOrder = req.DisplayOrder;
        existing.IsActive = req.IsActive;
        existing.ParentCategoryId = req.ParentCategoryId;
        existing.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateCategoryAsync(existing);
        return NoContent();
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
            ImageUrl = c.ImageUrl,
            DisplayOrder = c.DisplayOrder,
            IsActive = c.IsActive,
            ParentCategoryId = c.ParentCategoryId
        }).ToList();

        var result = new Elephanta.Application.Common.PagedResult<CategoryResponse>
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
            ImageUrl = c.ImageUrl,
            DisplayOrder = c.DisplayOrder,
            IsActive = c.IsActive,
            ParentCategoryId = c.ParentCategoryId
        };
        return Ok(resp);
    }

    // Products
    [Authorize]
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
            ImageIds = added.Images?.Select(i => i.Id).ToList() ?? new List<Guid>(),
            ReviewCount = added.Reviews?.Count ?? 0
        };

        return CreatedAtAction(nameof(GetProduct), new { id = resp.Id }, resp);
    }

    [Authorize]
    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductRequest req)
    {
        var existing = await _service.GetProductByIdAsync(id);
        if (existing == null) return NotFound();

        existing.Name = req.Name;
        existing.Slug = req.Slug;
        existing.SKU = req.SKU;
        existing.ShortDescription = req.ShortDescription;
        existing.Description = req.Description;
        existing.Price = req.Price;
        existing.CompareAtPrice = req.CompareAtPrice;
        existing.StockQuantity = req.StockQuantity;
        existing.IsActive = req.IsActive;
        existing.IsFeatured = req.IsFeatured;
        existing.CategoryId = req.CategoryId;
        existing.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateProductAsync(existing);
        return NoContent();
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
            ImageIds = p.Images?.Select(i => i.Id).ToList() ?? new List<Guid>(),
            ReviewCount = p.Reviews?.Count ?? 0
        }).ToList();

        var result = new Elephanta.Application.Common.PagedResult<ProductResponse>
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
            ImageIds = p.Images?.Select(i => i.Id).ToList() ?? new List<Guid>(),
            ReviewCount = p.Reviews?.Count ?? 0
        };

        return Ok(resp);
    }

    // Images
    [Authorize]
    [HttpPost("products/{productId}/images")]
    public async Task<IActionResult> AddImage(Guid productId, [FromBody] ProductImageRequest req)
    {
        var img = new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ImageUrl = req.ImageUrl,
            IsPrimary = req.IsPrimary,
            DisplayOrder = req.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddImageAsync(img);
        var resp = new ProductImageResponse { Id = added.Id, ImageUrl = added.ImageUrl, IsPrimary = added.IsPrimary, DisplayOrder = added.DisplayOrder };
        return CreatedAtAction(nameof(GetImage), new { id = resp.Id }, resp);
    }

    [Authorize]
    [HttpPut("products/images/{id}")]
    public async Task<IActionResult> UpdateImage(Guid id, [FromBody] ProductImageRequest req)
    {
        var existing = await _service.GetImageByIdAsync(id);
        if (existing == null) return NotFound();
        existing.ImageUrl = req.ImageUrl;
        existing.IsPrimary = req.IsPrimary;
        existing.DisplayOrder = req.DisplayOrder;
        existing.UpdatedAt = DateTime.UtcNow;
        await _service.UpdateImageAsync(existing);
        return NoContent();
    }

    [HttpGet("products/{productId}/images")]
    public async Task<IActionResult> GetImages(Guid productId)
    {
        var list = await _service.GetImagesByProductAsync(productId);
        var resp = list.Select(i => new ProductImageResponse { Id = i.Id, ImageUrl = i.ImageUrl, IsPrimary = i.IsPrimary, DisplayOrder = i.DisplayOrder }).ToList();
        return Ok(resp);
    }

    [HttpGet("products/images/{id}")]
    public async Task<IActionResult> GetImage(Guid id)
    {
        var i = await _service.GetImageByIdAsync(id);
        if (i == null) return NotFound();
        var resp = new ProductImageResponse { Id = i.Id, ImageUrl = i.ImageUrl, IsPrimary = i.IsPrimary, DisplayOrder = i.DisplayOrder };
        return Ok(resp);
    }

    // Reviews
    [Authorize]
    [HttpPost("products/{productId}/reviews")]
    public async Task<IActionResult> AddReview(Guid productId, [FromBody] ProductReviewRequest req)
    {
        var r = new ProductReview { Id = Guid.NewGuid(), ProductId = productId, Rating = req.Rating, Comment = req.Comment, CreatedAt = DateTime.UtcNow };
        var added = await _service.AddReviewAsync(r);
        var resp = new ProductReviewResponse { Id = added.Id, Rating = added.Rating, Comment = added.Comment, UserId = added.UserId };
        return CreatedAtAction(nameof(GetReview), new { id = resp.Id }, resp);
    }

    [Authorize]
    [HttpPut("products/reviews/{id}")]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] ProductReviewRequest req)
    {
        var existing = await _service.GetReviewByIdAsync(id);
        if (existing == null) return NotFound();
        existing.Rating = req.Rating;
        existing.Comment = req.Comment;
        existing.UpdatedAt = DateTime.UtcNow;
        await _service.UpdateReviewAsync(existing);
        return NoContent();
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
    public async Task<IActionResult> SearchProducts([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] Guid? categoryId, [FromQuery] string? sort, [FromQuery] bool? isActive, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
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
            ImageIds = p.Images?.Select(i => i.Id).ToList() ?? new List<Guid>(),
            ReviewCount = p.Reviews?.Count ?? 0
        }).ToList();

        var result = new Elephanta.Application.Common.PagedResult<ProductResponse>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }
}
