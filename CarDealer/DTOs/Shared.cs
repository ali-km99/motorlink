namespace CarDealer.API.DTOs
{
    // ─── Shared ────────────────────────────────────────────────────────────────────

    public record PagedResult<T>(
        List<T> Items,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );

    public record ApiResponse<T>(
        bool Success,
        string Message,
        T? Data = default
    )
    {
        public static ApiResponse<T> Ok(T data, string message = "Success") =>
            new(true, message, data);

        public static ApiResponse<T> Fail(string message) =>
            new(false, message, default);
    }

}
