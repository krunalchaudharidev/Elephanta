using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Application.Common;
using Elephanta.Application.Features.Offers.Interfaces;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _repo;

    public OfferService(IOfferRepository repo)
    {
        _repo = repo;
    }

    public Task<Offer> AddOfferAsync(Offer offer) => _repo.AddOfferAsync(offer);
    public Task UpdateOfferAsync(Offer offer) => _repo.UpdateOfferAsync(offer);
    public Task<Offer?> GetOfferByIdAsync(Guid id) => _repo.GetOfferByIdAsync(id);
    public Task<PagedResult<Offer>> GetOffersAsync(int pageNumber, int pageSize) => _repo.GetOffersAsync(pageNumber, pageSize);

    public Task<OfferImage> AddImageAsync(OfferImage img) => _repo.AddImageAsync(img);
    public Task UpdateImageAsync(OfferImage img) => _repo.UpdateImageAsync(img);
    public Task<OfferImage?> GetImageByIdAsync(Guid id) => _repo.GetImageByIdAsync(id);
    public Task<List<OfferImage>> GetImagesByOfferAsync(Guid offerId) => _repo.GetImagesByOfferAsync(offerId);
}
