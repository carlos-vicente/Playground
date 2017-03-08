using System;

namespace Playground.Domain.Persistence.Snapshots
{
    public class StoredSnapshot
    {
        public long Version { get; set; }
        public DateTime TakenOn { get; set; }
        public string Data { get; set; }

        public StoredSnapshot()
        {
            // only for mapping & deserialization
        }

        public StoredSnapshot(long version, DateTime takenOn, string data)
        {
            Version = version;
            TakenOn = takenOn;
            Data = data;
        }
    }
}