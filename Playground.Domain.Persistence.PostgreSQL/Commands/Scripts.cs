namespace Playground.Domain.Persistence.PostgreSQL.Commands
{
    internal static class Scripts
    {
        internal static string CreateEventStream = "public.create_event_stream";
        internal static string AddEvents = "public.save_events_in_stream";
        internal static string RemoveEvent = "";
        internal static string RemoveAllEvents = "";
    }
}