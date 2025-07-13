using System.Net;

namespace HouseBroker.Application.Exceptions;

public class UnitOfWorkExceptions : Exception
{
    public UnitOfWorkExceptions(string message, HttpStatusCode code) : base(message)
    {
        
    }
}