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
public class FlexChainRepairCsvTests
{
    [Test]
    public void ReadRestore_ParsesEveryColumn()
    {
        var csv = "Id,SdkSitId,ExpectVersion,ExpectNettoHours,RestoreNettoHours,RestoreNettoHoursInSeconds,Reason\n"
                + "10,7,3,6.25,7.5,27000,recompute\n\n"
                + "11,7,1,-9.58,0,0,open-shift\n";
        var lines = FlexChainRepairCsv.ReadRestore(new StringReader(csv));
        Assert.That(lines, Is.EqualTo(new[]
        {
            new RestoreLine(10, 7, 3, 6.25, 7.5, 27000, "recompute"),
            new RestoreLine(11, 7, 1, -9.58, 0, 0, "open-shift"),
        }));
    }

    [Test]
    public void ReadExpected_ParsesEveryColumn()
    {
        var csv = "SdkSitId,Id,SumFlexStart,SumFlexEnd\n7,10,1.5,2.25\n";
        Assert.That(FlexChainRepairCsv.ReadExpected(new StringReader(csv)),
            Is.EqualTo(new[] { new ExpectedBalance(7, 10, 1.5, 2.25) }));
    }

    [Test]
    public void BeforeImage_RoundTrips()
    {
        var row = new BeforeImageRow(10, 7, 3, 5, 7.123456789, 25644, -0.376543211, -1356,
            12.5, 45000, 12.123456789, 43644);
        var sw = new StringWriter();
        FlexChainRepairCsv.WriteBeforeImage(sw, new[] { row }, header: true);
        Assert.That(FlexChainRepairCsv.ReadBeforeImage(new StringReader(sw.ToString())),
            Is.EqualTo(new[] { row }));
    }

    [Test]
    public void WrongHeader_Throws()
    {
        Assert.Throws<FormatException>(() =>
            FlexChainRepairCsv.ReadExpected(new StringReader("Id,SdkSitId,SumFlexStart,SumFlexEnd\n")));
    }

    [Test]
    public void WrongColumnCount_ThrowsWithLineNumber()
    {
        var ex = Assert.Throws<FormatException>(() =>
            FlexChainRepairCsv.ReadExpected(new StringReader("SdkSitId,Id,SumFlexStart,SumFlexEnd\n7,10,1.5\n")));
        Assert.That(ex!.Message, Does.Contain("2"));
    }

    [Test]
    public void Parsing_IgnoresCurrentCulture()
    {
        var saved = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("da-DK");
            var csv = "SdkSitId,Id,SumFlexStart,SumFlexEnd\n7,10,1.5,-2.25\n";
            var e = FlexChainRepairCsv.ReadExpected(new StringReader(csv)).Single();
            Assert.That(e.SumFlexEnd, Is.EqualTo(-2.25));
            var sw = new StringWriter();
            FlexChainRepairCsv.WriteBeforeImage(sw,
                new[] { new BeforeImageRow(1, 1, 1, 1, 0.5, 1800, 0.5, 1800, 0, 0, 0.5, 1800) }, header: false);
            Assert.That(sw.ToString(), Does.Contain("0.5"));
            Assert.That(sw.ToString(), Does.Not.Contain("0,5"));
        }
        finally { CultureInfo.CurrentCulture = saved; }
    }
}
