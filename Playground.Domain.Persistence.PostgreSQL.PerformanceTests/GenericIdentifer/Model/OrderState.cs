using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model
{
    public class OrderState
        : IAggregateState,
        IGetAppliedWithForAggregateWithIdentity<OrderCreated>,
        IGetAppliedWithForAggregateWithIdentity<OrderShippingAddressChanged>,
        IGetAppliedWithForAggregateWithIdentity<OrderStartedBeingFulfilled>,
        IGetAppliedWithForAggregateWithIdentity<OrderShipped>,
        IGetAppliedWithForAggregateWithIdentity<OrderDelivered>
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
        void IGetAppliedWithForAggregateWithIdentity<OrderCreated>.Apply(OrderCreated e)
        {
            UserOrdering = e.UserOrdering;
            ShippingAddress = e.Address;
            ProductIdToSend = e.ProductId;
            Status = OrderStatus.Created;
        }

        void IGetAppliedWithForAggregateWithIdentity<OrderShippingAddressChanged>.Apply(OrderShippingAddressChanged e)
        {
            ShippingAddress = e.NewAddress;
        }

        void IGetAppliedWithForAggregateWithIdentity<OrderStartedBeingFulfilled>.Apply(OrderStartedBeingFulfilled e)
        {
            Status = OrderStatus.BeingFulfilled;
        }

        void IGetAppliedWithForAggregateWithIdentity<OrderShipped>.Apply(OrderShipped e)
        {
            Status = OrderStatus.Shipped;
        }

        void IGetAppliedWithForAggregateWithIdentity<OrderDelivered>.Apply(OrderDelivered e)
        {
            Status = OrderStatus.Delivered;
            PersonWhoReceivedOrder = e.PersonWhoReceived;
        }
        #endregion
    }
}