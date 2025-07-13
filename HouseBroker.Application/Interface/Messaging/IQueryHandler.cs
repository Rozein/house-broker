using HouseBroker.Application.Pagination;
using MediatR;

namespace HouseBroker.Application.Interface.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Response<TResponse>>
    where TQuery : IQuery<TResponse>
{
}

public interface IQueryHandlerPaginated<TQuery, TResponse> : IRequestHandler<TQuery, Response<PaginatedList<TResponse>>>
    where TQuery : IQueryPaginated<TResponse>
{
}