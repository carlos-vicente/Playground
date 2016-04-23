using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Playground.Messaging.Rebus.UnitTests.Model;
using Playground.Tests;
using Ploeh.AutoFixture;
using Rebus.Bus;

namespace Playground.Messaging.Rebus.UnitTests
{
    public class MessageBusTests: TestBaseWithSut<MessageBus>
    {
        [Test]
        public async Task SendCommand_WillSendComandToRebus()
        {
            // arrange
            var command = Fixture.Create<TestCommand>();

            // act
            await Sut
                .SendCommand(command)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IBus>().Send(command, null))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
