namespace QuantityMeasurementWebAPI.Middleware
{
    /// <summary>
    /// Middleware to add security headers to all HTTP responses
    /// Implements OWASP recommended security headers
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;

        public SecurityHeadersMiddleware(
            RequestDelegate next,
            ILogger<SecurityHeadersMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers to response
            var headers = context.Response.Headers;

            // HTTP Strict Transport Security (HSTS) - Enforce HTTPS
            headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";

            // X-Frame-Options - Prevent clickjacking attacks
            headers["X-Frame-Options"] = "DENY";

            // X-Content-Type-Options - Prevent MIME type sniffing
            headers["X-Content-Type-Options"] = "nosniff";

            // X-XSS-Protection - Cross-site scripting filter
            headers["X-XSS-Protection"] = "1; mode=block";

            // Referrer-Policy - Control referrer information
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Content-Security-Policy - Prevent XSS and injection attacks
            headers["Content-Security-Policy"] =
                "default-src 'self'; "
                + "script-src 'self' 'unsafe-inline' 'unsafe-eval'; "
                + "style-src 'self' 'unsafe-inline'; "
                + "img-src 'self' data: https:; "
                + "font-src 'self'; "
                + "connect-src 'self' https:; "
                + "frame-ancestors 'none';";

            // Permissions-Policy - Restrict browser features
            headers["Permissions-Policy"] =
                "geolocation=(), " + "microphone=(), " + "camera=(), " + "payment=(), " + "usb=()";

            // Cache-Control - Prevent caching of sensitive data
            if (context.Request.Path.StartsWithSegments("/api/v1/Auth"))
            {
                headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
                headers["Pragma"] = "no-cache";
                headers["Expires"] = "0";
            }

            await _next(context);
        }
    }
}
