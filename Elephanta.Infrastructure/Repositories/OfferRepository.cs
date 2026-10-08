using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elephanta.Application.Common;
using Elephanta.Application.Features.Offers.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Repositories;

public class OfferRepository : IOfferRepository
{
    private readonly ElephantaDbContext _db;

    public OfferRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<Offer> AddOfferAsync(Offer offer)
    {
        _db.Offers.Add(offer);
        await _db.SaveChangesAsync();
        return offer;
    }

    public async Task UpdateOfferAsync(Offer offer)
    {
        _db.Offers.Update(offer);
        await _db.SaveChangesAsync();
    }

    public async Task<Offer?> GetOfferByIdAsync(Guid id)
    {
        return await _db.Offers.Include(o => o.Images).FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<PagedResult<Offer>> GetOffersAsync(int pageNumber, int pageSize)
    {
        // Exclude soft-deleted offers
        var q = _db.Offers.Include(o => o.Images).Where(o => !o.IsDeleted).AsQueryable();
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(o => o.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Offer> { Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    public async Task<PagedResult<Offer>> SearchOffersAsync(string? name, string? code, string? sort, bool? isActive, int pageNumber, int pageSize)
    {
        var q = _db.Offers
            .Include(o => o.Images)
            .Where(o => !o.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name)) q = q.Where(o => o.Name.Contains(name));
        if (!string.IsNullOrWhiteSpace(code)) q = q.Where(o => o.Code.Contains(code));
        if (isActive.HasValue) q = q.Where(o => o.IsActive == isActive.Value);

        q = sort?.ToLower() switch
        {
            "nameasc" => q.OrderBy(o => o.Name),
            "namedesc" => q.OrderByDescending(o => o.Name),
            "startdateasc" => q.OrderBy(o => o.StartDate),
            "startdatedesc" => q.OrderByDescending(o => o.StartDate),
            "enddateasc" => q.OrderBy(o => o.EndDate),
            "enddatedesc" => q.OrderByDescending(o => o.EndDate),
            _ => q.OrderByDescending(o => o.CreatedAt)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Offer> { Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    public async Task<OfferImage> AddImageAsync(OfferImage img)
    {
        _db.OfferImages.Add(img);
        await _db.SaveChangesAsync();
        return img;
    }

    public async Task UpdateImageAsync(OfferImage img)
    {
        _db.OfferImages.Update(img);
        await _db.SaveChangesAsync();
    }

    public async Task<OfferImage?> GetImageByIdAsync(Guid id)
    {
        return await _db.OfferImages.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<OfferImage>> GetImagesByOfferAsync(Guid offerId)
    {
        return await _db.OfferImages.Where(i => i.OfferId == offerId).ToListAsync();
    }

    public async Task DeleteImageAsync(Guid id)
    {
        var img = await _db.OfferImages.FirstOrDefaultAsync(i => i.Id == id);
        if (img == null) return;
        _db.OfferImages.Remove(img);
        await _db.SaveChangesAsync();
    }

    public async Task<OfferImage?> GetImageByMediaIdAsync(Guid mediaId)
    {
        return await _db.OfferImages.FirstOrDefaultAsync(i => i.MediaId == mediaId);
    }
}
