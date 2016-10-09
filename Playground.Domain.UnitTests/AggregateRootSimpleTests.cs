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
            IGetAppliedWith<ItHappened>
        {
            public bool ApplyCalled { get; set; }

            public TestAggregateRoot(Guid id) 
                : base(id)
            {
                ApplyCalled = false;
            }

            void IGetAppliedWith<ItHappened>.Apply(ItHappened evt)
            {
                // do something to apply the event to the aggregate root
                ApplyCalled = true;
            }

            public void MakeItHappen(string name)
            {
                When(new ItHappened(Id)
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

            var expectedEvent = new ItHappened(_sut.Id)
            {
                Name = name
            };

            // act
            _sut.MakeItHappen(name);

            // assert
            _sut
                .UncommittedEvents
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