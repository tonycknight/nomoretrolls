namespace nomoretrolls.Telemetry
{
    internal record TelemetryEvent
    {
        public DateTime Time { get; init; } = DateTime.UtcNow;
        public string Message { get; init; } = default!;        
    }
}
