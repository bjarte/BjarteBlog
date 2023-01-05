using Core.Features.Phone;
using Xunit;

namespace Core.Tests;

public class PhoneNumberExtensionsTests
{
    [Fact]
    public void CleanPhoneNumber_WhenPhoneNumberIsNull_ShouldReturnEmpty()
    {
        Assert.Equal(string.Empty, PhoneNumberExtensions.CleanPhoneNumber(null));
    }

    [Fact]
    public void CleanPhoneNumber_WhenPhoneNumberIsEmpty_ShouldReturnEmpty()
    {
        Assert.Equal(string.Empty, string.Empty.CleanPhoneNumber());
    }

    [Fact]
    public void CleanPhoneNumber_WhenPhoneNumberHasOnlySpecialCharacters_ShouldReturnEmpty()
    {
        const string phoneNumber = "\".\"";

        Assert.Equal(string.Empty, phoneNumber.CleanPhoneNumber());
    }

    [Fact]
    public void CleanPhoneNumber_WhenPhoneNumberHasOnlyCharacters_ShouldReturnEmpty()
    {
        const string phoneNumber = "abcxxx";

        Assert.Equal(string.Empty, phoneNumber.CleanPhoneNumber());
    }

    [Fact]
    public void CleanPhoneNumber_WhenPhoneNumberHasCharactersAndSpecialCharacters_ShouldReturnOnlyDigits()
    {
        const string phoneNumber = "x\"2,klk22kk0!";

        Assert.Equal("2220", phoneNumber.CleanPhoneNumber());
    }

    [Fact]
    public void CleanPhoneNumber_WhenPhoneNumberBeginsWith00_ShouldRemove00()
    {
        const string phoneNumber = "0012345678";

        Assert.Equal("12345678", phoneNumber.CleanPhoneNumber());
    }

    [Fact]
    public void CleanPhoneNumber_WhenNorwegianPhoneNumberContainsCountryCode_ShouldRemoveCountryCode()
    {
        const string phoneNumber = "004712345678";

        Assert.Equal("12345678", phoneNumber.CleanPhoneNumber());
    }

    [Fact]
    public void CleanPhoneNumber_WhenPhoneNumberBeginsWith0_ShouldRemove0()
    {
        const string phoneNumber = "012345678";

        Assert.Equal("12345678", phoneNumber.CleanPhoneNumber());
    }

    [Fact]
    public void CleanPhoneNumber_When5DigitPhoneNumberBeginsWith0_ShouldNotRemove0()
    {
        const string phoneNumber = "01234";

        Assert.Equal("01234", phoneNumber.CleanPhoneNumber());
    }

    [Fact]
    public void CleanPhoneNumber_PhoneNumberContainsInvalidCharacters_InvalidCharactersShouldBeRemoved()
    {
        Assert.Equal("22334455", "004722334455".CleanPhoneNumber());
        Assert.Equal("22334455", "0047223v34o455".CleanPhoneNumber());
        Assert.Equal("23232323", "4723232323x".CleanPhoneNumber());
        Assert.Equal("03232", "03232".CleanPhoneNumber());
        Assert.Equal("323232", "0323232".CleanPhoneNumber());
        Assert.Equal("4544556677", "004544556677".CleanPhoneNumber());
        Assert.Equal("4533445566", "+4533445566".CleanPhoneNumber());
        Assert.Equal("1234", "1234".CleanPhoneNumber());
        Assert.Equal("98765432", "+4798765432".CleanPhoneNumber());
    }
}