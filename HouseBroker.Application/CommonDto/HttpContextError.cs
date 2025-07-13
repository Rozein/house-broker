using System.Net;

namespace HouseBroker.Application.CommonDto
{
    public static class HttpContextError
    {
        public static Error NotFound(string message)
        {
            return new Error(HttpStatusCode.NotFound, message);
        }
        
        public static Error GenericError(HttpStatusCode code,  string message, bool commitTransaction = false)
        {
            return new Error(code, message) { CommitTransaction = commitTransaction };
        }

        public static Error BadRequest(string message)
        {
            return new Error(HttpStatusCode.BadRequest, message);
        }

        public static Error Ambiguous(string message)
        {
            return new Error(HttpStatusCode.Ambiguous, message);
        }

        public static Error Forbidden(string message)
        {
            return new Error(HttpStatusCode.Forbidden, message);
        }
        
        public static Error NotAcceptable(string message)
        {
            return new Error(HttpStatusCode.NotAcceptable, message);
        }
        
        public static Error InternalServerError(string message)
        {
            return new Error(HttpStatusCode.InternalServerError, message);
        }
        
        public static Error UnAuthorized(string message)
        {
            return new Error(HttpStatusCode.Unauthorized, message);
        }
        
        public static Error CommittableUnAuthorized(string message)
        {
            return new Error(HttpStatusCode.Unauthorized, message)  { CommitTransaction = true };
        }
        
        public static Error Conflict(string message)
        {
            return new Error(HttpStatusCode.Conflict, message);
        }
    }
}
