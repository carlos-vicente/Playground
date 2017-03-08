using System;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model
{
    public class OrderIdentity : IIdentity
    {
        private readonly Guid _orderId;

        public OrderIdentity(Guid orderId)
        {
            _orderId = orderId;
        }

        public string Id => _orderId.ToString();
    }
}