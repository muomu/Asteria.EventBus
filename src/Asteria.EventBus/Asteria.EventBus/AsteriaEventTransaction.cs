namespace Asteria.EventBus
{
    /// <summary>
    /// 提供事件处理的提交与回滚操作
    /// </summary>
    public sealed class AsteriaEventTransaction : IDisposable
    {
        private readonly string _commit_url;
        private readonly HttpClient _httpClient;

        internal AsteriaEventTransaction(string commit_url, HttpClient httpClient)
        {
            _commit_url = commit_url;
            _httpClient = httpClient;
        }


        /// <summary>
        /// 提交当前环境消费的事件
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsync(_commit_url, null, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode) throw new InvalidOperationException(Enum.GetName(response.StatusCode));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 回滚当前环境消费的事件
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var response = await _httpClient.DeleteAsync(_commit_url, cancellationToken).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) break;
            }
        }
    }
    
}
