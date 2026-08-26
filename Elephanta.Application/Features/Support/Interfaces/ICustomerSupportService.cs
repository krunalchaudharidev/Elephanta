using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Support.Interfaces;

public interface ICustomerSupportService
{
    Task<CustomerSupportRequest> AddAsync(CustomerSupportRequest req);
    Task UpdateAsync(CustomerSupportRequest req);
    Task DeleteAsync(Guid id);
    Task<CustomerSupportRequest?> GetByIdAsync(Guid id);
    Task<List<CustomerSupportRequest>> GetByUserAsync(Guid userId);
    Task<Elephanta.Application.Common.PagedResult<CustomerSupportRequest>> GetAllAsync(int pageNumber, int pageSize);
}
