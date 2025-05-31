using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Validations;

namespace TestPetFamilyDomain;

public class ValidationExtensionsTests
{
    [Theory]
    [InlineData(null, "Name", 10)] // Null value
    [InlineData("", "Name", 10)] // Empty string
    [InlineData("    ", "Name", 10)] // Whitespace only
    public void ValidateRequiredField_ShouldFail_WhenValueIsNullOrEmpty(
        string? value,
        string valueName,
        int maxLength)
    {
        var result = ValidationExtensions.ValidateRequiredField(value, valueName, maxLength);
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("ValidName", "Name", 10)]
    [InlineData("   Valid  ", "Name", 10)]
    public void ValidateRequiredField_ShouldPass_WhenValueIsValid(string value, string valueName, int maxLength)
    {
        var result = ValidationExtensions.ValidateRequiredField(value, valueName, maxLength);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("TooLongValue", "Name", 5)]
    public void ValidateRequiredField_ShouldFail_WhenValueExceedsMaxLength(
        string value,
        string valueName,
        int maxLength)
    {
        var result = ValidationExtensions.ValidateRequiredField(value, valueName, maxLength);
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("Valid123", "Name", 10, "^[A-Za-z0-9]+$")] // Matches pattern
    [InlineData("Invalid@#", "Name", 10, "^[A-Za-z0-9]+$")] // Invalid pattern
    public void ValidateRequiredField_ShouldFail_WhenPatternDoesNotMatch(
        string value,
        string valueName,
        int maxLength,
        string pattern)
    {
        var result = ValidationExtensions.ValidateRequiredField(value, valueName, maxLength, pattern);
        Assert.Equal(value == "Invalid@#", result.IsFailure);
        Assert.Equal(value == "Valid123", result.IsSuccess);
    }

    [Theory]
    [InlineData(5, "Age", 1, 10)] // Within range
    [InlineData(0, "Age", 1, 10)] // Below range
    [InlineData(11, "Age", 1, 10)] // Above range
    public void ValidateIntegerNumber_ShouldPassOrFail_BasedOnRange(
        int number,
        string valueName,
        int minValue,
        int maxValue)
    {
        var result = ValidationExtensions.ValidateIntegerNumber(number, valueName, minValue, maxValue);
        Assert.Equal(number >= minValue && number <= maxValue, result.IsSuccess);
    }

    [Theory]
    [InlineData(5.5, "Price", 1.0, 10.0)] // Valid float
    [InlineData(11.0, "Price", 1.0, 10.0)] // Out of range
    public void ValidateNumber_ShouldPassOrFail_BasedOnRange(
        double number,
        string valueName,
        double minValue,
        double maxValue)
    {
        var result = ValidationExtensions.ValidateNumber(number, valueName, minValue, maxValue);
        Assert.Equal(number >= minValue && number <= maxValue, result.IsSuccess);
    }

    [Fact]
    public void ValidateRequiredObject_ShouldFail_WhenNull()
    {
        object? obj = null;
        var result = ValidationExtensions.ValidateRequiredObject(obj, "TestObject");
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ValidateRequiredObject_ShouldPass_WhenNotNull()
    {
        object obj = new();
        var result = ValidationExtensions.ValidateRequiredObject(obj, "TestObject");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void HasOnlyEmptyStrings_ShouldReturnTrue_WhenAllStringsAreEmpty()
    {
        bool result = ValidationExtensions.HasOnlyEmptyStrings("", "  ", null);
        Assert.True(result);
    }

    [Fact]
    public void HasOnlyEmptyStrings_ShouldReturnFalse_WhenAtLeastOneStringIsNotEmpty()
    {
        bool result = ValidationExtensions.HasOnlyEmptyStrings("", "  ", "Valid");
        Assert.False(result);
    }

    [Fact]
    public void ValidateItems_ShouldPass_WhenAllItemsAreValid()
    {
        var items = new List<string> { "Valid1", "Valid2" };
        var result = ValidationExtensions.ValidateItems(items, item => UnitResult.Ok());
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ValidateItems_ShouldFail_WhenAnyItemIsInvalid()
    {
        var items = new List<string> { "Valid1", "", "Valid2" };
        var result = ValidationExtensions.ValidateItems(
            items, item => string.IsNullOrWhiteSpace(item)
                ? UnitResult.Fail(Error.InvalidFormat("TestField"))
                : UnitResult.Ok());
        Assert.False(result.IsSuccess);
    }
}
