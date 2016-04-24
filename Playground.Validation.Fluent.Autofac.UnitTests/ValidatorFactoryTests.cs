using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using Playground.DependencyResolver.Contracts;
using Playground.Tests;

namespace Playground.Validation.Fluent.Autofac.UnitTests
{
    public class ValidatorFactoryTests : TestBaseWithSut<ValidatorFactory>
    {
        [Test]
        public void GenericGetValidator_WillGetIValidatorFromDependencyResolver()
        {
            // arrange
            var expectedValidator = Faker.Resolve<IValidator<string>>();

            A.CallTo(() => Faker.Resolve<IDependencyResolver>()
                .Resolve<IValidator<string>>())
                .Returns(expectedValidator);

            // act
            var actualValidator = Sut.GetValidator<string>();

            // assert
            actualValidator
                .Should()
                .Be(expectedValidator);
        }

        [Test]
        public void TypedGetValidator_WillGetIValidatorFromDependencyResolver()
        {
            // arrange
            var type = typeof (string);
            var expectedValidator = Faker.Resolve<IValidator>();

            A.CallTo(() => Faker.Resolve<IDependencyResolver>()
                .Resolve(type))
                .Returns(expectedValidator);

            // act
            var actualValidator = Sut.GetValidator(type);

            // assert
            actualValidator
                .Should()
                .Be(expectedValidator);
        }
    }
}
