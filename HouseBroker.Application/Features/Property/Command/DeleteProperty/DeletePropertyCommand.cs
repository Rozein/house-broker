using HouseBroker.Application.CommonDto;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Messaging;
using HouseBroker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HouseBroker.Application.Features.Property.Command.DeleteProperty;

public record DeletePropertyCommand(Guid Id) : ICommand<string>
{
}

internal sealed class DeletePropertyCommandHandler : ICommandHandler<DeletePropertyCommand, string>
{
    private readonly IGenericRepository<Domain.Entities.Property> _propertyRepository;
    private readonly IGenericRepository<Domain.Entities.PropertyAttachments> _propertyAttachmentRepository;
    private readonly ILogger<DeletePropertyCommandHandler> _logger;


    public DeletePropertyCommandHandler(IGenericRepository<Domain.Entities.Property> propertyRepository, IGenericRepository<PropertyAttachments> propertyAttachmentRepository, ILogger<DeletePropertyCommandHandler> logger)
    {
        _propertyRepository = propertyRepository;
        _propertyAttachmentRepository = propertyAttachmentRepository;
        _logger = logger;
    }

    
    public async Task<Response<string>> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting property deletion process for PropertyId: {PropertyId}", request.Id);
        var propertyRepo = await _propertyRepository.GetByIdAsync(request.Id);
        if (propertyRepo == null)
        {
            _logger.LogWarning("Property not found with Id: {PropertyId}", request.Id);
            return Response.Failure<string>(HttpContextError.NotFound("This Property does not exist"));
        }
        await _propertyRepository.DeleteAsync(propertyRepo);
        _logger.LogDebug("Property deleted successfully. PropertyId: {PropertyId}", request.Id);

        // Couldn't use this as it is harder to moq this ExecuteDeleteAsync
        //MockQueryable can't mock EF Core's bulk operations
        // await _propertyAttachmentRepository
        //     .GetAll()
        //     .Where(a => a.PropertyId == request.Id)
        //     .ExecuteDeleteAsync(cancellationToken);
        
        // Get attachments first, then delete them individually
        var attachments = await _propertyAttachmentRepository
            .GetAll()
            .Where(a => a.PropertyId == request.Id)
            .ToListAsync(cancellationToken);
        
        attachments.ForEach(async attachment => 
            await _propertyAttachmentRepository.DeleteAsync(attachment));
        
        _logger.LogInformation("Successfully deleted attachments");

        
        return Response.Success("Property and its attachments deleted successfully.");
    }
}