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

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// One manifest line: the Start1Id / Stop1Id (and optionally the two shift-1
/// stamps) one <c>PlanRegistration</c> row must get, guarded by the values the
/// manifest was built from. A null <c>New*At</c> leaves that stamp unchanged.
/// </summary>
public sealed record ShiftIdRepairLine(int Id, int SdkSitId, DateTime Date, int ExpectVersion,
    int ExpectStart1Id, int ExpectStop1Id, int NewStart1Id, int NewStop1Id,
    DateTime? NewStart1StartedAt, DateTime? NewStop1StoppedAt, string Reason);

/// <summary>What happened to one manifest line.</summary>
public enum ShiftIdRepairOutcome
{
    /// <summary>The row was (or, in a dry run, would be) given the new values.</summary>
    Repaired,

    /// <summary>The row's Version, Start1Id or Stop1Id no longer match the manifest; not written.</summary>
    Changed,

    /// <summary>The row does not exist, or its SdkSitId / Date differ from the manifest; not written.</summary>
    Missing,

    /// <summary>The row is reconciled, or on/before the worker's reconciled-day lock; not written.</summary>
    Locked
}

/// <summary>The outcome of one manifest line.</summary>
public sealed record ShiftIdRepairLineResult(int Id, int SdkSitId, ShiftIdRepairOutcome Outcome);

/// <summary>
/// A repaired row's shift-1 columns as they were before the repair, plus the
/// Version and ids the repair left it with (<see cref="RepairedVersion"/>,
/// <see cref="RepairedStart1Id"/>, <see cref="RepairedStop1Id"/>) — enough to
/// build the reversed manifest line (see <see cref="ShiftIdRepair.ReverseLine"/>).
/// </summary>
public sealed record ShiftIdBeforeImageRow(int Id, int SdkSitId, DateTime Date, int Version, int RepairedVersion,
    int Start1Id, int Stop1Id, DateTime? Start1StartedAt, DateTime? Stop1StoppedAt,
    int RepairedStart1Id, int RepairedStop1Id);

/// <summary>The outcome of one <see cref="ShiftIdRepair.ApplyAsync"/> call.</summary>
public sealed class ShiftIdRepairResult
{
    /// <summary>True when the transaction committed (apply only).</summary>
    public bool Applied { get; set; }

    /// <summary>One entry per manifest line, in manifest order.</summary>
    public List<ShiftIdRepairLineResult> Lines { get; } = new();

    /// <summary>
    /// One entry per row repaired (or, in a dry run, that would be repaired),
    /// ordered by Id. Captured once per row across execution-strategy retries,
    /// so after a retry it can also hold a row that attempt repaired but the
    /// retry then reported otherwise; reversing such a row is reported Changed.
    /// </summary>
    public List<ShiftIdBeforeImageRow> BeforeImage { get; } = new();
}
