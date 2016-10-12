using System;
using Playground.Domain.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model.Events
{
    public class ShipOrder : DomainEvent
    {
        public ShipOrder(Guid id) : base(id)
        {
            
        }
    }
}