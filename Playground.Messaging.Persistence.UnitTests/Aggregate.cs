using System;
using Playground.Domain.Model;

namespace Playground.Messaging.Persistence.UnitTests
{
    internal class Aggregate : AggregateRoot
    {
        public Aggregate(Guid id) : base(id)
        {
        }
    }
}