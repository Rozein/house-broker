using MediatR;

namespace HouseBroker.Application.Interface.Messaging
{
    public interface ICommandHandler <TCommand>: IRequestHandler<TCommand, Response>
       where TCommand : ICommand
    {
    }

    public interface ICommandHandler<TCommand , TResponse> : IRequestHandler<TCommand, Response<TResponse>> 
    where TCommand : ICommand<TResponse>
    { }
}
