using MediatR;

namespace HouseBroker.Application.Interface.Messaging
{
    public interface ICommand : IRequest<Response>, IBaseCommand
    {
    }

    public interface ICommand<TResponse> : IRequest<Response<TResponse>> , IBaseCommand
    {

    }

    public interface IBaseCommand
    {

    }
}
