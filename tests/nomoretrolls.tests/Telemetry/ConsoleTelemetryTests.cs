using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using nomoretrolls.Telemetry;
using Xunit;

namespace nomoretrolls.tests.Telemetry
{
    public class ConsoleTelemetryTests
    {
        [Theory]
        [MemberData(nameof(GetTelemetryEvents), "test")]
        public void Event_MessageSent(object evto)
        {
            var evt = (TelemetryEvent)evto;
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            
            t.Event(evt);

            r.Should().Contain(evt.Message);
        }

        [Fact]
        public void Event_MessageSent_WithTimestamp()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var evt = new TelemetryEvent()
            {
                Message = "test",
                Time = DateTime.UtcNow,
            };

            t.Event(evt);

            var expected = evt.Time.ToString("yyyy-MM-dd HH:mm:ss.fff");

            r.Should().Contain(expected);
        }

        [Fact]
        public void Message_MessageSent()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var msg = "test";

            t.Event(new TelemetryEvent() { Message = msg } );

            r.Should().Contain(msg);
        }

        

        [Fact]
        public void Error_MessageSent()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var msg = "test";

            t.Event(new TelemetryErrorEvent() { Message = msg } );

            r.Should().Contain(msg);
        }


        [Fact]
        public void Error_Exception_MessageSent()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var msg1 = "test1";
            var msg2 = "test2";

            t.Event(new TelemetryErrorEvent() { Exception = new ApplicationException(msg2) });

            r.Should().Contain(msg2);
            r.Should().NotContain(msg1);
        }

        [Fact]
        public void Error_Exception_EmptyExceptionMessage_MessageSent()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var msg1 = "test1";
            var msg2 = "test2";

            t.Event(new TelemetryErrorEvent() { Exception = new ApplicationException(""), Message = msg1 });

            r.Should().NotContain(msg2);
            r.Should().Contain(msg1);
        }

        private static IEnumerable<object[]> GetTelemetryEvents(string message)
        {
            return new[] { new TelemetryEvent() { Message = message },
                new TelemetryTraceEvent() { Message = message },
                new TelemetryErrorEvent() { Message = message },
                new TelemetryInfoEvent() { Message = message },
                new TelemetryWarningEvent { Message = message },
                new TelemetryHeadlineEvent() { Message = message } }
                    .Select(e => new[] { e });
        }
    }
}
