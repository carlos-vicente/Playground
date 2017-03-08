using System;

namespace Playground.Domain.Model
{
    public class Snapshot<TAggregateState>
        where TAggregateState : class, IAggregateState, new()
    {
        public long Version { get; private set; }

        public DateTime TakenOn { get; private set; }

        public TAggregateState Data { get; private set; }

        public Snapshot(long version, DateTime takenOn, TAggregateState data)
        {
            Version = version;
            TakenOn = takenOn;
            Data = data;
        }

        /// <summary>
        /// Factory method which uses UtcNow as the takenOn timestamp
        /// </summary>
        /// <param name="version">The aggregate state's version used to take the snapshot</param>
        /// <param name="data">The aggregate's state to snapshot</param>
        /// <returns></returns>
        public static Snapshot<TAggregateState> CreateSnapshot(
            long version,
            TAggregateState data)
        {
            return new Snapshot<TAggregateState>(
                version,
                DateTime.UtcNow,
                data);
        }
    }
}