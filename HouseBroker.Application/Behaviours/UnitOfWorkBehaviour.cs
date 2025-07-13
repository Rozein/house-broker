using System.Net;
using System.Transactions;
using HouseBroker.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HouseBroker.Application.Behaviours;

public sealed class UnitOfWorkBehavior<TRequest, TResponse>(
    IUnitOfWorkService unitOfWorkService,
    ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWorkService _unitOfWorkService = unitOfWorkService;
    private readonly ILogger<UnitOfWorkBehavior<TRequest,TResponse>>  _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!typeof(TRequest).Name.EndsWith("Command"))
        {
            return await next();
        }
        
        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var response =  await next();
            if (response is Response { IsSuccess: false })
            {
                _logger.LogInformation("UnitOfWork request Fails for: {@RequestName}", typeof(TRequest).Name);
                return response;
            }
            try
            {
             await _unitOfWorkService.CommitAsync(cancellationToken);
             transactionScope.Complete();
             _logger.LogInformation("UnitOfWork request Success for: {@RequestName}",typeof(TRequest).Name);
             return response;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("UnitOfWork request Fails for: {@RequestName}",typeof(TRequest).Name);
                throw new UnitOfWorkExceptions($"{ex.Message} : {ex.InnerException?.Message}", HttpStatusCode.InternalServerError);
            }
        }
    }
}