using HouseBroker.Application.CommonDto;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Messaging;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Domain.Entities;
using HouseBroker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Application.Features.Property.Command.UploadPropertyAttachment;

public class UploadPropertyAttachmentCommand : ICommand<string>
{
    public Guid PropertyId { get; set; }
    public string FileName { get; set; }
    public Stream FileContent { get; set; } = default!;

}

internal sealed class UploadPropertyAttachmentCommandHandler : ICommandHandler<UploadPropertyAttachmentCommand, string>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly IGenericRepository<Domain.Entities.Property>  _propertyRepository;
    private readonly IGenericRepository<Domain.Entities.PropertyAttachments>  _propertyAttachment;
    private readonly IAppSettings _appSettings;


    public UploadPropertyAttachmentCommandHandler(IImageStorageService imageStorageService, IGenericRepository<Domain.Entities.Property> propertyRepository, IGenericRepository<PropertyAttachments> propertyAttachment, IAppSettings appSettings)
    {
        _imageStorageService = imageStorageService;
        _propertyRepository = propertyRepository;
        _propertyAttachment = propertyAttachment;
        _appSettings = appSettings;
    }

    public async  Task<Response<string>> Handle(UploadPropertyAttachmentCommand request, CancellationToken cancellationToken)
    {
        var propertyRepository =await _propertyRepository.GetAll().FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken: cancellationToken);
        if (propertyRepository is null)
            return Response.Failure<string>(HttpContextError.NotFound("This Property does not exist"));
        
        var imageRelativePath = await _imageStorageService.SaveImageAsync(request.PropertyId, request.FileName, request.FileContent);

        var propertyAttachment = new PropertyAttachments
        {
            PropertyId = request.PropertyId,
            ImageUrl =  imageRelativePath,
            FileName = request.FileName,
            AttachmentType = AttachmentType.File,
        };
        await _propertyAttachment.InsertAsync(propertyAttachment);

        var fullUrl = $"{_appSettings.ImageBaseUrl.TrimEnd('/')}/{imageRelativePath}";
        return  fullUrl;
    }
}