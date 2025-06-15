// Copyright 2024 TELUS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Protobuf;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using My.Package;
using Google.Protobuf.WellKnownTypes;

namespace ProtoValidate.Tests;

[TestFixture]
public class ValidatorTests
{
    [Test]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Validator((ValidatorOptions)null!));
    }

    [Test]
    public void Constructor_WithNullOptionsAccessor_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Validator((IOptions<ValidatorOptions>)null!));
    }

    [Test]
    public void Validate_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        var validator = new Validator();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null!, false));
    }

    [Test]
    public void GetEvaluatorDebugString_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        var validator = new Validator();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => validator.GetEvaluatorDebugString(null!));
    }

    [Test]
    public void Constructor_WithDefaultOptions_CreatesValidator()
    {
        // Act
        var validator = new Validator();

        // Assert
        Assert.That(validator, Is.Not.Null);
    }

    [Test]
    public void Constructor_WithCustomOptions_CreatesValidator()
    {
        // Arrange
        var options = new ValidatorOptions
        {
            PreLoadDescriptors = true,
            DisableLazy = true
        };

        // Act
        var validator = new Validator(options);

        // Assert
        Assert.That(validator, Is.Not.Null);
    }

    [Test]
    public void Constructor_WithOptionsAccessor_CreatesValidator()
    {
        // Arrange
        var options = new ValidatorOptions
        {
            PreLoadDescriptors = true,
            DisableLazy = true
        };
        var optionsAccessor = new OptionsWrapper<ValidatorOptions>(options);

        // Act
        var validator = new Validator(optionsAccessor);

        // Assert
        Assert.That(validator, Is.Not.Null);
    }

    private static Validator CreateTransactionValidator()
    {
        var options = new ValidatorOptions
        {
            FileDescriptors = new List<Google.Protobuf.Reflection.FileDescriptor> { TransactionReflection.Descriptor },
            PreLoadDescriptors = true
        };
        return new Validator(options);
    }

    private static Transaction CreateValidTransaction()
    {
        return new Transaction
        {
            Id = 1000,
            PurchaseDate = Timestamp.FromDateTime(DateTime.UtcNow),
            DeliveryDate = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Price = "$123.45"
        };
    }

    [Test]
    public void Validate_ValidTransaction_PassesValidation()
    {
        var validator = CreateTransactionValidator();
        var valid = CreateValidTransaction();
        var result = validator.Validate(valid, failFast: false);
        Assert.That(result.IsSuccess, Is.True, "Expected valid transaction to pass validation");
    }

    [Test]
    public void Validate_TransactionWithInvalidId_FailsValidation()
    {
        var validator = CreateTransactionValidator();
        var invalid = CreateValidTransaction();
        invalid.Id = 1;
        var result = validator.Validate(invalid, failFast: false);
        Assert.That(result.IsSuccess, Is.False, "Expected transaction with id too low to fail");
        Assert.That(result.Violations.Any(v => v.ConstraintId.Contains("uint64.gt")), Is.True);
    }

    [Test]
    public void Validate_TransactionWithInvalidPrice_FailsValidation()
    {
        var validator = CreateTransactionValidator();
        var invalid = CreateValidTransaction();
        invalid.Price = "123.45";
        var result = validator.Validate(invalid, failFast: false);
        Assert.That(result.IsSuccess, Is.False, "Expected transaction with invalid price to fail");
        Assert.That(result.Violations.Any(v => v.ConstraintId.Contains("transaction.price")), Is.True);
    }

    [Test]
    public void Validate_TransactionWithInvalidDeliveryDate_FailsValidation()
    {
        var validator = CreateTransactionValidator();
        var invalid = CreateValidTransaction();
        invalid.DeliveryDate = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var result = validator.Validate(invalid, failFast: false);
        Assert.That(result.IsSuccess, Is.False, "Expected transaction with delivery date before purchase date to fail");
        Assert.That(result.Violations.Any(v => v.ConstraintId.Contains("transaction.delivery_date")), Is.True);
    }
}

// Helper class for OptionsWrapper
public class OptionsWrapper<T> : IOptions<T> where T : class, new()
{
    public OptionsWrapper(T value)
    {
        Value = value;
    }

    public T Value { get; }
} 