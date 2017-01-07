﻿namespace Playground.Domain.Persistence.PostgreSQL.Queries
{
    internal static class Scripts
    {
        internal static string CheckIfStreamExists = "es.check_stream_exists";
        internal static string GetAllEvents = "es.get_events_for_stream";
        internal static string GetSelectedEvents = "es.get_selected_events_for_stream";
        internal static string GetEvent = "";
        internal static string GetLastEvent = "es.get_last_event_for_stream";
    }

    internal static class ScriptsWithString
    {
        internal static string CheckIfStreamExists = "esgeneric.check_stream_exists";
        internal static string GetAllEvents = "esgeneric.get_events_for_stream";
        internal static string GetSelectedEvents = "esgeneric.get_selected_events_for_stream";
        internal static string GetEvent = "";
        internal static string GetLastEvent = "esgeneric.get_last_event_for_stream";
    }
}