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
using System.Globalization;
using System.IO;
using System.Linq;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class ShiftIdRepairCsvTests
{
    private const string Header =
        "Id,SdkSitId,Date,ExpectVersion,ExpectStart1Id,ExpectStop1Id,NewStart1Id,NewStop1Id," +
        "NewStart1StartedAt,NewStop1StoppedAt,Reason";

    [Test]
    public void ReadManifest_ParsesEveryColumn_AndSkipsBlankLines()
    {
        var csv = Header + "\n"
                + "10,7,2025-01-06 00:00:00,3,8,10,49,61,,,one-minute-div\n\n"
                + "11,7,2025-01-07 00:00:00,2,40,0,240,1,2025-01-07 22:00:00,2025-01-08 00:05:00,five-minute-cross-midnight\n";

        var lines = ShiftIdRepairCsv.ReadManifest(new StringReader(csv));

        Assert.That(lines, Is.EqualTo(new[]
        {
            new ShiftIdRepairLine(10, 7, new DateTime(2025, 1, 6), 3, 8, 10, 49, 61, null, null, "one-minute-div"),
            new ShiftIdRepairLine(11, 7, new DateTime(2025, 1, 7), 2, 40, 0, 240, 1,
                new DateTime(2025, 1, 7, 22, 0, 0), new DateTime(2025, 1, 8, 0, 5, 0), "five-minute-cross-midnight"),
        }));
    }

    [Test]
    public void ReadManifest_AcceptsADateOnlyDate_AsTheGeneratorWritesIt()
    {
        var csv = Header + "\n"
                + "10,7,2025-05-23,3,8,10,49,61,,,one-minute-div\n"
                + "11,7,2025-05-24,2,40,0,240,1,2025-05-24 22:00:00,2025-05-25 00:05:00,five-minute-cross-midnight\n";

        var lines = ShiftIdRepairCsv.ReadManifest(new StringReader(csv));

        Assert.Multiple(() =>
        {
            Assert.That(lines.Select(x => x.Date),
                Is.EqualTo(new[] { new DateTime(2025, 5, 23), new DateTime(2025, 5, 24) }));
            Assert.That(lines[1].NewStop1StoppedAt, Is.EqualTo(new DateTime(2025, 5, 25, 0, 5, 0)));
        });
    }

    [Test]
    public void DateOnlyDate_RoundTripsThroughBeforeImageAndReversedManifest()
    {
        var line = ShiftIdRepairCsv.ReadManifest(new StringReader(
            Header + "\n10,7,2025-05-23,3,8,10,49,61,,2025-05-24 00:05:00,five-minute-cross-midnight\n")).Single();
        var before = new ShiftIdBeforeImageRow(line.Id, line.SdkSitId, line.Date, line.ExpectVersion,
            line.ExpectVersion + 1, line.ExpectStart1Id, line.ExpectStop1Id, null,
            new DateTime(2025, 5, 23, 23, 55, 0), line.NewStart1Id, line.NewStop1Id);

        var imageCsv = new StringWriter();
        ShiftIdRepairCsv.WriteBeforeImage(imageCsv, new[] { before }, header: true);
        var readBack = ShiftIdRepairCsv.ReadBeforeImage(new StringReader(imageCsv.ToString())).Single();

        var manifestCsv = new StringWriter();
        ShiftIdRepairCsv.WriteManifest(manifestCsv, new[] { ShiftIdRepair.ReverseLine(readBack) });
        var reversed = ShiftIdRepairCsv.ReadManifest(new StringReader(manifestCsv.ToString())).Single();

        Assert.Multiple(() =>
        {
            Assert.That(readBack, Is.EqualTo(before));
            Assert.That(reversed.Date, Is.EqualTo(new DateTime(2025, 5, 23)));
            Assert.That(reversed, Is.EqualTo(ShiftIdRepair.ReverseLine(before)));
        });
    }

    [Test]
    public void ReadBeforeImage_AcceptsADateOnlyDate()
    {
        var csv = ShiftIdRepairCsv.BeforeImageHeader + "\n10,7,2025-05-23,3,4,8,10,,,49,61\n";
        Assert.That(ShiftIdRepairCsv.ReadBeforeImage(new StringReader(csv)).Single().Date,
            Is.EqualTo(new DateTime(2025, 5, 23)));
    }

    [Test]
    public void DateOnlyStamp_IsRejected()
    {
        var ex = Assert.Throws<FormatException>(() => ShiftIdRepairCsv.ReadManifest(new StringReader(
            Header + "\n10,7,2025-05-23,3,8,10,49,61,2025-05-23,,a\n")));
        Assert.That(ex!.Message, Does.Contain("line 2").And.Contain("NewStart1StartedAt"));
    }

    [Test]
    public void WrongHeader_Throws()
    {
        var ex = Assert.Throws<FormatException>(() => ShiftIdRepairCsv.ReadManifest(
            new StringReader("Id,SdkSitId,Date,ExpectVersion,ExpectStart1Id,ExpectStop1Id,NewStart1Id,NewStop1Id\n")));
        Assert.That(ex!.Message, Does.Contain("header"));
    }

    [Test]
    public void WrongColumnCount_ThrowsNamingTheLine()
    {
        var csv = Header + "\n10,7,2025-01-06 00:00:00,3,8,10,49,61,,,ok\n11,7,2025-01-07 00:00:00,3,8,10,49,61,,\n";
        var ex = Assert.Throws<FormatException>(() => ShiftIdRepairCsv.ReadManifest(new StringReader(csv)));
        Assert.That(ex!.Message, Does.Contain("line 3").And.Contain("11 columns"));
    }

    [Test]
    public void DuplicateId_ThrowsNamingTheLine()
    {
        var csv = Header + "\n10,7,2025-01-06 00:00:00,3,8,10,49,61,,,a\n10,7,2025-01-06 00:00:00,3,8,10,49,61,,,b\n";
        var ex = Assert.Throws<FormatException>(() => ShiftIdRepairCsv.ReadManifest(new StringReader(csv)));
        Assert.That(ex!.Message, Does.Contain("line 3").And.Contain("duplicate Id 10"));
    }

    [TestCase("x10,7,2025-01-06 00:00:00,3,8,10,49,61,,,a", "Id")]
    [TestCase("10,7,2025-01-06 00:00:00,3,8.5,10,49,61,,,a", "ExpectStart1Id")]
    [TestCase("10,7,2025-01-06 00:00:00,3,8,10,49,1e3,,,a", "NewStop1Id")]
    [TestCase("10,7,06-01-2025,3,8,10,49,61,,,a", "Date")]
    [TestCase("10,7,,3,8,10,49,61,,,a", "Date")]
    [TestCase("10,7,2025-01-06 00:00:00,3,8,10,49,61,2025-01-06T22:00:00,,a", "NewStart1StartedAt")]
    public void BadValue_ThrowsNamingLineAndColumn(string row, string column)
    {
        var ex = Assert.Throws<FormatException>(() =>
            ShiftIdRepairCsv.ReadManifest(new StringReader(Header + "\n" + row + "\n")));
        Assert.That(ex!.Message, Does.Contain("line 2").And.Contain(column));
    }

    [Test]
    public void BeforeImage_RoundTrips_IncludingFractionalSeconds()
    {
        var rows = new[]
        {
            new ShiftIdBeforeImageRow(10, 7, new DateTime(2025, 1, 6), 3, 4, 8, 10,
                new DateTime(2025, 1, 6, 22, 0, 0), new DateTime(2025, 1, 6, 23, 55, 0).AddTicks(1234560),
                49, 61),
            new ShiftIdBeforeImageRow(11, 7, new DateTime(2025, 1, 7), 1, 2, 0, 0, null, null, 1, 2),
        };
        var sw = new StringWriter();
        ShiftIdRepairCsv.WriteBeforeImage(sw, rows, header: true);

        Assert.That(ShiftIdRepairCsv.ReadBeforeImage(new StringReader(sw.ToString())), Is.EqualTo(rows));
    }

    [Test]
    public void ReversedManifest_RoundTripsThroughTheManifestFormat()
    {
        var before = new ShiftIdBeforeImageRow(10, 7, new DateTime(2025, 1, 6), 3, 4, 8, 10,
            new DateTime(2025, 1, 6, 22, 0, 0), new DateTime(2025, 1, 6, 23, 55, 0), 49, 61);
        var reversed = ShiftIdRepair.ReverseLine(before);
        var sw = new StringWriter();
        ShiftIdRepairCsv.WriteManifest(sw, new[] { reversed });

        Assert.Multiple(() =>
        {
            Assert.That(reversed, Is.EqualTo(new ShiftIdRepairLine(10, 7, new DateTime(2025, 1, 6), 4, 49, 61, 8, 10,
                new DateTime(2025, 1, 6, 22, 0, 0), new DateTime(2025, 1, 6, 23, 55, 0), "revert")));
            Assert.That(ShiftIdRepairCsv.ReadManifest(new StringReader(sw.ToString())).Single(),
                Is.EqualTo(reversed));
        });
    }

    [Test]
    public void ParsingAndWriting_IgnoreTheCurrentCulture()
    {
        var saved = CultureInfo.CurrentCulture;
        var savedUi = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("da-DK");
            CultureInfo.CurrentUICulture = new CultureInfo("da-DK");

            var line = ShiftIdRepairCsv.ReadManifest(new StringReader(
                Header + "\n-10,7,2025-01-06 00:00:00,3,8,10,49,61,,2025-01-07 00:05:00,a\n")).Single();
            var sw = new StringWriter();
            ShiftIdRepairCsv.WriteBeforeImage(sw, new[]
            {
                new ShiftIdBeforeImageRow(1, 7, new DateTime(2025, 1, 6), 1, 2, 8, 10, null,
                    new DateTime(2025, 1, 7, 0, 5, 0).AddTicks(5_000_000), 49, 61)
            }, header: false);

            Assert.Multiple(() =>
            {
                Assert.That(line.Id, Is.EqualTo(-10));
                Assert.That(line.NewStop1StoppedAt, Is.EqualTo(new DateTime(2025, 1, 7, 0, 5, 0)));
                Assert.That(sw.ToString(), Is.EqualTo("1,7,2025-01-06 00:00:00,1,2,8,10,,2025-01-07 00:05:00.5,49,61\n"));
            });
        }
        finally
        {
            CultureInfo.CurrentCulture = saved;
            CultureInfo.CurrentUICulture = savedUi;
        }
    }
}
