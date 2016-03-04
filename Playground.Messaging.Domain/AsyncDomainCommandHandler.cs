using System.Threading.Tasks;
using Playground.Domain;

namespace Playground.Messaging.Domain
{
    public abstract class AsyncDomainCommandHandler<TCommand, TAggregate>
        : IAsyncCommandHandler<TCommand> 
        where TAggregate: AggregateRoot 
        where TCommand : DomainCommand<TAggregate>
    {
        private readonly IAggregateContext _aggregateContext;

        protected AsyncDomainCommandHandler(IAggregateContext aggregateContext)
        {
            _aggregateContext = aggregateContext;
        }

        public async Task Handle(TCommand command)
        {
            var aggregate = await _aggregateContext.TryLoad<TAggregate>(command.Metadata.AggregateRootId)
                            ?? await _aggregateContext.Create<TAggregate>(command.Metadata.AggregateRootId);

            await HandleOnAggregate(command, aggregate);
        }

        public override string ToString()
        {
            return $"{GetType().Name} => {typeof (TAggregate).Name}";
        }

        protected abstract Task HandleOnAggregate(TCommand command, TAggregate aggregate);
    }
}
