using Serilog.Core;
using Serilog.Events;

namespace LundBot.IntegrationTests.Discord.Events.Support;

internal sealed class EventLogSink : ILogEventSink
{
    public List<LogEvent> Events { get; } = [];

    public void Emit(LogEvent logEvent) => Events.Add(logEvent);
}
