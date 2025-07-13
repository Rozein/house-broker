using HouseBroker.Application.Features.Property.Dto;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Messaging;
using HouseBroker.Application.Pagination;
using HouseBroker.Application.Property.Dto;
using HouseBroker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Application.Property.Queries;

public record GetPropertyListQuery : IQueryPaginated<PropertyResponseDto>
{
    public Guid? CityId { get; init; }
    public string? Area { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public PropertyType? PropertyType { get; init; }

    public string? PostalCode { get; init; }

    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

internal sealed class GetPropertyListQueryHandler : IQueryHandlerPaginated<GetPropertyListQuery, PropertyResponseDto>
{
    private readonly IGenericRepository<Domain.Entities.Property> _propertyRepository;
    private readonly IAppSettings _appSettings;


    public GetPropertyListQueryHandler(IGenericRepository<Domain.Entities.Property> propertyRepository, IAppSettings appSettings)
    {
        _propertyRepository = propertyRepository;
        _appSettings = appSettings;
    }
    public async Task<Response<PaginatedList<PropertyResponseDto>>> Handle(GetPropertyListQuery request, CancellationToken cancellationToken)
    {
        var query = _propertyRepository.GetAllNoTracking()
            .Include(p => p.Broker).ThenInclude(b => b.User)
            .Include(p => p.Location).ThenInclude(l => l.City).Include(a => a.PropertyAttachments)
            .AsQueryable();

        // 🧭 Apply Filters
        if (request.CityId.HasValue)
            query = query.Where(p => p.Location.CityId == request.CityId);

        if (!string.IsNullOrWhiteSpace(request.Area))
            query = query.Where(p => p.Location.Area.Contains(request.Area));

        if (!string.IsNullOrWhiteSpace(request.PostalCode))
            query = query.Where(p => p.Location.PostalCode == request.PostalCode);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        if (request.PropertyType.HasValue)
            query = query.Where(p => p.PropertyType == request.PropertyType.Value);

        var properties = query.Select(p => new PropertyResponseDto
        {
            Id = p.Id,
            Price = p.Price,
            PropertyType = p.PropertyType,
            Feature = p.Feature,
            IsSold = p.IsSold,
            Location = new LocationDto
            {
                CityName = p.Location.City.Name,
                Area = p.Location.Area,
                PostalCode = p.Location.PostalCode
            },
            BrokerDetail = new BrokerDetail
            {
                FullName = p.Broker.User.FirstName + " " + p.Broker.User.LastName,
                Email = p.Broker.User.Email,
                PhoneNumber = p.Broker.User.PhoneNumber,
                Address = p.Broker.User.Address,
            }, 
            Attachments = p.PropertyAttachments
                .Select(a => new AttachmentDetails
                {
                    FileName = a.FileName,
                    ImageUrl =  $"{_appSettings.ImageBaseUrl.TrimEnd('/')}/{a.ImageUrl}"
                })
                .ToList()
        });

        var paginatedResult = await properties.PaginatedListAsync(request.PageNumber, request.PageSize);
        return paginatedResult;

    }
}