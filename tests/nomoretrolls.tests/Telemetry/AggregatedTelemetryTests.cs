using System.Linq;
using nomoretrolls.Config;
using nomoretrolls.Telemetry;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Telemetry
{
    public class AggregatedTelemetryTests
    {
        [Fact]
        public void Event_EventMessagePropagated()
        {            
            var cp = CreateMockConfigurationProvider();
            var telemetries = Enumerable.Range(1, 3)
                .Select(i => Substitute.For<ITelemetry>())
                .ToArray();

            var aggTelemetry = new AggregatedTelemetry(telemetries, cp);

            var evt = new TelemetryEvent();

            aggTelemetry.Event(evt);

            foreach(var t in telemetries)
            {
                t.Received(1).Event(evt);
            }
        }

        [Fact]
        public void Message_EventMessagePropagated()
        {
            var cp = CreateMockConfigurationProvider();
            var telemetries = Enumerable.Range(1, 3)
                .Select(i => Substitute.For<ITelemetry>())
                .ToArray();

            var aggTelemetry = new AggregatedTelemetry(telemetries, cp);

            var evt = new TelemetryEvent() { Message = "test" };

            aggTelemetry.Event(evt);

            foreach (var t in telemetries)
            {
                t.Received(1).Event(Arg.Is<TelemetryEvent>(e => e.Message == evt.Message));
            }
        }

        [Fact]
        public void Error_EventMessagePropagated()
        {
            var cp = CreateMockConfigurationProvider();
            var telemetries = Enumerable.Range(1, 3)
                .Select(i => Substitute.For<ITelemetry>())
                .ToArray();

            var aggTelemetry = new AggregatedTelemetry(telemetries, cp);

            var evt = new TelemetryEvent() { Message = "test" };

            aggTelemetry.Event(evt);

            foreach (var t in telemetries)
            {
                t.Received(1).Event(Arg.Is<TelemetryEvent>(e => e.Message == evt.Message));
            }
        }

        private IConfigurationProvider CreateMockConfigurationProvider()
        {
            var config = new AppConfiguration();
            var cp = Substitute.For<IConfigurationProvider>();
            cp.GetAppConfiguration().Returns(config);
            return cp;
        }
    }
}
