using HouseBroker.Application.CommonDto;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Messaging;
using HouseBroker.Domain.Entities;

namespace HouseBroker.Application.Features.Property.Command.RemovePropertyAttachment;

public record RemovePropertyAttachmentCommand(Guid Id) : ICommand<string>;

internal sealed class RemovePropertyAttachmentCommandHandler : ICommandHandler<RemovePropertyAttachmentCommand, string>
{
    private readonly IGenericRepository<Domain.Entities.PropertyAttachments> _propertyAttachmentRepository;

    public RemovePropertyAttachmentCommandHandler(IGenericRepository<PropertyAttachments> propertyAttachmentRepository)
    {
        _propertyAttachmentRepository = propertyAttachmentRepository;
    }
    public async Task<Response<string>> Handle(RemovePropertyAttachmentCommand request, CancellationToken cancellationToken)
    {
        // Find the attachment by Id
        var attachment = await _propertyAttachmentRepository.GetByIdAsync(request.Id);
        
        if (attachment is null)
        {
            return Response.Failure<string>(HttpContextError.NotFound("Attachment does not exist"));
        }
        
        await _propertyAttachmentRepository.DeleteAsync(attachment);

        return Response.Success("Attachment deleted successfully.");
    }
}
