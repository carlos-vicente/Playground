using Playground.Domain.Events;
using Playground.Domain.UnitTests.Events;

namespace Playground.Domain.UnitTests
{
    public class AggregateState
        : IGetAppliedWith<ItHappened>,
        IGetAppliedWith<GotDone>
    {
        public bool AppliedItHappened { get; private set; }
        public bool AppliedGotDone { get; private set; }

        public void Apply(ItHappened e)
        {
            AppliedItHappened = true;
        }

        public void Apply(GotDone e)
        {
            AppliedGotDone = true;
        }
    }
}