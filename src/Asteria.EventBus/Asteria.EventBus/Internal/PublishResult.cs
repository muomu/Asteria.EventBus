namespace Asteria.EventBus.Internal
{
    internal record PublishResult(string State, string Message, Dictionary<string, object> ReturnTags)
    {
        const string STATE_Succeed = "Succeed";
        const string STATE_ERROR = "Error";
        public bool IsSucceed => State.Equals(STATE_Succeed, StringComparison.InvariantCultureIgnoreCase);
        public bool IsFailed => State.Equals(STATE_ERROR, StringComparison.InvariantCultureIgnoreCase);
    }
}
