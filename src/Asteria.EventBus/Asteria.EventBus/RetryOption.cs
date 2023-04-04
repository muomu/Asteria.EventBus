namespace Asteria.EventBus
{
    /// <summary>
    /// 提供重试策略的配置
    /// </summary>
    public class RetryOption
    {
        /// <summary>
        /// 获取或设置重试策略的重试次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 获取或设置重试策略的重试间隔
        /// </summary>
        public TimeSpan Interval { get; set; }
    }
}
