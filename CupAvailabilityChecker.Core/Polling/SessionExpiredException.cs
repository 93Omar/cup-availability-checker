namespace CupAvailabilityChecker.Core.Polling
{
    /// <summary>
    /// Signals that the browser session has expired while polling for availability (step 4),
    /// requiring the orchestrator to restart the flow from step 1 (navigation/login).
    /// </summary>
    public sealed class SessionExpiredException : Exception
    {
        public SessionExpiredException()
            : base("La sessione di navigazione è scaduta.")
        {
        }

        public SessionExpiredException(string message)
            : base(message)
        {
        }
    }
}
