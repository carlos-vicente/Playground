using System;
using System.Data;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Data.Dapper.Tests
{
    [TestFixture]
    public class ConnectionFactorySimpleTests : SimpleTestBase
    {
        [Test]
        public void CreateConnection_ReturnsConnectionAbstraction_WithObtainedConnection()
        {
            // arrange
            var connectionString = Fixture.Create<string>();
            var expectedConnection = Faker.Resolve<IDbConnection>();

            var fakeBuilder = A.Fake<Func<string, IDbConnection>>();

            A.CallTo(() => fakeBuilder(connectionString))
                .Returns(expectedConnection);

            var sut = Faker.Resolve<ConnectionFactory>(
                new NamedParameter("connectionString", connectionString),
                new NamedParameter("connectionBuilderFunc", fakeBuilder));

            // act
            var actual = sut.CreateConnection();

            // assert
            A.CallTo(() => fakeBuilder(connectionString))
                .MustHaveHappened(Repeated.AtLeast.Once);

            actual
                .As<Connection>()
                .InnerConnection
                .ShouldBeEquivalentTo(expectedConnection);

            A.CallTo(() => expectedConnection.Open())
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}