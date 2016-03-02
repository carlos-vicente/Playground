using System;
using System.Collections.Generic;

namespace Playground.Domain.Persistence
{
    public interface IEventStore
    {
        void StoreEvents(Guid streamId, ICollection<IEvent> eventsToStore);
        ICollection<IEvent> LoadAllEvents(Guid streamId);
        ICollection<IEvent> LoadSelectedEvents(Guid streamId, long fromEventId, long toEventId);
    }
}