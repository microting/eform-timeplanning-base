/*
The MIT License (MIT)

Copyright (c) 2007 - 2026 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// Reads the shift-id repair manifest and reads/writes the before-image the
/// repair produces. Plain comma-separated values without quoting (so Reason
/// must not contain a comma); every number and timestamp is parsed and written
/// with <see cref="CultureInfo.InvariantCulture"/>, whatever the machine's locale.
/// </summary>
public static class ShiftIdRepairCsv
{
    public const string ManifestHeader =
        "Id,SdkSitId,Date,ExpectVersion,ExpectStart1Id,ExpectStop1Id,NewStart1Id,NewStop1Id," +
        "NewStart1StartedAt,NewStop1StoppedAt,Reason";

    public const string BeforeImageHeader =
        "Id,SdkSitId,Date,Version,RepairedVersion,Start1Id,Stop1Id,Start1StartedAt,Stop1StoppedAt," +
        "RepairedStart1Id,RepairedStop1Id";

    private const string WholeSeconds = "yyyy-MM-dd HH:mm:ss";

    // The stamp columns are datetime(6): the before-image keeps any fraction so a
    // reversed manifest restores the exact value. ".FFFFFFF" prints nothing (not
    // even the dot) for a whole second, so whole-second values look the same in
    // both formats.
    private const string WithFraction = "yyyy-MM-dd HH:mm:ss.FFFFFFF";
    private static readonly string[] StampFormats = { WholeSeconds, WithFraction };

    // The Date column is a day; the generator writes it bare ("2025-05-23"),
    // the before-image writes it as a stamp. Both are accepted.
    private const string DateOnly = "yyyy-MM-dd";
    private static readonly string[] DateFormats = { DateOnly, WholeSeconds, WithFraction };

    /// <summary>
    /// Reads a manifest. Throws <see cref="FormatException"/> on a wrong header,
    /// a wrong column count, an unparseable value or a duplicate Id — the
    /// message names the 1-based line. Blank lines are skipped.
    /// </summary>
    public static IReadOnlyList<ShiftIdRepairLine> ReadManifest(TextReader reader)
    {
        var seen = new HashSet<int>();
        return ReadRows(reader, ManifestHeader, f =>
        {
            var line = new ShiftIdRepairLine(
                f.Int(0), f.Int(1), f.Date(2), f.Int(3), f.Int(4), f.Int(5), f.Int(6), f.Int(7),
                f.OptionalStamp(8), f.OptionalStamp(9), f.Values[10]);
            if (!seen.Add(line.Id))
            {
                throw new FormatException($"line {f.LineNumber}: duplicate Id {line.Id}");
            }
            return line;
        });
    }

    /// <summary>Writes a manifest (e.g. a reversed one built from a before-image).</summary>
    public static void WriteManifest(TextWriter writer, IEnumerable<ShiftIdRepairLine> lines)
    {
        WriteCsvLine(writer, ManifestHeader);
        foreach (var l in lines)
        {
            WriteCsvLine(writer, string.Join(",",
                Int(l.Id), Int(l.SdkSitId), Stamp(l.Date), Int(l.ExpectVersion),
                Int(l.ExpectStart1Id), Int(l.ExpectStop1Id), Int(l.NewStart1Id), Int(l.NewStop1Id),
                Stamp(l.NewStart1StartedAt), Stamp(l.NewStop1StoppedAt), l.Reason));
        }
    }

    /// <summary>Reads a before-image written by <see cref="WriteBeforeImage"/>.</summary>
    public static IReadOnlyList<ShiftIdBeforeImageRow> ReadBeforeImage(TextReader reader) =>
        ReadRows(reader, BeforeImageHeader, f => new ShiftIdBeforeImageRow(
            f.Int(0), f.Int(1), f.Date(2), f.Int(3), f.Int(4), f.Int(5), f.Int(6),
            f.OptionalStamp(7), f.OptionalStamp(8), f.Int(9), f.Int(10)));

    /// <summary>
    /// Writes before-image rows. Pass <paramref name="header"/> false to append
    /// to a file that already has the header line.
    /// </summary>
    public static void WriteBeforeImage(TextWriter writer, IEnumerable<ShiftIdBeforeImageRow> rows, bool header)
    {
        if (header)
        {
            WriteCsvLine(writer, BeforeImageHeader);
        }

        foreach (var r in rows)
        {
            WriteCsvLine(writer, string.Join(",",
                Int(r.Id), Int(r.SdkSitId), Stamp(r.Date), Int(r.Version), Int(r.RepairedVersion),
                Int(r.Start1Id), Int(r.Stop1Id), Stamp(r.Start1StartedAt), Stamp(r.Stop1StoppedAt),
                Int(r.RepairedStart1Id), Int(r.RepairedStop1Id)));
        }
    }

    private static List<T> ReadRows<T>(TextReader reader, string header, Func<Fields, T> parse)
    {
        var columns = header.Split(',');
        var firstLine = reader.ReadLine();
        if (firstLine != header)
        {
            throw new FormatException($"line 1: expected header '{header}'");
        }

        var results = new List<T>();
        var lineNumber = 1;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            if (line.Length == 0)
            {
                continue;
            }

            var values = line.Split(',');
            if (values.Length != columns.Length)
            {
                throw new FormatException(
                    $"line {lineNumber}: expected {columns.Length} columns, got {values.Length}");
            }

            results.Add(parse(new Fields(values, columns, lineNumber)));
        }

        return results;
    }

    /// <summary>One data line's fields; every parse error names the line and column.</summary>
    private readonly record struct Fields(string[] Values, string[] Columns, int LineNumber)
    {
        public int Int(int i)
        {
            if (!int.TryParse(Values[i], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var v))
            {
                throw Error(i, "is not an integer");
            }
            return v;
        }

        /// <summary>A required day: <c>yyyy-MM-dd</c>, or a full timestamp.</summary>
        public DateTime Date(int i)
        {
            if (Values[i].Length == 0)
            {
                throw Error(i, "is required");
            }
            return Parse(i, DateFormats, $"'{DateOnly}' date");
        }

        /// <summary>An optional timestamp; empty means null.</summary>
        public DateTime? OptionalStamp(int i) =>
            Values[i].Length == 0 ? null : Parse(i, StampFormats, $"'{WholeSeconds}' timestamp");

        private DateTime Parse(int i, string[] formats, string expected)
        {
            if (!DateTime.TryParseExact(Values[i], formats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var d))
            {
                throw Error(i, $"is not a {expected}");
            }
            return d;
        }

        private FormatException Error(int i, string what) =>
            new($"line {LineNumber}: {Columns[i]} {what} ('{Values[i]}')");
    }

    private static void WriteCsvLine(TextWriter writer, string line)
    {
        writer.Write(line);
        writer.Write('\n');
    }

    private static string Int(int i) => i.ToString(CultureInfo.InvariantCulture);

    private static string Stamp(DateTime? d) => d?.ToString(WithFraction, CultureInfo.InvariantCulture) ?? "";
}
