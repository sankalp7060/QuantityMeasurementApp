using System.Collections.Concurrent;

namespace QuantityMeasurementWebAPI.Middleware
{
    /// <summary>
    /// Middleware for rate limiting to prevent brute force attacks
    /// This middleware tracks requests by IP address and limits the number of requests
    /// to the login endpoint within a specified time window
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        // Thread-safe dictionary to store client request information
        private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clientRequests =
            new();

        // Maximum number of requests allowed per time window
        private const int MAX_REQUESTS = 10;

        // Time window in seconds
        private const int TIME_WINDOW_SECONDS = 60;

        /// <summary>
        /// Initializes a new instance of the RateLimitingMiddleware
        /// </summary>
        /// <param name="next">Next middleware in the pipeline</param>
        /// <param name="logger">Logger instance</param>
        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware for each HTTP request
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply rate limiting to login endpoint
            // This protects against brute force password attacks
            if (context.Request.Path.StartsWithSegments("/api/v1/Auth/login"))
            {
                // Get client IP address
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var clientKey = $"{clientIp}";

                var now = DateTime.UtcNow;

                // Update or add client request information
                var clientInfo = _clientRequests.AddOrUpdate(
                    clientKey,
                    // Add new entry
                    new ClientRequestInfo { RequestCount = 1, WindowStart = now },
                    // Update existing entry
                    (key, existingInfo) =>
                    {
                        // If the time window has expired, reset the counter
                        if (
                            now - existingInfo.WindowStart
                            > TimeSpan.FromSeconds(TIME_WINDOW_SECONDS)
                        )
                        {
                            return new ClientRequestInfo { RequestCount = 1, WindowStart = now };
                        }

                        // Otherwise increment the counter
                        existingInfo.RequestCount++;
                        return existingInfo;
                    }
                );

                // Log request count for monitoring (useful for detecting attacks)
                _logger.LogDebug(
                    "Client {ClientIp} has made {Count} requests in the current window",
                    clientIp,
                    clientInfo.RequestCount
                );

                // Check if the client has exceeded the rate limit
                if (clientInfo.RequestCount > MAX_REQUESTS)
                {
                    // Calculate when the client can try again
                    var retryAfterSeconds = (int)(
                        TIME_WINDOW_SECONDS - (now - clientInfo.WindowStart).TotalSeconds
                    );

                    // Log the rate limit violation (important for security monitoring)
                    _logger.LogWarning(
                        "Rate limit exceeded for IP: {ClientIp}. Requests: {Count}, Retry after: {RetryAfter}s",
                        clientIp,
                        clientInfo.RequestCount,
                        retryAfterSeconds
                    );

                    // Set response headers
                    context.Response.StatusCode = 429; // Too Many Requests
                    context.Response.ContentType = "application/json";
                    context.Response.Headers.Append("Retry-After", retryAfterSeconds.ToString());

                    // Create error response
                    var response = new
                    {
                        Success = false,
                        Message = "Too many login attempts. Please try again later.",
                        RetryAfterSeconds = retryAfterSeconds,
                    };

                    // Write response as JSON
                    await context.Response.WriteAsJsonAsync(response);
                    return; // Short-circuit the pipeline
                }
            }

            // Continue to the next middleware
            await _next(context);
        }

        /// <summary>
        /// Helper class to track client request information
        /// </summary>
        private class ClientRequestInfo
        {
            /// <summary>
            /// Number of requests made in the current time window
            /// </summary>
            public int RequestCount { get; set; }

            /// <summary>
            /// Start time of the current time window
            /// </summary>
            public DateTime WindowStart { get; set; }
        }
    }
}
