using HouseBroker.Application.Pagination;
using MediatR;

namespace HouseBroker.Application.Interface.Messaging;

public interface IQuery<TResponse> : IRequest<Response<TResponse>>
{
}

public interface IQueryPaginated<TResponse> : IRequest<Response<PaginatedList<TResponse>>>
{

}