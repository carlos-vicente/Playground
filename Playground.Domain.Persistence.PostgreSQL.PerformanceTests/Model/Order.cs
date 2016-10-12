using System;
using Playground.Domain.Model;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model
{
    internal class Order : AggregateRoot<OrderState>
    {
        public Order(Guid id) 
            : base(id)
        {
        }

        public Order(Guid id, OrderState hydratedState, long currentVersion) 
            : base(id, hydratedState, currentVersion)
        {
        }

        public void CreateOrder(string userOrdering, string address, Guid productId)
        {
            When(new OrderCreated(Id, userOrdering, address, productId));
        }

        public void ChangeAddress(string address)
        {
            if (State.Status >= OrderStatus.Shipped)
                throw new CannotChangeOrderAddressException($"Cannot change address on order {Id} because it is {State.Status}");

            When(new OrderChanged(Id, address));
        }

        public void StartFulfilling()
        {
            if(State.Status != OrderStatus.Created)
                throw new CannotStartFulfillingException($"Cannot start fulfilling order {Id} because it is {State.Status}");

            When(new StartedFulfilment(Id));
        }

        public void Ship()
        {
            if (State.Status != OrderStatus.BeingFulfilled)
                throw new CannotShipException($"Cannot ship order {Id} because it is {State.Status}");

            When(new ShipOrder(Id));
        }

        public void Deliver(string personWhoReceived)
        {
            if (State.Status != OrderStatus.Shipped)
                throw new CannotDeliverException($"Cannot deliver order {Id} because it is {State.Status}");

            When(new OrderDelivered(Id, personWhoReceived));
        }
    }
}
