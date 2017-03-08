using System;
using Playground.Domain.Events;

namespace Playground.Domain.UnitTests.Events
{
    public class ItHappened : DomainEvent
    {
        public string Name { get; set; }
    }
}