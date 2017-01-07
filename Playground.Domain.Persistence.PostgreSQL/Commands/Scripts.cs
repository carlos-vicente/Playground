namespace Playground.Domain.Persistence.PostgreSQL.Commands
{
    internal static class Scripts
    {
        internal static string CreateEventStream = "es.create_event_stream";
        internal static string AddEvents = "es.save_events_in_stream";
        internal static string RemoveEvent = "";
        internal static string RemoveAllEvents = "";
    }

    internal static class ScriptsWithString
    {
        internal static string CreateEventStream = "esgeneric.create_event_stream";
        internal static string AddEvents = "esgeneric.save_events_in_stream";
        internal static string RemoveEvent = "";
        internal static string RemoveAllEvents = "";
    }
}