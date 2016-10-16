using System.Threading.Tasks;
using Playground.Domain.Model;
using Playground.Domain.Persistence;
using Playground.Messaging.Commands;

namespace Playground.Messaging.Persistence
{
    public abstract class AsyncDomainCommandHandler<TCommand, TAggregate, TAggregateState>
        : IAsyncCommandHandler<TCommand>
        where TCommand : DomainCommand<TAggregate, TAggregateState>
        where TAggregate : AggregateRoot<TAggregateState>
        where TAggregateState : class, IAggregateState, new()
    {
        private readonly IAggregateContext _aggregateContext;

        protected AsyncDomainCommandHandler(IAggregateContext aggregateContext)
        {
            _aggregateContext = aggregateContext;
        }

        public async Task Handle(TCommand command)
        {
            var aggregate = await _aggregateContext
                .TryLoad<TAggregate, TAggregateState>(command.AggregateRootId)
                .ConfigureAwait(false);

            if (aggregate == null)
            {
                aggregate = await _aggregateContext
                    .Create<TAggregate, TAggregateState>(command.AggregateRootId)
                    .ConfigureAwait(false);
            }

            await HandleOnAggregate(command, aggregate)
                .ConfigureAwait(false);

            await _aggregateContext
                .Save<TAggregate, TAggregateState>(aggregate)
                .ConfigureAwait(false);
        }

        public override string ToString()
        {
            return $"{GetType().Name} => {typeof(TAggregate).Name}";
        }

        protected abstract Task HandleOnAggregate(TCommand command, TAggregate aggregate);
    }
}
