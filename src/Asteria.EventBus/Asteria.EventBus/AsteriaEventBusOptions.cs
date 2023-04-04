using System.Reflection;

namespace Asteria.EventBus
{
    /// <summary>
    /// 
    /// </summary>
    public class AsteriaEventBusOptions
    {

        /// <summary>
        /// 
        /// </summary>
        public AsteriaEventBusOptions()
        {
            RetryOption = new RetryOption()
            {
                Count = 1,
                Interval = TimeSpan.FromSeconds(1)
            };
            DefaultHeaders = new Dictionary<string, string>();
        }


        /// <summary>
        /// 获取或设置默认重试策略
        /// </summary>
        public RetryOption RetryOption { get; set; }

        /// <summary>
        /// 获取或设置集群服务器地址
        /// </summary>
        public string Server { get; set; } = null!;

        /// <summary>
        /// 获取或设置请求默认携带的HTTP请求头
        /// </summary>
        public Dictionary<string, string> DefaultHeaders { get; set; }


        /// <summary>
        /// 获取或设置api版本
        /// </summary>
        public string ApiVersion { get; set; } = "v1";

        /// <summary>
        /// 获取或设置默认消费者组
        /// </summary>
        public string ConsumerGroup { get; set; } = Assembly.GetEntryAssembly()?.FullName ?? null!;

    }
}
