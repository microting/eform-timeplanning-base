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
/// One row of a restore plan: what a single <c>PlanRegistration</c> row's
/// NettoHours must become, and why. Read from / written to the "restore" CSV.
/// </summary>
public sealed record RestoreLine(int Id, int SdkSitId, int ExpectVersion, double ExpectNettoHours,
    double RestoreNettoHours, int RestoreNettoHoursInSeconds, string Reason);

/// <summary>
/// The expected end-of-chain flex balance for one worker, used to verify a
/// repair walk landed where it should. Read from / written to the "expected" CSV.
/// </summary>
public sealed record ExpectedBalance(int SdkSitId, int Id, double SumFlexStart, double SumFlexEnd);

/// <summary>
/// A snapshot of one row's chain-relevant columns, taken before a repair
/// touches it, so the repair can be reverted. Read from / written to the
/// "before-image" CSV.
/// </summary>
public sealed record BeforeImageRow(int Id, int SdkSitId, int Version, int RepairedVersion,
    double NettoHours, int NettoHoursInSeconds, double Flex, int FlexInSeconds,
    double SumFlexStart, int SumFlexStartInSeconds, double SumFlexEnd, int SumFlexEndInSeconds);

/// <summary>
/// How a single worker's repair attempt concluded.
/// </summary>
public enum RepairOutcome
{
    /// <summary>Dry run only: nothing was written, the plan checks out.</summary>
    DryRunOk,

    /// <summary>The repair was applied and the resulting chain matches expectations.</summary>
    Applied,

    /// <summary>A row's stored version didn't match the plan's expected version; nothing was applied.</summary>
    GuardFailed,

    /// <summary>The repair was applied but the resulting chain does not match the expected balance.</summary>
    Mismatch,

    /// <summary>A row that needed repair falls on or before the worker's reconciled-day lock.</summary>
    Locked
}

/// <summary>
/// One column that disagreed with its expected value after a repair walk.
/// </summary>
public sealed record RowMismatch(int Id, DateTime Date, string Field, double Expected, double Actual);

/// <summary>
/// The outcome of repairing (or dry-running a repair for) one worker's flex chain.
/// </summary>
public sealed class WorkerRepairResult
{
    public int SdkSitId { get; init; }
    public RepairOutcome Outcome { get; set; }
    public int RowsRestored { get; set; }
    public int RowsWalked { get; set; }
    public double? EndBalanceBefore { get; set; }
    public double? EndBalanceAfter { get; set; }
    public List<BeforeImageRow> BeforeImage { get; } = new();
    public List<int> GuardFailedIds { get; } = new();
    public List<RowMismatch> Mismatches { get; } = new();
}

/// <summary>
/// The outcome of reverting a previously applied repair from its before-image.
/// </summary>
public sealed class RevertResult
{
    public bool Applied { get; set; }
    public int Reverted { get; set; }
    public List<int> ChangedIds { get; } = new();
    public List<int> MissingIds { get; } = new();
}
