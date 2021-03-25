
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.ApplicationInsights.AspNetCore
{
    public static class ApplicationInsightExtensions
    {
        public static IServiceCollection AddApplicationInsightRequestBodyLogging(this IServiceCollection services) => 
            services.AddTransient<RequestBodyLoggingMiddleware>().AddTransient<ResponseBodyLoggingMiddleware>();

        public static IApplicationBuilder UseApplicationInsightRequestBodyLogging(this IApplicationBuilder builder) => 
            builder.UseMiddleware<RequestBodyLoggingMiddleware>();

        public static IApplicationBuilder UseApplicationInsightResponseBodyLogging(this IApplicationBuilder builder) => 
            builder.UseMiddleware<ResponseBodyLoggingMiddleware>();
    }
}