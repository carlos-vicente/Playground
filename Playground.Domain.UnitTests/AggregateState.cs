using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.UnitTests.Events;

namespace Playground.Domain.UnitTests
{
    public class AggregateState
        : IAggregateState,
            IGetAppliedWith<ItHappened>,
            IGetAppliedWith<GotDone>,
            IGetAppliedWith<SomethingGotChanged>
    {
        public bool AppliedItHappened { get; private set; }
        public bool AppliedGotDone { get; private set; }

        public string SomethingNotSetItHappenedNorGotDone { get; set; }

        public void Apply(ItHappened e)
        {
            AppliedItHappened = true;
        }

        public void Apply(GotDone e)
        {
            AppliedGotDone = true;
        }

        public void Apply(SomethingGotChanged e)
        {
            SomethingNotSetItHappenedNorGotDone = e.Something;
        }
    }
}