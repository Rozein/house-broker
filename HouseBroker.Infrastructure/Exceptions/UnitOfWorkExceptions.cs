using System.Net;

namespace HouseBroker.Infrastructure.Exceptions;

public class UnitOfWorkExceptions : Exception
{
    public UnitOfWorkExceptions(string message, HttpStatusCode code) : base(message)
    {
        
    }
}