using System.Globalization;

namespace VyshKat.Cli;

/// <summary>
/// Thin wrapper over TextReader/TextWriter: prompts, validation and retry loops.
/// Reading from injected streams (instead of System.Console directly) keeps the UI testable.
/// </summary>
public sealed class ConsoleIO
{
    /// <summary>Word that the user can type at a prompt to abort the current operation.</summary>
    public const string CancelKeyword = "cancel";

    // Injected streams (not System.Console), so the UI can be tested with in-memory readers and writers.
    private readonly TextReader _input;
    private readonly TextWriter _output;

    /// <summary>
    /// Creates the wrapper over the given streams.
    /// </summary>
    /// <param name="input">Source of the user's lines.</param>
    /// <param name="output">Destination of prompts and messages.</param>
    /// <exception cref="ArgumentNullException">Any argument is <c>null</c>.</exception>
    public ConsoleIO(TextReader input, TextWriter output)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    /// <summary>Writes text without a line break (used for prompts).</summary>
    public void Write(string text) => _output.Write(text);

    /// <summary>Writes text followed by a line break.</summary>
    public void WriteLine(string text = "") => _output.WriteLine(text);

    /// <summary>Writes an error message with the <c>[!]</c> prefix.</summary>
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

    /// <summary>Reads a line, asking again until it is not empty.</summary>
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

    /// <summary>
    /// Reads an integer from <paramref name="min"/> to <paramref name="max"/> (inclusive), asking again on invalid input.
    /// </summary>
    /// <param name="prompt">Prompt text.</param>
    /// <param name="min">Lowest accepted value.</param>
    /// <param name="max">Highest accepted value.</param>
    /// <param name="allowCancel">Whether typing the cancel keyword aborts the operation.</param>
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
