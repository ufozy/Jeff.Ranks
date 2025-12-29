namespace Jeff.Ranks.Common
{

    public class BusinessException : Exception
    {
        public string Code { get; }

        public BusinessException(string code, string message)
            : base(message)
        {
            Code = code;
        }
    }

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                await WriteError(context, StatusCodes.Status400BadRequest, ex.Code, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteError(context, StatusCodes.Status500InternalServerError,
                    "internal_error",
                    "internal error");
            }
        }

        public class ApiError
        {
            public string Code { get; set; } = default!;
            public string Message { get; set; } = default!;
            public string? Detail { get; set; }
        }


        private static async Task WriteError(
            HttpContext context,
            int statusCode,
            string code,
            string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var error = new ApiError
            {
                Code = code,
                Message = message
            };

            await context.Response.WriteAsJsonAsync(error);
        }
    }

}
