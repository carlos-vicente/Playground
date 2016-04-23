using System;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using Playground.Tests;

namespace Playground.Validation.Fluent.UnitTests
{
    public class FluentValidationValidatorFactoryTests 
        : TestBaseWithSut<FluentValidationValidatorFactory>
    {
        [Test]
        public void GenericCreateValidator_WillReturnValidator()
        {
            // arrange

            // act
            var validator = Sut.CreateValidator<string>();

            // assert
            validator
                .Should()
                .BeAssignableTo<FluentValidationValidator<string>>();
            
            A.CallTo(() => Faker.Resolve<FluentValidation.IValidatorFactory>()
                .GetValidator<string>())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void GenericCreateValidator_WillThrowException_WhenFluentValidatorDoesNotExist()
        {
            // arrange
            A.CallTo(() => Faker.Resolve<FluentValidation.IValidatorFactory>()
                .GetValidator<string>())
                .Returns(null as IValidator<string>);

            Action exceptionThrower = () => Sut.CreateValidator<string>();

            // act/assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void CreateValidator_WillReturnValidator()
        {
            // arrange
            var type = typeof (string);

            // act
            var validator = Sut.CreateValidator(type);

            // assert
            validator
                .Should()
                .BeAssignableTo<FluentValidationValidator>();

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidatorFactory>()
                .GetValidator(type))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CreateValidator_WillThrowException_WhenFluentValidatorDoesNotExist()
        {
            // arrange
            var type = typeof (string);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidatorFactory>()
                .GetValidator(type))
                .Returns(null as IValidator);

            Action exceptionThrower = () => Sut.CreateValidator(type);

            // act/assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>();
        }
    }
}