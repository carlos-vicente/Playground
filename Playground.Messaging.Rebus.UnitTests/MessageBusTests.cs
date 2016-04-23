using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Playground.Messaging.Commands;
using Playground.Messaging.Rebus.UnitTests.Model;
using Playground.Tests;
using Ploeh.AutoFixture;
using Rebus.Bus;

namespace Playground.Messaging.Rebus.UnitTests
{
    public class MessageBusTests: TestBase
    {
        private MessageBus _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = Faker.Resolve<MessageBus>();
        }

        [Test]
        public async Task SendCommand_WillSendComandToRebus()
        {
            // arrange
            var command = Fixture.Create<TestCommand>();

            // act
            await _sut
                .SendCommand(command)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IBus>().Send(command, null))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
