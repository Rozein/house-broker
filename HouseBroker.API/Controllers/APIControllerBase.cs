using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HouseBroker.API.Controllers;

[ApiController]
[EnableCors("AllowedOrigins")]
[Route("api/[controller]")]
public abstract class APIControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
