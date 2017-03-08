using System;
using System.Threading.Tasks;
using Playground.Core.Logging;
using Playground.Core.Serialization;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence.Snapshots
{
    public class SnapshotStore : ISnapshotStore
    {
        private readonly ISnapshotRepository _repository;
        private readonly IObjectSerializer _objectSerializer;
        private readonly ILogger _logger;

        public SnapshotStore(
            ISnapshotRepository repository,
            IObjectSerializer objectSerializer,
            ILogger logger)
        {
            _repository = repository;
            _objectSerializer = objectSerializer;
            _logger = logger;
        }

        public async Task<Snapshot<TAggregateState>> GetLastestSnaptshot<TAggregateState>(
            Guid streamId)
            where TAggregateState : class, IAggregateState, new()
        {
            if(streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            _logger.Debug($"Going to obtain a snapshot for stream {streamId}");

            var snapshot = await _repository
                .GetLatestSnapshot(streamId)
                .ConfigureAwait(false);

            if (snapshot == null)
            {
                _logger.Debug($"No available snapshot for stream {streamId}");
                return null;
            }

            var state = _objectSerializer
                .Deserialize(snapshot.Data, typeof(TAggregateState))
                as TAggregateState;

            _logger.Debug($"Deserialized snapshot for stream {streamId}");

            return new Snapshot<TAggregateState>(
                snapshot.Version,
                snapshot.TakenOn,
                state);
        }

        public async Task StoreNewSnapshot<TAggregateState>(
            Guid streamId,
            Snapshot<TAggregateState> snapshot) 
            where TAggregateState : class, IAggregateState, new()
        {
            if(streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            if(snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            if(snapshot.Version <= 0)
                throw new ArgumentException("Snapshot's version number must be higher than 0");
            if (snapshot.TakenOn > DateTime.UtcNow)
                throw new ArgumentException("Snapshot's taken on timestamp must be in the past");
            if (snapshot.Data == null)
                throw new ArgumentException("Snapshot's data must not be null");

            _logger
                .Debug("Going to save new snapshot for stream {0}", streamId);

            var latestVersion = await _repository
                .GetLatestSnapshotVersion(streamId)
                .ConfigureAwait(false);

            if (latestVersion.HasValue 
                && latestVersion.Value >= snapshot.Version)
            {
                throw new InvalidOperationException(string.Format(
                    "Can not save new snapshot for stream {0} on version {1} because there is one for version {2}",
                    streamId,
                    snapshot.Version,
                    latestVersion.Value));
            }

            var serialized = _objectSerializer
                .Serialize(snapshot.Data);

            var storedSnapshot = new StoredSnapshot(
                snapshot.Version,
                snapshot.TakenOn,
                serialized);

            var saved = await _repository
                .SaveSnapshot(streamId, storedSnapshot)
                .ConfigureAwait(false);

            if (!saved)
            {
                throw new InvalidOperationException(string.Format(
                    "Failed saving a new snapshot for stream {0} for version {1}",
                    streamId,
                    snapshot.Version));
            }
            else
            {
                _logger
                    .Information("New snapshot saved for stream {0}", streamId);
            }

        }

        public Task<Snapshot<TAggregateState>> GetClosestSnapshot<TAggregateState>(
            Guid streamId,
            long version) 
            where TAggregateState : class, IAggregateState, new()
        {
            throw new NotImplementedException();
        }
    }

    public class SnapshotStoreWithGenericIdentity : ISnapshotStoreWithGenericIdentity
    {
        private readonly ISnapshotRepositoryWithGenericIdentity _repository;
        private readonly IObjectSerializer _objectSerializer;
        private readonly ILogger _logger;

        public SnapshotStoreWithGenericIdentity(
            ISnapshotRepositoryWithGenericIdentity repository,
            IObjectSerializer objectSerializer,
            ILogger logger)
        {
            _repository = repository;
            _objectSerializer = objectSerializer;
            _logger = logger;
        }

        public async Task<Snapshot<TAggregateState>> GetLastestSnaptshot<TAggregateState>(
            string streamId)
            where TAggregateState : class, IAggregateState, new()
        {
            if (streamId == null)
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            _logger.Debug($"Going to obtain a snapshot for stream {streamId}");

            var snapshot = await _repository
                .GetLatestSnapshot(streamId)
                .ConfigureAwait(false);

            if (snapshot == null)
            {
                _logger.Debug($"No available snapshot for stream {streamId}");
                return null;
            }

            var state = _objectSerializer
                .Deserialize(snapshot.Data, typeof(TAggregateState))
                as TAggregateState;

            _logger.Debug($"Deserialized snapshot for stream {streamId}");

            return new Snapshot<TAggregateState>(
                snapshot.Version,
                snapshot.TakenOn,
                state);
        }

        public async Task StoreNewSnapshot<TAggregateState>(
            string streamId,
            Snapshot<TAggregateState> snapshot)
            where TAggregateState : class, IAggregateState, new()
        {
            if (streamId == null)
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            if (snapshot.Version <= 0)
                throw new ArgumentException("Snapshot's version number must be higher than 0");
            if (snapshot.TakenOn > DateTime.UtcNow)
                throw new ArgumentException("Snapshot's taken on timestamp must be in the past");
            if (snapshot.Data == null)
                throw new ArgumentException("Snapshot's data must not be null");

            _logger
                .Debug("Going to save new snapshot for stream {0}", streamId);

            var latestVersion = await _repository
                .GetLatestSnapshotVersion(streamId)
                .ConfigureAwait(false);

            if (latestVersion.HasValue
                && latestVersion.Value >= snapshot.Version)
            {
                throw new InvalidOperationException(string.Format(
                    "Can not save new snapshot for stream {0} on version {1} because there is one for version {2}",
                    streamId,
                    snapshot.Version,
                    latestVersion.Value));
            }

            var serialized = _objectSerializer
                .Serialize(snapshot.Data);

            var storedSnapshot = new StoredSnapshot(
                snapshot.Version,
                snapshot.TakenOn,
                serialized);

            var saved = await _repository
                .SaveSnapshot(streamId, storedSnapshot)
                .ConfigureAwait(false);

            if (!saved)
            {
                throw new InvalidOperationException(string.Format(
                    "Failed saving a new snapshot for stream {0} for version {1}",
                    streamId,
                    snapshot.Version));
            }
            else
            {
                _logger
                    .Information("New snapshot saved for stream {0}", streamId);
            }

        }

        public Task<Snapshot<TAggregateState>> GetClosestSnapshot<TAggregateState>(
            string streamId,
            long version)
            where TAggregateState : class, IAggregateState, new()
        {
            throw new NotImplementedException();
        }
    }
}