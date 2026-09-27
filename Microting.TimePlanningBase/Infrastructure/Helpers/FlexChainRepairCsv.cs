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
/// Reads and writes the plain-CSV shapes used by the flex-chain repair tool:
/// restore plans, expected end balances, and before-images. Every number is
/// parsed and written with <see cref="CultureInfo.InvariantCulture"/> so the
/// files are stable regardless of the machine's locale.
/// </summary>
public static class FlexChainRepairCsv
{
    private const string RestoreHeader =
        "Id,SdkSitId,ExpectVersion,ExpectNettoHours,RestoreNettoHours,RestoreNettoHoursInSeconds,Reason";

    private const string ExpectedHeader = "SdkSitId,Id,SumFlexStart,SumFlexEnd";

    private const string BeforeImageHeader =
        "Id,SdkSitId,Version,RepairedVersion,NettoHours,NettoHoursInSeconds,Flex,FlexInSeconds," +
        "SumFlexStart,SumFlexStartInSeconds,SumFlexEnd,SumFlexEndInSeconds";

    /// <summary>Reads a restore-plan CSV (see <see cref="RestoreLine"/>).</summary>
    public static IReadOnlyList<RestoreLine> ReadRestore(TextReader reader) =>
        ReadRows(reader, RestoreHeader, f => new RestoreLine(
            f.Int(0), f.Int(1), f.Int(2), f.Double(3), f.Double(4), f.Int(5), f.Text(6)));

    /// <summary>Reads an expected-balance CSV (see <see cref="ExpectedBalance"/>).</summary>
    public static IReadOnlyList<ExpectedBalance> ReadExpected(TextReader reader) =>
        ReadRows(reader, ExpectedHeader, f => new ExpectedBalance(
            f.Int(0), f.Int(1), f.Double(2), f.Double(3)));

    /// <summary>Reads a before-image CSV (see <see cref="BeforeImageRow"/>).</summary>
    public static IReadOnlyList<BeforeImageRow> ReadBeforeImage(TextReader reader) =>
        ReadRows(reader, BeforeImageHeader, f => new BeforeImageRow(
            f.Int(0), f.Int(1), f.Int(2), f.Int(3), f.Double(4), f.Int(5), f.Double(6), f.Int(7),
            f.Double(8), f.Int(9), f.Double(10), f.Int(11)));

    /// <summary>
    /// Writes a before-image CSV. Pass <paramref name="header"/> false to append
    /// rows to a file that already has the header line.
    /// </summary>
    public static void WriteBeforeImage(TextWriter writer, IEnumerable<BeforeImageRow> rows, bool header)
    {
        if (header)
        {
            writer.Write(BeforeImageHeader);
            writer.Write('\n');
        }

        foreach (var row in rows)
        {
            writer.Write(string.Join(",",
                WriteInt(row.Id),
                WriteInt(row.SdkSitId),
                WriteInt(row.Version),
                WriteInt(row.RepairedVersion),
                WriteDouble(row.NettoHours),
                WriteInt(row.NettoHoursInSeconds),
                WriteDouble(row.Flex),
                WriteInt(row.FlexInSeconds),
                WriteDouble(row.SumFlexStart),
                WriteInt(row.SumFlexStartInSeconds),
                WriteDouble(row.SumFlexEnd),
                WriteInt(row.SumFlexEndInSeconds)));
            writer.Write('\n');
        }
    }

    private static List<T> ReadRows<T>(TextReader reader, string header, Func<Fields, T> parse)
    {
        var results = new List<T>();
        var columns = header.Split(',');

        var firstLine = reader.ReadLine();
        if (firstLine != header)
        {
            throw new FormatException($"expected header '{header}'");
        }

        var lineNumber = 1;
        string? line;
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

    /// <summary>One data line's fields, with the context a parse error needs.</summary>
    private readonly record struct Fields(string[] Values, string[] Columns, int LineNumber)
    {
        public string Text(int i) => Values[i];

        public int Int(int i) => int.Parse(Values[i], NumberStyles.Integer, CultureInfo.InvariantCulture);

        /// <summary>
        /// NaN and ±Infinity parse under <see cref="NumberStyles.Float"/> but would
        /// poison every tolerance comparison downstream, so they are rejected here.
        /// </summary>
        public double Double(int i)
        {
            var d = double.Parse(Values[i], NumberStyles.Float, CultureInfo.InvariantCulture);
            if (double.IsNaN(d) || double.IsInfinity(d))
            {
                throw new FormatException($"line {LineNumber}: {Columns[i]} is not a finite number ('{Values[i]}')");
            }
            return d;
        }
    }

    private static string WriteDouble(double d) => d.ToString("R", CultureInfo.InvariantCulture);

    private static string WriteInt(int i) => i.ToString(CultureInfo.InvariantCulture);
}
