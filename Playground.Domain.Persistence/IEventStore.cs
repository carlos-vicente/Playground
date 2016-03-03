using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playground.Domain.Persistence
{
    public interface IEventStore
    {
        Task StoreEvents(Guid streamId, ICollection<IEvent> eventsToStore);
        Task<ICollection<IEvent>> LoadAllEvents(Guid streamId);
        Task<ICollection<IEvent>> LoadSelectedEvents(Guid streamId, long fromEventId, long toEventId);
    }
}