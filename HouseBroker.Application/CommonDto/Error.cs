using System.Net;
using System.Text.Json.Serialization;

namespace HouseBroker.Application.CommonDto
{

    public record Error(HttpStatusCode Status, string ErrorMessage, [property: JsonIgnore] bool CommitTransaction = false)
    {
        public static Error None = new(HttpStatusCode.OK, string.Empty);
    }

}
