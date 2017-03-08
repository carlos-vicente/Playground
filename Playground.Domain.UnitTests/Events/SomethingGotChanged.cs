using System;
using Playground.Domain.Events;

namespace Playground.Domain.UnitTests.Events
{
    public class SomethingGotChanged : DomainEvent
    {
        public string Something { get; private set; }
    }
}