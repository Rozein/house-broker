namespace HouseBroker.Domain.Utils;

public static class HouseBrokerConstants
{
    public static class PaginationDefaults
    {
        // default = No pagination
        public const int PageSize = default;
        public const int PageNumber = default;
        
        public const int MinPageSize = 1;
        public const int MinPageNumber = 1;
    }
}