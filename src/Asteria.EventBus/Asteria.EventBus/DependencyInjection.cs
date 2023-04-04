using Asteria.EventBus;
using Asteria.EventBus.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// 注入事务总线服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAsteriaEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AsteriaEventBusOptions>().Configure(x => configuration.GetRequiredSection("Asteria:EventBus").Bind(x));
            services.TryAddSingleton<IAsteriaEventBus, AsteriaEventBus>();
            services.AddHttpClient();
            services.AddHostedService<Launch>();
            return services;
        }
    }
}
