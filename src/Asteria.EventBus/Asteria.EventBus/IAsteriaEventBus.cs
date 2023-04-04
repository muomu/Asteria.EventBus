namespace Asteria.EventBus
{
    /// <summary>
    /// Asteria Event Bus Api Entry
    /// </summary>
    public interface IAsteriaEventBus
    {

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="asteriaEvent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync<T>(T asteriaEvent, CancellationToken cancellationToken = default) where T : AsteriaEvent;

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="asteriaEvent"></param>
        /// <param name="retryOptionAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync<T>(T asteriaEvent, Action<RetryOption> retryOptionAction, CancellationToken cancellationToken = default) where T : AsteriaEvent;

        /// <summary>
        /// 获取事件处理的事务
        /// </summary>
        AsteriaEventTransaction? Transaction { get; }
    }
    
}
