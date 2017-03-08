using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier.Model.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier.Model
{
    public class OrderState
        : IAggregateState,
        IGetAppliedWith<OrderCreated>,
        IGetAppliedWith<OrderShippingAddressChanged>,
        IGetAppliedWith<OrderStartedBeingFulfilled>,
        IGetAppliedWith<OrderShipped>,
        IGetAppliedWith<OrderDelivered>
    {
        public string UserOrdering { get; set; }

        public string ShippingAddress { get; set; }

        public string ProductIdToSend { get; set; }

        public OrderStatus Status { get; set; }

        public string PersonWhoReceivedOrder { get; set; }

        public OrderState()
        {
            
        }

        // For testing purposes only
        public OrderState(
            string userOrdering,
            string shippingAddress,
            string productIdToSend,
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

        void IGetAppliedWith<OrderStartedBeingFulfilled>.Apply(OrderStartedBeingFulfilled e)
        {
            Status = OrderStatus.BeingFulfilled;
        }

        void IGetAppliedWith<OrderShipped>.Apply(OrderShipped e)
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