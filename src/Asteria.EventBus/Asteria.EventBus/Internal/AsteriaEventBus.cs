using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Asteria.EventBus.Internal
{
    internal class AsteriaEventBus : IAsteriaEventBus
    {
        internal static AsyncLocal<AsteriaEventTransaction?> _transaction_internal = new AsyncLocal<AsteriaEventTransaction?>();


        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<AsteriaEventBusOptions> _optionsMonitor;
        private readonly IOptionsFactory<JsonSerializerOptions> jsonSerializerOptions;

        public AsteriaEventBus(IOptions<AsteriaEventBusOptions> optionsMonitor, IOptionsFactory<JsonSerializerOptions> jsonSerializerOptions, IHttpClientFactory httpClientFactory)
        {
            _optionsMonitor = optionsMonitor;
            this.jsonSerializerOptions = jsonSerializerOptions;
            _httpClientFactory = httpClientFactory;
        }

        public AsteriaEventTransaction? Transaction => _transaction_internal?.Value;

        private AsteriaEventBusOptions Configuration => _optionsMonitor.Value;
        private JsonSerializerOptions JsonSerializerOptions => jsonSerializerOptions.Create(nameof(AsteriaEventBus));

        private Uri PublishApiUri => new(new Uri(Configuration.Server), $"api/{Configuration.ApiVersion}/events/publish");

        public Task PublishAsync<T>(T asteriaEvent, CancellationToken cancelToken = default) where T : AsteriaEvent
        {
            return PublishAsync(asteriaEvent, Configuration.RetryOption, cancelToken);
        }

        public Task PublishAsync<T>(T asteriaEvent, Action<RetryOption> retryOptionAction, CancellationToken cancelToken = default) where T : AsteriaEvent
        {
            var retryOption = new RetryOption();
            retryOptionAction?.Invoke(retryOption);

            return PublishAsync(asteriaEvent, retryOption, cancelToken);
        }

        private async Task PublishAsync<T>(T asteriaEvent, RetryOption retryOption, CancellationToken cancellationToken) where T : AsteriaEvent
        {
            var content = JsonContent.Create(inputValue: asteriaEvent, mediaType: null, options: JsonSerializerOptions);

            using var client = _httpClientFactory.CreateClient(nameof(AsteriaEventBus));

            PublishResult? result = null;

            for (int i = 0; i < retryOption.Count; i++)
            {
                var response = await client.PostAsync(PublishApiUri, content, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<PublishResult>(JsonSerializerOptions, cancellationToken).ConfigureAwait(false);
                    if (result is not null) break;
                }
                else
                {
                    await Task.Yield();
                    await Task.Delay(retryOption.Interval, cancellationToken).ConfigureAwait(false);
                }
            }

            if (result is null) throw new EventPublishException("json serialize failed.", asteriaEvent);
            if (result.IsFailed) throw new EventPublishException(result.Message, asteriaEvent);

            asteriaEvent.InitialTags(result.ReturnTags);

        }
    }
}
