namespace Playground.Domain.Persistence.PostgreSQL.Queries
{
    internal static class Scripts
    {
        internal static string CheckIfStreamExists = "public.check_stream_exists";
        internal static string GetAllEvents = "public.get_events_for_stream";
        internal static string GetEvent = "";
        internal static string GetLastEvent = "public.get_last_event_for_stream";
    }
}