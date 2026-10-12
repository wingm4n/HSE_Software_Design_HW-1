using VyshKat.Cli;


namespace VyshKat.Tests.Cli;


public class ConsoleIOTests
{
    [Fact]
    public void Constructor_RejectsNulls()
    {
        Assert.Throws<ArgumentNullException>(() => new ConsoleIO(null!, new StringWriter()));
        Assert.Throws<ArgumentNullException>(() => new ConsoleIO(new StringReader(""), null!));
    }


    [Fact]
    public void WriteError_PrefixesMessage()
    {
        var h = ConsoleHarness.WithInput();


        h.Io.WriteError("boom");


        Assert.Contains("[!] boom", h.Text);
    }


    [Fact]
    public void ReadLine_PrintsPrompt_AndReturnsTrimmedLine()
    {
        var h = ConsoleHarness.WithInput("   hello  ");


        var line = h.Io.ReadLine("Name: ");


        Assert.Equal("hello", line);
        Assert.Contains("Name: ", h.Text);
    }


    [Theory]
    [InlineData("cancel")]
    [InlineData("  CANCEL ")]
    public void ReadLine_ThrowsOperationCanceled_OnCancelKeyword(string typed)
    {
        var h = ConsoleHarness.WithInput(typed);


        Assert.Throws<OperationCanceledException>(() => h.Io.ReadLine("> "));
    }


    [Fact]
    public void ReadLine_ReturnsCancelAsText_WhenCancelIsNotAllowed()
    {
        var h = ConsoleHarness.WithInput("cancel");


        Assert.Equal("cancel", h.Io.ReadLine("> ", allowCancel: false));
    }


    [Fact]
    public void ReadLine_ThrowsEndOfStream_WhenInputIsClosed()
    {
        var h = ConsoleHarness.WithInput();


        Assert.Throws<EndOfStreamException>(() => h.Io.ReadLine("> "));
    }


    [Fact]
    public void ReadNonEmpty_AsksAgain_UntilValueIsEntered()
    {
        var h = ConsoleHarness.WithInput("", "   ", "S-001");


        var value = h.Io.ReadNonEmpty("Number: ");


        Assert.Equal("S-001", value);
        Assert.Contains("Value cannot be empty.", h.Text);
    }


    [Fact]
    public void ReadInt_ReturnsValue_WhenInRange()
    {
        var h = ConsoleHarness.WithInput("7");


        Assert.Equal(7, h.Io.ReadInt("n: ", 1, 10));
    }


    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("11")]
    [InlineData("1.5")]
    public void ReadInt_AsksAgain_OnInvalidOrOutOfRangeInput(string bad)
    {
        var h = ConsoleHarness.WithInput(bad, "5");


        var value = h.Io.ReadInt("n: ", 1, 10);


        Assert.Equal(5, value);
        Assert.Contains("Enter an integer from 1 to 10.", h.Text);
    }


    [Theory]
    [InlineData("1.5", 1.5)]
    [InlineData("1,5", 1.5)]
    [InlineData("0", 0.0)]
    [InlineData(" 2 ", 2.0)]
    public void ReadNonNegativeDouble_AcceptsDotAndComma(string typed, double expected)
    {
        var h = ConsoleHarness.WithInput(typed);


        Assert.Equal(expected, h.Io.ReadNonNegativeDouble("kWh: "));
    }


    [Theory]
    [InlineData("-1")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    [InlineData("abc")]
    [InlineData("")]
    public void ReadNonNegativeDouble_AsksAgain_OnInvalidInput(string bad)
    {
        var h = ConsoleHarness.WithInput(bad, "2");


        var value = h.Io.ReadNonNegativeDouble("kWh: ");


        Assert.Equal(2.0, value);
        Assert.Contains("Enter a non-negative number", h.Text);
    }


    [Fact]
    public void ReadChoice_PrintsNumberedOptions_AndReturnsZeroBasedIndex()
    {
        var h = ConsoleHarness.WithInput("9", "2");


        var index = h.Io.ReadChoice("Pick:", new[] { "Alpha", "Beta", "Gamma" });


        Assert.Equal(1, index);
        Assert.Contains("1. Alpha", h.Text);
        Assert.Contains("3. Gamma", h.Text);
        Assert.Contains("Enter an integer from 1 to 3.", h.Text);
    }
}
