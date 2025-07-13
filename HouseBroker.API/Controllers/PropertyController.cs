using HouseBroker.API.Attributes;
using HouseBroker.Application.CommonDto;
using HouseBroker.Application.Features.Property.Command.CreateProperty;
using HouseBroker.Application.Features.Property.Command.DeleteProperty;
using HouseBroker.Application.Features.Property.Command.RemovePropertyAttachment;
using HouseBroker.Application.Features.Property.Command.UploadPropertyAttachment;
using HouseBroker.Application.Features.Property.Dto;
using HouseBroker.Application.Features.Property.Queries;
using HouseBroker.Application.Pagination;
using HouseBroker.Application.Property.Command.UpdateProperty;
using HouseBroker.Application.Property.Queries;
using HouseBroker.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HouseBroker.API.Controllers;

public class PropertyController : APIControllerBase
{
    /// <summary>
    /// Creates a new property listing.
    /// </summary>
    /// <param name="command">The command containing property details from the request body.</param>
    /// <returns>The ID of the created property wrapped in a response object.</returns>
    [HttpPost("create")]
    [RoleAuthorization(RoleEnum.Broker)]
    public async Task<Response<Guid>> CreateProperty([FromBody] CreatePropertyCommand command)
    {
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Updates an existing property using the provided data.
    /// </summary>
    /// <param name="command">The command containing updated property information.</param>
    /// <returns>A response containing the ID of the updated property.</returns>
    [HttpPut("update")]
    [RoleAuthorization(RoleEnum.Broker)]
    public async Task<Response<Guid>> UpdateProperty([FromBody] UpdatePropertyCommand command)
    {
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Retrieves a property by its unique identifier.
    /// </summary>
    /// <param name="query">The query object containing the property ID.</param>
    /// <returns>A response containing the property details if found, otherwise an error.</returns>
    [HttpGet]
    [RoleAuthorization(RoleEnum.Broker,RoleEnum.HouseSeeker)]
    public async Task<Response<PropertyResponseDto>> GetPropertyById([FromQuery]GetPropertyByIdQuery query)
    {
        return await Mediator.Send(query);
    }
    
    /// <summary>
    /// Retrieves a paginated list of properties based on search criteria and filters.
    /// </summary>
    /// <param name="query">Query parameters including filters, page number, and page size.</param>
    /// <returns>A paginated response containing a list of PropertyResponseDto.</returns>
    [HttpGet("list")]
    [RoleAuthorization(RoleEnum.Broker,RoleEnum.HouseSeeker)]
    public async Task<Response<PaginatedList<PropertyResponseDto>>> GetPropertyList([FromQuery]GetPropertyListQuery query)
    {
        return await Mediator.Send(query);
    }
    
    /// <summary>
    /// Deletes a property based on the provided DeletePropertyCommand.
    /// </summary>
    /// <param name="command">The command containing the ID or criteria to identify the property to delete.</param>
    /// <returns>A Response indicating success or failure of the delete operation, with an optional message.</returns>
    [HttpDelete]
    [RoleAuthorization(RoleEnum.Broker)]
    public async Task<Response<string>> DeleteProperty([FromQuery]DeletePropertyCommand command)
    {
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Uploads an image for a specific property.
    /// </summary>
    /// <param name="propertyId">The ID of the property to associate the image with.</param>
    /// <param name="imageFile">The uploaded image file (multipart/form-data).</param>
    /// <returns>Returns the uploaded image URL if successful.</returns>
    [HttpPost("upload/{propertyId}")]
    [RoleAuthorization(RoleEnum.Broker)]
    public async Task<Response<string>> UploadPropertyImage([FromRoute(Name = "propertyId")] Guid propertyId, IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return global::Response.Failure<string>(HttpContextError.NotFound("This Property does not exist"));

        using var stream = imageFile.OpenReadStream();

        var command = new UploadPropertyAttachmentCommand
        {
            PropertyId = propertyId,
            FileName = imageFile.FileName,
            FileContent = stream // pass the stream
        };

        return await Mediator.Send(command);
    }
    /// <summary>
    /// Soft deletes a property attachment by its ID.
    /// </summary>
    /// <param name="id">The ID of the property attachment to delete.</param>
    /// <returns>Returns a Response<string> indicating success or failure.</returns>
    [HttpDelete("attachment/{id}")]
    [RoleAuthorization(RoleEnum.Broker)]
    public async Task<Response<string>> DeletePropertyAttachment([FromRoute] Guid id)
    {
        var response = await Mediator.Send(new RemovePropertyAttachmentCommand(id));

        return response;
    }
}