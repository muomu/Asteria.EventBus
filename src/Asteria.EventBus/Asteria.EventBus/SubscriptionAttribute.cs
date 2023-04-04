using Asteria.EventBus.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Asteria.EventBus
{
    /// <summary>
    /// 事件订阅
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class EventSubscriptionAttribute : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// 实例化 EventSubscriptionAttribute 对象的新的实例
        /// </summary>
        /// <param name="eventName"></param>
        /// <exception cref="ArgumentException"></exception>
        public EventSubscriptionAttribute(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException($"“{nameof(eventName)}”不能为 null 或空白。", nameof(eventName));
            }
            EventName = eventName;
        }

        /// <summary>
        /// 获取订阅的事件名称
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// 获取或设置事件消费的消费者组
        /// </summary>
        public string? ConsumerGroup { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值表示此事件是否需要事务同步
        /// </summary>
        public bool Transaction { get; set; }



        /// <inheritdoc/>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!Transaction)
            {
                await next();
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("x-asteria-transaction-commit-url", out var xtcu) || xtcu.FirstOrDefault() is not string url)
            {
                await next();
                return;
            }
            if (!context.HttpContext.Request.Headers.TryGetValue("x-asteria-event-id", out var xaei) || xaei.FirstOrDefault() is not string eventId)
            {
                await next();
                return;
            }



            var clientFactory = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
            
            var client = clientFactory.CreateClient();
            var commitUrl = string.Concat(url, "?id=", eventId);
            var asteriaEventTransaction = new AsteriaEventTransaction(commitUrl, client);
            context.HttpContext.Items["x-asteria-transaction"] = asteriaEventTransaction;
            AsteriaEventBus._transaction_internal.Value = asteriaEventTransaction;
        }
    }
}
