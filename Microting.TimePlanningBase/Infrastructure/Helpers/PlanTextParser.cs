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
using System.Text.RegularExpressions;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// Parses and generates the free-text shift strings stored in
/// <c>PlanRegistration.PlanText</c>.
///
/// Format: up to <see cref="MaxShifts"/> segments separated by ';', each
/// "start-end" or "start-end/break".
///
/// Times accept ':' , '.' or ',' before the minutes. A two-digit fraction is
/// clock minutes ("7.30" is 07:30). A single-digit fraction is shorthand: '0'
/// is :00, '3' and '5' are both :30 — two conventions planners use for the
/// same half hour. Any other single digit is ambiguous and rejects the
/// segment rather than guessing.
///
/// Breaks are decimal hours ("0.5" is 30 minutes, "1.5" is 90), plus the '½'
/// and '¾' glyphs and an "H:MM" form.
///
/// No parsing method throws. Text that does not conform is stripped, so a
/// stray note in a spreadsheet cell costs its own segment at most, never the
/// import.
/// </summary>
public static class PlanTextParser
{
    public const int MaxShifts = 5;

    private const int MinutesPerDay = 24 * 60;
    private const int HalfHourMinutes = 30;
    private const int ThreeQuarterHourMinutes = 45;

    /// <summary>
    /// Upper bound for a break. There is deliberately no one-hour ceiling —
    /// that ceiling was the defect this parser replaces — but a break still
    /// cannot exceed a day, and without a bound a huge decimal saturates the
    /// cast to int and lands 2,147,483,647 minutes in the database.
    /// </summary>
    private const int MaxBreakMinutes = MinutesPerDay;

