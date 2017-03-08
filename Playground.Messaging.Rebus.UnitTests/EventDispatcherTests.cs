using System;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Playground.Messaging.Rebus.UnitTests.Model;
using Playground.Tests;
using Rebus.Bus;

namespace Playground.Messaging.Rebus.UnitTests
{
    public class EventDispatcherTests : TestBaseWithSut<EventDispatcher>
    {
        [Test]
        public async Task RaiseEvent_WillPublishEventOnBus()
        {
            // arrange
            var evt = new TestEvent();

            // act
            await Sut
                .RaiseEvent(evt)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IBus>()
                .Publish(evt, null))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}