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
    }
}