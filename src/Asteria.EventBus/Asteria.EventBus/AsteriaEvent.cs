using System.Collections.Concurrent;
using System.Diagnostics;

namespace Asteria.EventBus
{
    /// <summary>
    /// 表示事件，提供对事件的Tag读写
    /// </summary>

    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public abstract class AsteriaEvent 
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly object sync_root = new();

        private readonly ConcurrentDictionary<string, object> _tags;


        /// <summary>
        /// get all tags
        /// </summary>
        public virtual IReadOnlyDictionary<string, object> Tags => _tags;

        /// <summary>
        /// 获取该事件的事件名称
        /// </summary>
        public virtual string EventName { get; }

        /// <inheritdoc/>
        public AsteriaEvent(string eventName)
        {
            EventName = eventName;
            _tags = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        /// 获取或设置当前事件的Tag
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual object? this[string name]
        {
            get
            {
                if (_tags.TryGetValue(name, out var value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                if (value is null)
                {
                    _tags.TryRemove(name, out _);
                    return;
                }
                _tags.AddOrUpdate(name, value, (name, v) => value);
            }
        }


        /// <summary>
        /// 设置Tag值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void SetTag<T>(string name, T value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (value is null) throw new ArgumentNullException(nameof(value));

            _tags.AddOrUpdate(name, value, (key, v) => value);
        }

        /// <summary>
        /// 获取Tag值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual T? GetTag<T>(string name)
        {
            if(_tags.TryGetValue(name, out var value))
            {
                if (value is T v) return v;
            }

            return default;
        }

        /// <summary>
        /// 初始化tags
        /// </summary>
        /// <param name="items"></param>
        public virtual void InitialTags(IEnumerable<KeyValuePair<string, object>> items)
        {
            _tags.Clear();
            foreach (var item in items)
            {
                _tags.TryAdd(item.Key, item.Value);
            }
        }

        /// <inheritdoc/>
        protected virtual string GetDebuggerDisplay()
        {
            return EventName;
        }

    }


    /// <summary>
    /// 空事件
    /// </summary>
    public class AsteriaEmptyEvent : AsteriaEvent
    {
        /// <inheritdoc/>
        public AsteriaEmptyEvent(string eventName) : base(eventName)
        {
        }
    }


    /// <summary>
    /// 提供表示事件所需的所有信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsteriaEvent<T> : AsteriaEvent
    {
        /// <inheritdoc/>
        public AsteriaEvent(string eventName, T payload) : base(eventName)
        {
            Payload = payload;
        }

        /// <inheritdoc/>
        public AsteriaEvent(T payload) : base(typeof(T).FullName ?? typeof(T).Name)
        {
            Payload = payload;
        }


        /// <summary>
        /// 获取当前事件的荷载实例
        /// </summary>
        public virtual T Payload { get; }


    }

}
