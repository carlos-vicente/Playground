using System;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Tests;
using System.Linq;
using FluentAssertions;
using Playground.Domain.UnitTests.Events;
using Ploeh.AutoFixture;

namespace Playground.Domain.UnitTests
{
    public class AggregateRootTests : SimpleTestBase
    {
        public class TestAggregateState 
            : IAggregateState, 
            IGetAppliedWith<ItHappened>
        {
            public bool ApplyCalled { get; set; }

            void IGetAppliedWith<ItHappened>.Apply(ItHappened e)
            {
                ApplyCalled = true;
            }
        }

        public class TestAggregateRootWithState
            : AggregateRoot<TestAggregateState>
        {

            public TestAggregateRootWithState(Guid id) 
                : base(id)
            {

            }

            public void DoIt(string name)
            {
                When(new ItHappened(Id)
                {
                    Name = name
                });
            }
        }

        private TestAggregateRootWithState _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = new TestAggregateRootWithState(Guid.NewGuid());
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
            _sut.DoIt(name);

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
                .State
                .ApplyCalled
                .Should()
                .BeTrue();
        }
    }
}