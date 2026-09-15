using ChequeWriter;

namespace ChequeWriter.Tests;

public class ChequeAmountConverterTests
{
    [Theory]
    [InlineData(
        "1234.56",
        "One thousand, two hundred and thirty-four dollars and fifty-six cents")]
    [InlineData(
        "0",
        "Zero dollars and zero cents")]
    [InlineData(
        "1.01",
        "One dollar and one cent")]
    [InlineData(
        "0.05",
        "Zero dollars and five cents")]
    [InlineData(
        "21.00",
        "Twenty-one dollars and zero cents")]
    [InlineData(
        "101.00",
        "One hundred and one dollars and zero cents")]
    [InlineData(
        "1001.00",
        "One thousand and one dollars and zero cents")]
    [InlineData(
        "1000001.99",
        "One million and one dollars and ninety-nine cents")]
    [InlineData(
        "12.344",
        "Twelve dollars and thirty-four cents")]
    [InlineData(
        "12.345",
        "Twelve dollars and thirty-five cents")]
    [InlineData(
        "12.999",
        "Thirteen dollars and zero cents")]
    [InlineData(
        "999999999999999.99",
        "Nine hundred and ninety-nine trillion, nine hundred and ninety-nine billion, nine hundred and ninety-nine million, nine hundred and ninety-nine thousand, nine hundred and ninety-nine dollars and ninety-nine cents")]
    public void Convert_ReturnsExpectedWording(string amountText, string expected)
    {
        decimal amount = decimal.Parse(amountText, System.Globalization.CultureInfo.InvariantCulture);
        string actual = ChequeAmountConverter.Convert(amount);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Convert_RejectsNegativeAmounts()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => ChequeAmountConverter.Convert(-1.00m));

        Assert.Contains("cannot be negative", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Convert_RejectsAmountAboveMaximum()
    {
        decimal aboveMax = ChequeAmountConverter.MaxAmount + 0.01m;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => ChequeAmountConverter.Convert(aboveMax));

        Assert.Contains("maximum supported amount", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("1234567", "One million, two hundred and thirty-four thousand, five hundred and sixty-seven dollars and zero cents")]
    public void Convert_FormatsLargeGroupedAmounts(string amountText, string expected)
    {
        decimal amount = decimal.Parse(amountText, System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(expected, ChequeAmountConverter.Convert(amount));
    }
}
