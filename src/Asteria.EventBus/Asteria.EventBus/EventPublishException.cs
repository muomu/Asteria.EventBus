namespace Asteria.EventBus
{
    /// <summary>
    /// 事件发布失败产生的异常
    /// </summary>
    public class EventPublishException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="asteriaEvent"></param>
        public EventPublishException(string? message, AsteriaEvent asteriaEvent) : base(message)
        {
            AsteriaEvent = asteriaEvent;
        }

        /// <summary>
        /// 获取产生异常的事件
        /// </summary>
        public AsteriaEvent AsteriaEvent { get; }
    }
}
