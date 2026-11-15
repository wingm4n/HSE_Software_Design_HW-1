using System.Globalization;
using VyshKat.Cli;
using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Entities.Transports;


namespace VyshKat.Tests.Cli;


/// <summary>
/// Test helper: feeds scripted lines to <see cref="ConsoleIO"/> and captures everything it prints.
/// When the script runs out, the reader returns null, which the console treats as "input closed".
/// </summary>
internal sealed class ConsoleHarness
{
    private ConsoleHarness(string[] lines)
    {
        // Reports format numbers with the current culture; pin it so asserts do not depend on the OS locale.
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;


        Input = new StringReader(string.Concat(lines.Select(l => l + "\n")));
        Output = new StringWriter();
        Io = new ConsoleIO(Input, Output);
    }


    public StringReader Input { get; }
    public StringWriter Output { get; }
    public ConsoleIO Io { get; }


    /// <summary>Everything printed so far.</summary>
    public string Text => Output.ToString();


    public static ConsoleHarness WithInput(params string[] lines) => new(lines);
}


/// <summary>Hand-written fake of the inspection: returns a fixed decision and remembers what it was asked about.</summary>
internal sealed class FakeServiceCenter : IServiceCenter
{
    private readonly bool _decision;


    public FakeServiceCenter(bool decision) => _decision = decision;


    public List<Transport> Inspected { get; } = new();


    public bool Inspect(Transport transport)
    {
        Inspected.Add(transport);
        return _decision;
    }
}
