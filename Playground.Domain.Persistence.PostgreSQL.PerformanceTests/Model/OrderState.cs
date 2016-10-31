using System;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model
{
    public class OrderState
        : IAggregateState,
        IGetAppliedWith<OrderCreated>,
        IGetAppliedWith<OrderShippingAddressChanged>,
        IGetAppliedWith<StartedFulfilment>,
        IGetAppliedWith<ShipOrder>,
        IGetAppliedWith<OrderDelivered>
    {
        public string UserOrdering { get; private set; }

        public string ShippingAddress { get; private set; }

        public Guid ProductIdToSend { get; private set; }

        public OrderStatus Status { get; private set; }

        public string PersonWhoReceivedOrder { get; private set; }

        public OrderState()
        {
            
        }

        // For testing purposes only
        public OrderState(
            string userOrdering,
            string shippingAddress,
            Guid productIdToSend,
            OrderStatus status,
            string personWhoReceivedOrder)
        {
            UserOrdering = userOrdering;
            ShippingAddress = shippingAddress;
            ProductIdToSend = productIdToSend;
            Status = status;
            PersonWhoReceivedOrder = personWhoReceivedOrder;
        }

        #region IGetAppliedWith Events
        void IGetAppliedWith<OrderCreated>.Apply(OrderCreated e)
        {
            UserOrdering = e.UserOrdering;
            ShippingAddress = e.Address;
            ProductIdToSend = e.ProductId;
            Status = OrderStatus.Created;
        }

        void IGetAppliedWith<OrderShippingAddressChanged>.Apply(OrderShippingAddressChanged e)
        {
            ShippingAddress = e.NewAddress;
        }

        void IGetAppliedWith<StartedFulfilment>.Apply(StartedFulfilment e)
        {
            Status = OrderStatus.BeingFulfilled;
        }

        void IGetAppliedWith<ShipOrder>.Apply(ShipOrder e)
        {
            Status = OrderStatus.Shipped;
        }

        void IGetAppliedWith<OrderDelivered>.Apply(OrderDelivered e)
        {
            Status = OrderStatus.Delivered;
            PersonWhoReceivedOrder = e.PersonWhoReceived;
        }
        #endregion
    }
}