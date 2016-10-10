using System;
using Playground.Domain.Model;

namespace Playground.Messaging.Persistence.UnitTests
{
    internal class AggregateState
    {
        
    }

    internal class Aggregate : AggregateRoot<AggregateState>
    {
        public Aggregate(Guid id) : base(id)
        {
        }
    }
}