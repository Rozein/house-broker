using System.Runtime.Serialization;

namespace HouseBroker.Application.Exceptions
{
    [Serializable]
    public class HouseBrokerException : Exception
    {
        //
        // Summary:
        //     Creates a new HouseBrokerException object.
        public HouseBrokerException()
        {
        }

        //
        // Summary:
        //     Creates a new HouseBrokerException object.
        public HouseBrokerException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        //
        // Summary:
        //     Creates a new HouseBrokerException object.
        //
        // Parameters:
        //   message:
        //     Exception message
        public HouseBrokerException(string message)
            : base(message)
        {
        }

        //
        // Summary:
        //     Creates a new HouseBrokerException object.
        //
        // Parameters:
        //   message:
        //     Exception message
        //
        //   innerException:
        //     Inner exception
        public HouseBrokerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
