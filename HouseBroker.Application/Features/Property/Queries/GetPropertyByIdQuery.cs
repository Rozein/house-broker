using System.Net;
using HouseBroker.Application.CommonDto;
using HouseBroker.Application.Features.Property.Dto;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Messaging;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Application.Features.Property.Queries;

public record GetPropertyByIdQuery(Guid Id) : IQuery<PropertyResponseDto>;

internal sealed class GetPropertyByIdQueryHandler : IQueryHandler<GetPropertyByIdQuery, PropertyResponseDto>
{
    private readonly IGenericRepository<Domain.Entities.Property> _propertyRepository;
    private readonly IAppSettings _appSettings;


    public GetPropertyByIdQueryHandler(IGenericRepository<Domain.Entities.Property> propertyRepository, IAppSettings appSettings)
    {
        _propertyRepository = propertyRepository;
        _appSettings = appSettings;
    }
    
    public async Task<Response<PropertyResponseDto>> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var propertyRepo = await _propertyRepository.GetAllNoTracking().Where(x => x.Id == request.Id)
            .Include(x => x.Broker).ThenInclude(x => x.User).Include(x => x.Location).ThenInclude(x => x.City)
            .Include(x => x.PropertyAttachments)
            .Select(x =>
                new PropertyResponseDto
                {
                    Id = x.Id,
                    Price = x.Price,
                    PropertyType = x.PropertyType,
                    Location = new LocationDto
                    {
                        CityName = x.Location.City.Name,
                        Area = x.Location.Area,
                        PostalCode = x.Location.PostalCode
                    },
                    IsSold = x.IsSold,
                    Feature = x.Feature,
                    BrokerDetail = new BrokerDetail
                    {
                        FullName = x.Broker.User.FirstName + " " + x.Broker.User.LastName,
                        Email = x.Broker.User.Email,
                        PhoneNumber = x.Broker.User.PhoneNumber,
                        Address = x.Broker.User.Address,
                    },
                    Attachments = x.PropertyAttachments
                        .Select(a => new AttachmentDetails
                        {
                            FileName = a.FileName,
                            ImageUrl = $"{_appSettings.ImageBaseUrl.TrimEnd('/')}/{a.ImageUrl}"
                        })
                        .ToList()                
                }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (propertyRepo == null)
        {
            return Response.Failure<PropertyResponseDto>(
                new Error(HttpStatusCode.NotFound, "This Property does not exist"));
        }
        return propertyRepo;
    }
}