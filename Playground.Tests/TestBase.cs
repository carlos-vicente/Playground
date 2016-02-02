using Autofac.Extras.FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Playground.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected AutoFake Faker;
        protected Fixture Fixture;

        [SetUp]
        public virtual void SetUp()
        {
            Faker = new AutoFake();
            Fixture = new Fixture();
        }
    }
}
