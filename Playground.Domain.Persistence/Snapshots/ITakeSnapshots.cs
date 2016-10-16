using Playground.Domain.Model;

namespace Playground.Domain.Persistence.Snapshots
{
    public interface ITakeSnapshots<TAggregateState>
        where TAggregateState : class, IAggregateState, new()
    {
        Snapshot<TAggregateState> TakeSnapshot();
    }
}