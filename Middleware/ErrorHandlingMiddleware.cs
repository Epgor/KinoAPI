using StackExchange.Redis;

namespace KinoAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public ErrorHandlingMiddleware() {}
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadHttpRequestException exception)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(exception.Message);
            }
            catch (UnauthorizedAccessException exception)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(exception.Message);
            }
            catch (KeyNotFoundException exception)
            {
                context.Response.StatusCode = 402;
                await context.Response.WriteAsync(exception.Message);
            }
            catch (RedisException redisException)
            {
                context.Response.StatusCode = 501;
                await context.Response.WriteAsync(redisException.Message);
            }
            catch (FormatException)
            {
                context.Response.StatusCode = 601;
                await context.Response.WriteAsync("Incorrect format exception");
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(e.Message);
            }
        }
    }
}
