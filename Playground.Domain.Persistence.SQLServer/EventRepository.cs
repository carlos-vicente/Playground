using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Playground.Domain.Persistence.Events;

namespace Playground.Domain.Persistence.SQLServer
{
    public class EventRepository : IEventRepository
    {
        public Task<bool> CheckStream(Guid streamId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StoredEvent>> GetAll(Guid streamId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StoredEvent>> GetSelected(Guid streamId, long fromEventId)
        {
            throw new NotImplementedException();
        }

        public Task<StoredEvent> Get(Guid streamId, long eventId)
        {
            throw new NotImplementedException();
        }

        public Task<StoredEvent> GetLast(Guid streamId)
        {
            throw new NotImplementedException();
        }

        public Task CreateStream(Guid streamId, string aggregateTypeName)
        {
            throw new NotImplementedException();
        }

        public Task Add(Guid streamId, StoredEvent storedEvent)
        {
            throw new NotImplementedException();
        }

        public Task Add(Guid streamId, ICollection<StoredEvent> events)
        {
            throw new NotImplementedException();
        }

        public Task Remove(Guid streamId, long eventId)
        {
            throw new NotImplementedException();
        }

        public Task Remove(Guid streamId)
        {
            throw new NotImplementedException();
        }
    }
}
