using System;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.DependencyResolver.Contracts;
using Playground.QueryService.Contracts;
using Playground.QueryService.InMemory.UnitTests.TestModel;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.QueryService.InMemory.UnitTests
{
    public class QueryServiceTests : TestBase
    {
        private QueryService _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = Faker.Resolve<QueryService>();
        }

        [Test]
        public void Query_WillExecuteQueryHandler_WithReceivedQuery()
        {
            // arrange
            var query = Fixture.Create<TestQuery>();
            var queryResult = Fixture.Create<TestQueryResult>();
            var queryHandler = Faker.Resolve<IQueryHandler<TestQuery, TestQueryResult>>();

            A.CallTo(() => Faker.Resolve<IDependencyResolver>()
                .Resolve<IQueryHandler<TestQuery, TestQueryResult>>())
                .Returns(queryHandler);

            A.CallTo(() => queryHandler
                .Handle(query))
                .Returns(queryResult);

            // act
            var actual = _sut.Query<TestQuery, TestQueryResult>(query);

            // assert
            actual
                .ShouldBeEquivalentTo(queryResult);
        }

        [Test]
        public void Query_WillThrowException_WhenNoQueryHandlerExists()
        {
            // arrange
            var query = Fixture.Create<TestQuery>();

            A.CallTo(() => Faker.Resolve<IDependencyResolver>()
                .Resolve<IQueryHandler<TestQuery, TestQueryResult>>())
                .Returns(null);

            Action expectionThrower = () => _sut.Query<TestQuery, TestQueryResult>(query);

            // act & assert
            expectionThrower
                .ShouldThrow<InvalidOperationException>()
                .And
                .Message
                .Should()
                .Contain($"Failed to resolve implementation of {typeof(IQueryHandler<TestQuery, TestQueryResult>)}");
        }

        [Test]
        public async Task QueryAsync_WillExecuteAsyncQueryHandler_WithReceivedQuery()
        {
            // arrange
            var query = Fixture.Create<TestQuery>();
            var queryResult = Fixture.Create<TestQueryResult>();
            var asyncQueryHandler = Faker.Resolve<IAsyncQueryHandler<TestQuery, TestQueryResult>>();

            A.CallTo(() => Faker.Resolve<IDependencyResolver>()
                .Resolve<IAsyncQueryHandler<TestQuery, TestQueryResult>>())
                .Returns(asyncQueryHandler);

            A.CallTo(() => asyncQueryHandler
                .Handle(query))
                .Returns(queryResult);

            // act
            var actual = await _sut
                .QueryAsync<TestQuery, TestQueryResult>(query)
                .ConfigureAwait(false);

            // assert
            actual
                .ShouldBeEquivalentTo(queryResult);
        }

        [Test]
        public void QueryAsync_WillThrowException_WhenNoAyncQueryHandlerExists()
        {
            // arrange
            var query = Fixture.Create<TestQuery>();

            A.CallTo(() => Faker.Resolve<IDependencyResolver>()
                .Resolve<IAsyncQueryHandler<TestQuery, TestQueryResult>>())
                .Returns(null);

            Func<Task> expectionThrower = async () => await _sut
                .QueryAsync<TestQuery, TestQueryResult>(query)
                .ConfigureAwait(false);

            // act & assert
            expectionThrower
                .ShouldThrow<InvalidOperationException>()
                .And
                .Message
                .Should()
                .Contain($"Failed to resolve implementation of {typeof(IQueryHandler<TestQuery, TestQueryResult>)}");
        }
    }
}
