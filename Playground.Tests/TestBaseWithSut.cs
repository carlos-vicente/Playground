namespace Playground.Tests
{
    public abstract class TestBaseWithSut<TSut> : SimpleTestBase
    {
        protected TSut Sut;

        public override void SetUp()
        {
            base.SetUp();

            Sut = Faker.Resolve<TSut>();
        }
    }
}