    /// <summary>
    /// A conforming segment: "start-end" with an optional "/break". Anchored
    /// on the time tokens rather than the whole cell, so surrounding prose is
    /// discarded instead of derailing the parse. The lookarounds keep the
    /// match off the inside of a longer number, so "100-200" is not read as
    /// "00-20".
    /// </summary>
    private static readonly Regex SegmentRegex = new(
        @"(?<!\d)(?<start>\d{1,2}(?:[:.,]\d{1,2}|½)?)(?!\d)"
        + @"\s*-\s*"
        + @"(?<end>\d{1,2}(?:[:.,]\d{1,2}|½)?)(?!\d)"
        + @"(?:\s*/\s*(?<break>[^;]*))?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// One time token: hours, then either a separator and a fraction, or '½'.
    /// Anchored, because it re-reads a token <see cref="SegmentRegex"/> has
    /// already delimited. Stating the token grammar once keeps the two halves
    /// of a segment from drifting apart.
    /// </summary>
    private static readonly Regex TimeRegex = new(
        @"^(?<hour>\d{1,2})(?:(?<sep>[:.,])(?<fraction>\d{1,2})|(?<half>½))?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>A break written as "H:MM" rather than decimal hours.</summary>
    private static readonly Regex BreakColonRegex = new(
        @"^(?<h>\d{1,2}):(?<m>\d{1,2})",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Decimal hours with an optional trailing glyph, ignoring any prose that
    /// follows. The digits are optional so a leading-decimal break like ".5"
    /// and a bare glyph like "½ + AT" both resolve.
    /// </summary>
    private static readonly Regex BreakValueRegex = new(
        @"^(?<num>\d*(?:\.\d+)?)(?<glyph>[½¾])?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>One shift parsed out of a segment. All values are minutes.</summary>
    public readonly record struct Shift(int StartMinutes, int EndMinutes, int BreakMinutes);

    /// <summary>
    /// Parses a PlanText string into shift slots. The result always has
    /// <see cref="MaxShifts"/> entries and is positional: segment <c>n</c>
    /// occupies slot <c>n</c>, so a non-conforming segment leaves its slot
    /// null rather than shifting later segments forward. Callers write every
    /// slot, which clears shifts the text no longer mentions.
    ///
    /// Never throws.
    /// </summary>
    public static Shift?[] Parse(string planText)
    {
        var slots = new Shift?[MaxShifts];

        if (string.IsNullOrWhiteSpace(planText))
        {
            return slots;
        }

        var segments = planText.Split(';');

        for (var i = 0; i < segments.Length && i < MaxShifts; i++)
        {
            if (TryParseSegment(segments[i], out var shift))
            {
                slots[i] = shift;
            }
        }

        return slots;
    }

    /// <summary>
    /// Parses a single segment such as "8:00-16:00/0.5" or "6,3-16/1". Returns
    /// false when the segment carries no conforming shift — including a value
    /// with no '-', which is plan hours rather than a shift.
    ///
    /// Never throws.
    /// </summary>
    public static bool TryParseSegment(string segment, out Shift shift)
    {
        shift = default;

        if (string.IsNullOrWhiteSpace(segment))
        {
            return false;
        }

        // Shape and value are separate gates: a token can look like a shift and
        // still carry an ambiguous fraction or an impossible hour. Keep scanning
        // so a real shift later in the cell is not lost to earlier noise.
        for (var match = SegmentRegex.Match(segment); match.Success; match = match.NextMatch())
        {
            if (!TryResolveTime(match.Groups["start"].Value, out var start)
                || !TryResolveTime(match.Groups["end"].Value, out var end))
            {
                continue;
            }

            var breakGroup = match.Groups["break"];
            shift = new Shift(start, end, breakGroup.Success ? ParseBreakMinutes(breakGroup.Value) : 0);
            return true;
        }

        return false;
    }

    private static bool TryResolveTime(string token, out int minutes)
    {
        minutes = 0;

        // SegmentRegex only hands over tokens this pattern accepts, so the
        // guard cannot fire today. It is what keeps the never-throw promise if
        // the two patterns ever drift apart.
        var match = TimeRegex.Match(token);
        if (!match.Success)
        {
            return false;
        }

        var hours = int.Parse(match.Groups["hour"].Value, CultureInfo.InvariantCulture);
        var fractionMinutes = 0;

        if (match.Groups["half"].Success)
        {
            fractionMinutes = HalfHourMinutes;
        }
        else
        {
            var fraction = match.Groups["fraction"];
            if (fraction.Success
                && !TryResolveFraction(fraction.Value, match.Groups["sep"].Value, out fractionMinutes))
            {
                return false;
            }
        }

        var total = hours * 60 + fractionMinutes;

        // Validate the resolved time, not the hour digits alone, so the cutoff
        // does not land mid-hour. 24:00 is a legal end of day; nothing past it is.
        if (total > MinutesPerDay)
        {
            return false;
        }

        minutes = total;
        return true;
    }

    /// <summary>
    /// Resolves the part after the separator into minutes past the hour.
    ///
    /// Two digits are clock minutes and must be 00-59. A single digit is
    /// shorthand and only '0', '3' and '5' occur in practice: '3' is a dropped
    /// trailing zero ("6.3" is 06:30) and '5' a decimal half hour ("7,5" is
    /// 07:30). Other single digits have no established meaning, so they are
    /// rejected rather than resolved to a plausible-looking wrong time.
    /// </summary>
    private static bool TryResolveFraction(string fraction, string separator, out int minutes)
    {
        minutes = 0;

        // A colon always introduces clock minutes, so "7:3" is 07:03 and the
        // shorthand below does not apply to it.
        if (separator == ":")
        {
            var colonValue = int.Parse(fraction, CultureInfo.InvariantCulture);
            if (colonValue > 59)
            {
                return false;
            }

            minutes = colonValue;
            return true;
        }

        if (fraction.Length == 2)
        {
            var value = int.Parse(fraction, CultureInfo.InvariantCulture);
            if (value > 59)
            {
                return false;
            }

            minutes = value;
            return true;
        }

        switch (fraction)
        {
            case "0":
                minutes = 0;
                return true;
            case "3":
            case "5":
                minutes = HalfHourMinutes;
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Converts a break token to minutes. Breaks are decimal hours; "1.5" is
    /// 90 minutes. Trailing prose is ignored, so "2 helligdag" is 120 minutes
    /// and "½ + AT" is 30. Unrecognised input yields no break. Never throws.
    /// </summary>
    public static int ParseBreakMinutes(string breakPart)
    {
        if (string.IsNullOrWhiteSpace(breakPart))
        {
            return 0;
        }

        var normalized = breakPart.Replace(',', '.').Trim();

        var colonMatch = BreakColonRegex.Match(normalized);
        if (colonMatch.Success)
        {
            var minutes = int.Parse(colonMatch.Groups["h"].Value, CultureInfo.InvariantCulture) * 60
                          + int.Parse(colonMatch.Groups["m"].Value, CultureInfo.InvariantCulture);
            return Math.Clamp(minutes, 0, MaxBreakMinutes);
        }

        var valueMatch = BreakValueRegex.Match(normalized);
        var number = valueMatch.Groups["num"].Value;
        var glyph = valueMatch.Groups["glyph"];

        var glyphMinutes = glyph.Success
            ? glyph.Value == "½" ? HalfHourMinutes : ThreeQuarterHourMinutes
            : 0;

        if (number.Length == 0)
        {
            return glyphMinutes;
        }

        if (!double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var hours)
            || !double.IsFinite(hours))
        {
            return glyphMinutes;
        }

        // Clamp in double space: a large value saturates the cast to int
        // silently, which would put a nonsense break in the database.
        var totalMinutes = Math.Round(hours * 60, MidpointRounding.AwayFromZero) + glyphMinutes;

        return totalMinutes >= MaxBreakMinutes ? MaxBreakMinutes : (int)totalMinutes;
    }

    /// <summary>
    /// Renders shifts back to PlanText, skipping empty slots. Times use
    /// "H:MM", which this grammar reads unambiguously, and breaks use
    /// invariant decimal hours, so the output parses back to the same shifts
    /// for every representable value. A shift that both starts and ends at
    /// midnight is an empty row rather than a shift, and is omitted.
    /// </summary>
    public static string Generate(IEnumerable<Shift?> shifts)
    {
        if (shifts is null)
        {
            return string.Empty;
        }

        var segments = new List<string>();

        foreach (var slot in shifts)
        {
            if (slot is not { } shift || (shift.StartMinutes == 0 && shift.EndMinutes == 0))
            {
                continue;
            }

            var start = FormatTime(shift.StartMinutes);
            var end = FormatTime(shift.EndMinutes);

            segments.Add(shift.BreakMinutes > 0
                ? $"{start}-{end}/{FormatBreak(shift.BreakMinutes)}"
                : $"{start}-{end}");
        }

        return string.Join(";", segments);
    }

    /// <summary>Converts minutes since midnight to "H:MM".</summary>
    public static string FormatTime(int totalMinutes) => $"{totalMinutes / 60}:{totalMinutes % 60:D2}";

    /// <summary>Converts break minutes to the decimal-hours form used in PlanText.</summary>
    public static string FormatBreak(int breakMinutes) =>
        (breakMinutes / 60.0).ToString("0.####", CultureInfo.InvariantCulture);
}
