using System;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Tests;
using System.Linq;
using FluentAssertions;
using Ploeh.AutoFixture;

namespace Playground.Domain.UnitTests
{
    public class AggregateRootSimpleTests : SimpleTestBase
    {
        public class TestAggregateRoot 
            : AggregateRoot,
            IEmit<Event1>
        {
            public bool ApplyCalled { get; set; }

            public TestAggregateRoot(Guid id) 
                : base(id)
            {
                ApplyCalled = false;
            }

            void IEmit<Event1>.Apply(Event1 evt)
            {
                // do something to apply the event to the aggregate root
                ApplyCalled = true;
            }

            public void ItHappened(string name)
            {
                When(new Event1(Id)
                {
                    Name = name
                });
            }
        }

        private TestAggregateRoot _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = new TestAggregateRoot(Guid.NewGuid());
        }

        [Test]
        public void When_WillApplyADomainEventOnToTheAggregateRoot()
        {
            // arrange
            var name = Fixture.Create<string>();

            var expectedEvent = new Event1(_sut.Id)
            {
                Name = name
            };

            // act
            _sut.ItHappened(name);

            // assert
            _sut
                .Events
                .Should()
                .ContainSingle()
                .And
                .Subject
                .Single()
                .ShouldBeEquivalentTo(
                    expectedEvent,
                    opts => opts
                        .Excluding(de => de.Metadata.OccorredOn)
                        .Excluding(de => de.Metadata.StorageVersion));

            _sut
                .ApplyCalled
                .Should()
                .BeTrue();
        }
    }
}