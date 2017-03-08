using System;
using Playground.Domain.Model;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model
{
    internal class Order : AggregateRootWithIdentity<OrderState, OrderIdentity>
    {
        public Order(OrderIdentity id) 
            : base(id)
        {
        }

        public Order(OrderIdentity id, OrderState hydratedState, long currentVersion) 
            : base(id, hydratedState, currentVersion)
        {
        }

        public void CreateOrder(string userOrdering, string address, string productId)
        {
            When(new OrderCreated(userOrdering, address, productId));
        }

        public void ChangeAddress(string address)
        {
            if (State.Status >= OrderStatus.Shipped)
                throw new CannotChangeOrderAddressException($"Cannot change address on order {Identity.Id} because it is {State.Status}");

            When(new OrderShippingAddressChanged(address));
        }

        public void StartFulfilling()
        {
            if(State.Status != OrderStatus.Created)
                throw new CannotStartFulfillingException($"Cannot start fulfilling order {Identity.Id} because it is {State.Status}");

            When(new OrderStartedBeingFulfilled());
        }

        public void Ship()
        {
            if (State.Status != OrderStatus.BeingFulfilled)
                throw new CannotShipException($"Cannot ship order {Identity.Id} because it is {State.Status}");

            When(new OrderShipped());
        }

        public void Deliver(string personWhoReceived)
        {
            if (State.Status != OrderStatus.Shipped)
                throw new CannotDeliverException($"Cannot deliver order {Identity.Id} because it is {State.Status}");

            When(new OrderDelivered(personWhoReceived));
        }
    }
}
