using System;
using Playground.Domain.Model;

namespace Playground.Messaging.Persistence.UnitTests
{
    internal class AggregateState : IAggregateState
    {
        
    }

    internal class Aggregate : AggregateRoot<AggregateState>
    {
        public Aggregate(Guid id) : base(id)
        {
        }
    }
}