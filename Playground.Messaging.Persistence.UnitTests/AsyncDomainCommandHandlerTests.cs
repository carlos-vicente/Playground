using System;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Playground.Domain.Persistence;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Messaging.Persistence.UnitTests
{
    public class AsyncDomainCommandHandlerTests : SimpleTestBase
    {
        private TestHandler _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = Faker.Resolve<TestHandler>();
        }

        [Test]
        public async Task Handle_WillCreateANewAggregate_WhenThereIsNoneWithTheGivenIdentifier()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();
            var command = Fixture.Create<Command>();
            var aggregateRoot = Fixture.Create<Aggregate>();

            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .TryLoad<Aggregate>(aggregateRootId))
                .Returns(null as Aggregate);

            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .Create<Aggregate>(aggregateRootId))
                .Returns(aggregateRoot);

            // act
            await _sut
                .Handle(command)
                .ConfigureAwait(false);

            // assert
            
        }
    }
}