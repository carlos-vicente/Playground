using Playground.Messaging.Commands;

namespace Playground.Messaging.Rebus.UnitTests.Model
{
    public class TestCommand : ICommand
    {
        public string Name { get; set; }
    }
}