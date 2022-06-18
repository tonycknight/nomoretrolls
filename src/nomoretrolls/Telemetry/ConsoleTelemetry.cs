using Crayon;

namespace nomoretrolls.Telemetry
{
    internal class ConsoleTelemetry : ITelemetry
    {
        private readonly Action<string> _writeMessage;

        public ConsoleTelemetry() : this(Console.WriteLine)
        {
        }

        public ConsoleTelemetry(Action<string> writeMessage)
        {
            _writeMessage = writeMessage;
        }

        public void Event(TelemetryEvent evt)
        {
            var time = evt.Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var line = evt switch
            {
                TelemetryErrorEvent error =>    $"[{time}] {Output.Bright.Red(error.Message)}",
                TelemetryTraceEvent trace =>    $"[{time}] {Output.Dim().White(trace.Message)}",
                TelemetryWarningEvent warn =>   $"[{time}] {Output.Bright.Yellow(warn.Message)}",
                TelemetryHeadlineEvent head =>  $"[{time}] {Output.Bright.Cyan(head.Message)}",
                _ =>                            $"[{time}] {Output.Bright.White(evt.Message)}",
            };

            _writeMessage(line);
        }

        public void Message(string message) => Event(new TelemetryInfoEvent { Message = message });
        
        public void Error(string message) => Event(new TelemetryErrorEvent { Message = message });
    }
}
