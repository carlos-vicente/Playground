using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Validation.Fluent.UnitTests
{
    public class FluentValidationValidatorTests: TestBaseWithSut<FluentValidationValidator>
    {
        [Test]
        public void Validate_WillCallValidateOnFluentValidationValidator()
        {
            // arrange
            object objToValidate = Fixture.Create<string>();
            var validResult = new ValidationResult();

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .Validate(objToValidate))
                .Returns(validResult);

            // act
            Sut.Validate(objToValidate);

            // assert
            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .Validate(objToValidate))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Validate_WillThrowException_WhenValidateOnFluentValidationValidatorFails()
        {
            // arrange
            object objToValidate = Fixture.Create<string>();
            var expectedFailures = new[] { new ValidationFailure(string.Empty, "error") };

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .Validate(objToValidate))
                .Returns(new ValidationResult(expectedFailures));

            Action expectionThrower = () => Sut.Validate(objToValidate);

            var expectedErrors = expectedFailures.Select(f => f.ErrorMessage);

            // act/assert
            expectionThrower
                .ShouldThrow<Core.Validation.ValidationException>()
                .And
                .Errors
                .ShouldAllBeEquivalentTo(expectedErrors);
        }

        [Test]
        public void ValidateAll_WillCallValidateOnFluentValidationValidator_ForEachObject()
        {
            // arrange
            var objsToValidate = Fixture
                .CreateMany<string>()
                .Cast<object>()
                .ToArray();
            var validResult = new ValidationResult();

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .CanValidateInstancesOfType(A<Type>._))
                .Returns(true);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .Validate(A<string>.That.Matches(s => objsToValidate.Contains(s))))
                .Returns(validResult);

            // act
            Sut.ValidateAll(objsToValidate);

            // assert
            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .Validate(A<string>.That.Matches(s => objsToValidate.Contains(s))))
                .MustHaveHappened(Repeated.Exactly.Times(objsToValidate.Length));
        }

        [Test]
        public void ValidateAll_WillThrowExceptionWithAggregateErrors_ForTheFailedResults()
        {
            // arrange
            var validObjects = Fixture
                .CreateMany<string>()
                .Cast<object>()
                .ToList();

            var failedObjects = Fixture
                .CreateMany<string>()
                .Cast<object>()
                .ToList();

            var validResult = new ValidationResult();

            var failedResult = new ValidationResult(new[]
            {
                new ValidationFailure(string.Empty, "error")
            });

            var objectsToValidate = new List<object>();
            objectsToValidate.AddRange(validObjects);
            objectsToValidate.AddRange(failedObjects);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .CanValidateInstancesOfType(A<Type>._))
                .Returns(true);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .Validate(A<string>.That.Matches(s => validObjects.Contains(s))))
                .Returns(validResult);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .Validate(A<string>.That.Matches(s => failedObjects.Contains(s))))
                .Returns(failedResult);

            Action exceptionThrower = () => Sut.ValidateAll(objectsToValidate.ToArray());

            // act/assert
            exceptionThrower
                .ShouldThrow<Core.Validation.ValidationException>()
                .And
                .Errors
                .Should()
                .HaveSameCount(failedObjects);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .Validate(A<string>.That.Matches(s =>
                    validObjects.Contains(s)
                    || failedObjects.Contains(s))))
                .MustHaveHappened(Repeated.Exactly.Times(validObjects.Count + failedObjects.Count));
        }

        [Test]
        public void ValidateAll_WillThrowException_WhenAnyObjectCanNotBeValidated()
        {
            // arrange
            var objectsToValidate = Fixture
                .CreateMany<string>()
                .Cast<object>()
                .ToList();

            var validType = objectsToValidate
                .Select(obj => obj.GetType())
                .Distinct()
                .Single();

            var invalidObject = new object();

            objectsToValidate.Add(invalidObject);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .CanValidateInstancesOfType(A<Type>.That.Matches(t => t == validType)))
                .Returns(true);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .CanValidateInstancesOfType(A<Type>.That.Matches(t => t == invalidObject.GetType())))
                .Returns(false);

            Action exceptionThrower = () => Sut.ValidateAll(objectsToValidate.ToArray());

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator>()
                .Validate(A<object>._))
                .MustNotHaveHappened();
        }
    }

    public class GenericFluentValidationValidatorTests 
        : TestBaseWithSut<FluentValidationValidator<string>>
    {
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void Validate_WillCallValidateOnFluentValidationValidator()
        {
            // arrange
            var objToValidate = Fixture.Create<string>();
            var validResult = new ValidationResult();

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .Validate(objToValidate))
                .Returns(validResult);

            // act
            Sut.Validate(objToValidate);

            // assert
            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .Validate(objToValidate))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Validate_WillThrowException_WhenValidateOnFluentValidationValidatorFails()
        {
            // arrange
            var objToValidate = Fixture.Create<string>();
            var expectedFailures = new[] { new ValidationFailure(string.Empty, "error") };

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .Validate(objToValidate))
                .Returns(new ValidationResult(expectedFailures));

            Action expectionThrower = () => Sut.Validate(objToValidate);

            var expectedErrors = expectedFailures.Select(f => f.ErrorMessage);

            // act/assert
            expectionThrower
                .ShouldThrow<Core.Validation.ValidationException>()
                .And
                .Errors
                .ShouldAllBeEquivalentTo(expectedErrors);
        }

        [Test]
        public void ValidateAll_WillCallValidateOnFluentValidationValidator_ForEachObject()
        {
            // arrange
            var objsToValidate = Fixture
                .CreateMany<string>()
                .ToArray();
            var validResult = new ValidationResult();

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .CanValidateInstancesOfType(A<Type>._))
                .Returns(true);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .Validate(A<string>.That.Matches(s => objsToValidate.Contains(s))))
                .Returns(validResult);

            // act
            Sut.ValidateAll(objsToValidate);

            // assert
            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .Validate(A<string>.That.Matches(s => objsToValidate.Contains(s))))
                .MustHaveHappened(Repeated.Exactly.Times(objsToValidate.Length));
        }

        [Test]
        public void ValidateAll_WillThrowExceptionWithAggregateErrors_ForTheFailedResults()
        {
            // arrange
            var validObjects = Fixture
                .CreateMany<string>()
                .ToList();

            var failedObjects = Fixture
                .CreateMany<string>()
                .ToList();

            var validResult = new ValidationResult();

            var failedResult = new ValidationResult(new[]
            {
                new ValidationFailure(string.Empty, "error")
            });

            var objectsToValidate = new List<string>();
            objectsToValidate.AddRange(validObjects);
            objectsToValidate.AddRange(failedObjects);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .CanValidateInstancesOfType(A<Type>._))
                .Returns(true);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .Validate(A<string>.That.Matches(s => validObjects.Contains(s))))
                .Returns(validResult);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .Validate(A<string>.That.Matches(s => failedObjects.Contains(s))))
                .Returns(failedResult);

            Action exceptionThrower = () => Sut.ValidateAll(objectsToValidate.ToArray());

            // act/assert
            exceptionThrower
                .ShouldThrow<Core.Validation.ValidationException>()
                .And
                .Errors
                .Should()
                .HaveSameCount(failedObjects);

            A.CallTo(() => Faker.Resolve<FluentValidation.IValidator<string>>()
                .Validate(A<string>.That.Matches(s =>
                    validObjects.Contains(s)
                    || failedObjects.Contains(s))))
                .MustHaveHappened(Repeated.Exactly.Times(validObjects.Count + failedObjects.Count));
        }
    }
}