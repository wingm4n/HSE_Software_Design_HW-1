using System.Globalization;

namespace VyshKat.Cli;

/// <summary>
/// Thin wrapper over TextReader/TextWriter: prompts, validation and retry loops.
/// Reading from injected streams (instead of System.Console directly) keeps the UI testable.
/// </summary>
public sealed class ConsoleIO
{
    public const string CancelKeyword = "cancel";

    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleIO(TextReader input, TextWriter output)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void Write(string text) => _output.Write(text);

    public void WriteLine(string text = "") => _output.WriteLine(text);

    public void WriteError(string text) => _output.WriteLine($"  [!] {text}");

    /// <summary>
    /// Reads one trimmed line.
    /// Throws <see cref="EndOfStreamException"/> when the input is closed and
    /// <see cref="OperationCanceledException"/> when the user types "cancel" (if allowed).
    /// </summary>
    public string ReadLine(string prompt, bool allowCancel = true)
    {
        _output.Write(prompt);
        var line = _input.ReadLine();

        if (line is null)
            throw new EndOfStreamException("Input stream closed.");

        line = line.Trim();

        if (allowCancel && line.Equals(CancelKeyword, StringComparison.OrdinalIgnoreCase))
            throw new OperationCanceledException("Cancelled by user.");

        return line;
    }

    public string ReadNonEmpty(string prompt)
    {
        while (true)
        {
            var value = ReadLine(prompt);
            if (value.Length > 0)
                return value;

            WriteError("Value cannot be empty.");
        }
    }

    public int ReadInt(string prompt, int min, int max, bool allowCancel = true)
    {
        while (true)
        {
            var raw = ReadLine(prompt, allowCancel);
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                && value >= min && value <= max)
            {
                return value;
            }

            WriteError($"Enter an integer from {min} to {max}.");
        }
    }

    /// <summary>Reads a finite, non-negative number. Both '.' and ',' are accepted as decimal separator.</summary>
    public double ReadNonNegativeDouble(string prompt)
    {
        while (true)
        {
            var raw = ReadLine(prompt).Replace(',', '.');
            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                && double.IsFinite(value) && value >= 0)
            {
                return value;
            }

            WriteError("Enter a non-negative number (for example 1.5).");
        }
    }

    /// <summary>Prints a numbered list and returns the zero-based index of the chosen option.</summary>
    public int ReadChoice(string title, IReadOnlyList<string> options)
    {
        WriteLine(title);
        for (var i = 0; i < options.Count; i++)
            WriteLine($"  {i + 1}. {options[i]}");

        return ReadInt($"Choose 1-{options.Count}: ", 1, options.Count) - 1;
    }
}
