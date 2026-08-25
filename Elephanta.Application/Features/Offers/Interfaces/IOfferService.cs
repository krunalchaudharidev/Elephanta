using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Application.Common;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Offers.Interfaces;

public interface IOfferService
{
    Task<Offer> AddOfferAsync(Offer offer);
    Task UpdateOfferAsync(Offer offer);
    Task<Offer?> GetOfferByIdAsync(Guid id);
    Task<PagedResult<Offer>> GetOffersAsync(int pageNumber, int pageSize);

    Task<OfferImage> AddImageAsync(OfferImage img);
    Task UpdateImageAsync(OfferImage img);
    Task<OfferImage?> GetImageByIdAsync(Guid id);
    Task<List<OfferImage>> GetImagesByOfferAsync(Guid offerId);
}
