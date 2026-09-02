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
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// The shared running-flex-balance chain: pure functions over
/// <see cref="PlanRegistration"/> fields that compute netto hours, per-shift
/// pause seconds and the running SumFlexStart/SumFlexEnd/Flex chain in both
/// the legacy five-minute decimal precision and the second precision used
/// under one-minute intervals. No dependency beyond the base entities and the
/// BCL, so both the plugin and the background service consume this same
/// implementation instead of holding independently-drifting copies.
/// </summary>
public static class FlexChain
{
    /// <summary>
    /// Aggregates total pause minutes for a PlanRegistration by summing the
    /// canonical per-shift pause (<see cref="ComputeShiftPauseSeconds"/>) across
    /// shifts 1-5 and rounding the total down to whole minutes.
    ///
    /// ComputeShiftPauseSeconds is the single source of truth: per shift it walks
    /// every populated pause slot (primary Pause{N} plus the multi-pause sub-slots)
    /// and applies the exact stamp delta when useOneMinuteIntervals is true, or the
    /// floor-to-5-minute clock-tick delta when it is false, falling back per shift to
    /// the legacy Pause{N}Id tick value only when that shift has no timestamped slots.
    /// </summary>
    public static int AggregatePauseMinutes(PlanRegistration pr, bool useOneMinuteIntervals)
    {
        // Sum the canonical per-shift pause across all 5 shifts. The canonical
        // method walks EVERY populated slot of each shift (primary + sub-slots),
        // applies the exact delta (flag on) or the floor-to-5-minute clock-tick
        // delta (flag off), and falls back per-shift to the legacy Pause{N}Id
        // tick value only when that shift has no timestamped slots.
        long totalSeconds = 0;
        for (var shift = 1; shift <= 5; shift++)
        {
            totalSeconds += ComputeShiftPauseSeconds(pr, shift, useOneMinuteIntervals);
        }

        return (int)(totalSeconds / 60); // round down to whole minutes
    }

    /// <summary>
    /// Reads an <c>*InSeconds</c> column, falling back to its legacy
    /// <c>double</c> hour sibling when the column is still 0.
    ///
    /// Every <c>*InSeconds</c> column was added by a migration with
    /// <c>defaultValue: 0</c> and NO backfill (SumFlexEndInSeconds by
    /// 20260108054344), so on the overwhelming majority of historical rows the
    /// column reads 0 while the real value lives in the decimal. Taking the
    /// column at face value silently substitutes zero for a real balance.
    ///
    /// A genuine zero and an unbackfilled zero are indistinguishable, which is
    /// harmless: both fall back to the decimal, and a genuinely-zero row has a
    /// zero decimal too.
    /// </summary>
    public static int SecondsOrDecimalFallback(int seconds, double hours)
        => seconds != 0 ? seconds : (int)Math.Round(hours * 3600);

    /// <summary>
    /// Seeds the running flex chain from the preceding day's closing balance,
    /// in seconds, via <see cref="SecondsOrDecimalFallback"/>; 0 when there is
    /// no preceding row.
    ///
    /// The fallback is load-bearing at a one-minute mode transition: the first
    /// post-switch row seeds from the last PRE-switch row, which by definition
    /// only ever had the decimal columns written.
    /// </summary>
    /// <param name="preIsOneMinute">
    /// The mode the PRECEDING row resolves to (write-time marker, else the
    /// site's effective date / audit timeline). Pass <c>false</c> and the
    /// row's <c>SumFlexEndInSeconds</c> column is IGNORED — a five-minute row
    /// carries its balance in the decimal only, so a non-zero seconds column
    /// there is stale residue from an earlier one-minute write, never a
    /// balance. Pass <c>null</c> (the default) when the mode is not known and
    /// the column is taken at face value, as before.
    ///
    /// Defence in depth. <see cref="ApplyNettoFlexChainDecimal"/> now clears
    /// both seconds columns on every five-minute write, so a row written by
    /// THIS version of the code cannot carry stale seconds; rows written by an
    /// older version, by the background service (which never touches the
    /// seconds columns) or by a direct DB edit still can, and seeding the chain
    /// from such a value is what restated whole balances at a mode boundary.
    /// </param>
    public static int SumFlexEndSecondsWithFallback(
        PlanRegistration? preTimePlanning, bool? preIsOneMinute = null)
    {
        if (preTimePlanning == null)
        {
            return 0;
        }

        if (preIsOneMinute == false)
        {
            return (int)Math.Round(preTimePlanning.SumFlexEnd * 3600);
        }

        return SecondsOrDecimalFallback(
            preTimePlanning.SumFlexEndInSeconds, preTimePlanning.SumFlexEnd);
    }

    /// <summary>
    /// Clears the second-precision SumFlex columns.
    ///
    /// The invariant: a row whose balance was last written in FIVE-MINUTE
    /// (decimal) mode carries NO seconds — <c>SumFlexStartInSeconds</c> and
    /// <c>SumFlexEndInSeconds</c> read 0, and every reader therefore falls back
    /// to the decimal via <see cref="SecondsOrDecimalFallback"/>. Leaving a
    /// previous one-minute write's value behind makes the row claim a balance
    /// it no longer has, and the next row seeds the whole chain from it.
    /// </summary>
    public static void ClearSumFlexSeconds(PlanRegistration pr)
    {
        pr.SumFlexStartInSeconds = 0;
        pr.SumFlexEndInSeconds = 0;
    }

    /// <summary>
    /// The FIVE-MINUTE counterpart of
    /// <see cref="ApplyNettoFlexChainSecondPrecision(PlanRegistration, PlanRegistration?, bool?)"/>:
    /// writes the legacy decimal Flex / SumFlexStart / SumFlexEnd chain AND
    /// clears the <c>*InSeconds</c> siblings, so no call site can write one
    /// without the other.
    ///
    ///   Flex       = (override ? NettoHoursOverride : NettoHours) - PlanHours
    ///   SumFlexStart = preTimePlanning?.SumFlexEnd ?? 0
    ///   SumFlexEnd   = SumFlexStart + effectiveNetto - PlanHours - PaiedOutFlex
    ///
    /// <c>NettoHours</c> is used AS-IS (callers that recompute it from the
    /// five-minute tick math assign it immediately before calling).
    /// </summary>
    public static void ApplyNettoFlexChainDecimal(
        PlanRegistration pr, PlanRegistration? preTimePlanning)
    {
        var effectiveNettoHours = pr.NettoHoursOverrideActive
            ? pr.NettoHoursOverride
            : pr.NettoHours;

        pr.Flex = effectiveNettoHours - pr.PlanHours;
        pr.SumFlexStart = preTimePlanning?.SumFlexEnd ?? 0;
        pr.SumFlexEnd = pr.SumFlexStart + effectiveNettoHours - pr.PlanHours - pr.PaiedOutFlex;

        ClearSumFlexSeconds(pr);
    }

    /// <summary>
    /// Preferred overload: seeds the chain from <paramref name="preTimePlanning"/>
    /// (null when this is the first row) through
    /// <see cref="SumFlexEndSecondsWithFallback"/>, so no call site can
    /// accidentally seed from the raw, usually-zero <c>SumFlexEndInSeconds</c>
    /// column and silently discard the carried-forward balance.
    /// </summary>
    /// <param name="preIsOneMinute">
    /// The preceding row's resolved mode, forwarded to
    /// <see cref="SumFlexEndSecondsWithFallback"/> so a five-minute
    /// predecessor seeds from its decimal balance instead of a stale seconds
    /// column. Pass <c>null</c> when the mode is not cheaply resolvable.
    /// </param>
    public static void ApplyNettoFlexChainSecondPrecision(
        PlanRegistration pr, PlanRegistration? preTimePlanning, bool? preIsOneMinute = null)
        => ApplyNettoFlexChainSecondPrecision(
            pr,
            SumFlexEndSecondsWithFallback(preTimePlanning, preIsOneMinute),
            preTimePlanning != null);

    /// <summary>
    /// Phase 2 — write the second-precision NettoHours / Flex / SumFlex chain.
    ///
    /// Computes <c>NettoHoursInSeconds</c> from DateTime deltas (or legacy
    /// fallback) via <see cref="ComputeNettoSecondsFromDateTimeShifts"/>,
    /// derives <c>FlexInSeconds</c> from <c>PlanHoursInSeconds</c>, then
    /// derives <c>SumFlexEndInSeconds</c> from the running balance plus the
    /// computed flex minus paid-out flex. Back-derives the legacy
    /// <c>double</c> hour fields (<c>x = xInSeconds / 3600.0</c>) so existing
    /// read paths stay compatible.
    ///
    /// Mirrors the existing flag-off formula sign-for-sign:
    ///   Flex            = NettoHours - PlanHours          (or override)
    ///   SumFlexEnd      = SumFlexStart + NettoHours - PlanHours - PaiedOutFlex
    ///                       (when preTimePlanning exists)
    ///   SumFlexEnd      = NettoHours - PlanHours - PaiedOutFlex
    ///                       (when no preTimePlanning, SumFlexStart = 0)
    /// — but every operand is in seconds, so no precision is lost on the
    /// way through the int columns.
    ///
    /// Caller passes <paramref name="sumFlexStartInSeconds"/> from the previous
    /// day's <c>SumFlexEndInSeconds</c> (or 0 when there is no preceding row).
    /// When the override is active, the override (in hours) is converted to
    /// seconds via <c>* 3600</c> for the chain.
    /// </summary>
    /// <param name="pr">The plan registration to update in place.</param>
    /// <param name="sumFlexStartInSeconds">
    /// Running flex balance carried in from the previous day's
    /// <c>SumFlexEndInSeconds</c>; pass 0 when there is no preceding row.
    /// </param>
    /// <param name="hasPreTimePlanning">
    /// True when there is a preceding planning row (use the running balance);
    /// false when this is the first row (reset SumFlexStart to 0).
    /// </param>
    public static void ApplyNettoFlexChainSecondPrecision(PlanRegistration pr,
        int sumFlexStartInSeconds, bool hasPreTimePlanning)
    {
        var nettoSeconds = ComputeNettoSecondsFromDateTimeShifts(pr);
        pr.NettoHoursInSeconds = (int)nettoSeconds;
        pr.NettoHours = nettoSeconds / 3600.0;

        // Punch-clock / scheduled days and production writers populate only the
        // doubles; the *InSeconds siblings stay 0. See SecondsOrDecimalFallback.
        var planHoursSeconds = SecondsOrDecimalFallback(pr.PlanHoursInSeconds, pr.PlanHours);
        var paiedOutFlexSeconds =
            SecondsOrDecimalFallback(pr.PaiedOutFlexInSeconds, pr.PaiedOutFlex);

        // Mirror the flag-off override semantics:
        //   Flex      = (override ? NettoHoursOverride : NettoHours) - PlanHours
        //   SumFlexEnd uses the same numerator.
        var effectiveNettoSecondsForFlex = pr.NettoHoursOverrideActive
            ? (long)(pr.NettoHoursOverride * 3600)
            : nettoSeconds;

        var flexSeconds = effectiveNettoSecondsForFlex - planHoursSeconds;
        pr.FlexInSeconds = (int)flexSeconds;
        pr.Flex = flexSeconds / 3600.0;

        if (hasPreTimePlanning)
        {
            pr.SumFlexStartInSeconds = sumFlexStartInSeconds;
            pr.SumFlexStart = sumFlexStartInSeconds / 3600.0;
            var sumFlexEndSeconds = (long)sumFlexStartInSeconds
                                    + effectiveNettoSecondsForFlex - planHoursSeconds
                                    - paiedOutFlexSeconds;
            pr.SumFlexEndInSeconds = (int)sumFlexEndSeconds;
            pr.SumFlexEnd = sumFlexEndSeconds / 3600.0;
        }
        else
        {
            pr.SumFlexStartInSeconds = 0;
            pr.SumFlexStart = 0;
            var sumFlexEndSeconds = effectiveNettoSecondsForFlex - planHoursSeconds - paiedOutFlexSeconds;
            pr.SumFlexEndInSeconds = (int)sumFlexEndSeconds;
            pr.SumFlexEnd = sumFlexEndSeconds / 3600.0;
        }
    }

    /// <summary>
    /// Phase 2 — second-precision NettoHours computation.
    ///
    /// When <see cref="AssignedSite.UseOneMinuteIntervals"/> is on, this helper
    /// computes NettoHours from DateTime deltas (precise to the second) instead
    /// of the legacy <c>(StopId - StartId - (PauseId-1)) * 5</c> minute-tick math
    /// in the per-call sites. Mirrors the flag-off formula in seconds:
    ///
    /// <code>
    /// nettoSeconds = 0
    /// for each shift n in 1..5:
    ///     if (Start_n_StartedAt and Stop_n_StoppedAt are populated):
    ///         nettoSeconds += (Stop_n_StoppedAt - Start_n_StartedAt).TotalSeconds
    ///         if (Pause_n_StartedAt and Pause_n_StoppedAt are populated):
    ///             nettoSeconds -= (Pause_n_StoppedAt - Pause_n_StartedAt).TotalSeconds
    ///         else if (Pause_n_Id > 0):
    ///             nettoSeconds -= (Pause_n_Id - 1) * 5 * 60
    ///     else if (Stop_n_Id &gt;= Start_n_Id and Stop_n_Id != 0):
    ///         // legacy fallback for shifts that don't have DateTime stamps
    ///         nettoSeconds += (Stop_n_Id - Start_n_Id) * 5 * 60
    ///         nettoSeconds -= (Pause_n_Id &gt; 0 ? Pause_n_Id - 1 : 0) * 5 * 60
    /// </code>
    ///
    /// Returns the computed netto seconds. The caller writes both the
    /// <c>*InSeconds</c> primary and back-derives the legacy <c>double</c>
    /// hour field (<c>x = xInSeconds / 3600.0</c>) for read compatibility.
    /// </summary>
    public static long ComputeNettoSecondsFromDateTimeShifts(PlanRegistration pr)
    {
        long nettoSeconds = 0;

        // Helper: compute one shift's contribution. Prefer DateTime delta when
        // both stamps are populated; otherwise fall back to the legacy 5-min
        // tick math so mixed-precision rows (some shifts precise, some not)
        // still get a complete total. Pause is the canonical per-shift total
        // (ALL slots, not just the primary) — second-precision because this
        // method only runs on UseOneMinuteIntervals sites.
        long ShiftSeconds(int shift, DateTime? startAt, DateTime? stopAt, int startId, int stopId)
        {
            long workSeconds;
            if (startAt.HasValue && stopAt.HasValue && stopAt.Value > startAt.Value)
            {
                workSeconds = (long)(stopAt.Value - startAt.Value).TotalSeconds;
            }
            else if (stopId >= startId && stopId != 0)
            {
                workSeconds = (long)(stopId - startId) * 5 * 60;
            }
            else
            {
                return 0;
            }

            long pauseSeconds = ComputeShiftPauseSeconds(pr, shift, useOneMinuteIntervals: true);

            return workSeconds - pauseSeconds;
        }

        nettoSeconds += ShiftSeconds(1, pr.Start1StartedAt, pr.Stop1StoppedAt, pr.Start1Id, pr.Stop1Id);
        nettoSeconds += ShiftSeconds(2, pr.Start2StartedAt, pr.Stop2StoppedAt, pr.Start2Id, pr.Stop2Id);
        nettoSeconds += ShiftSeconds(3, pr.Start3StartedAt, pr.Stop3StoppedAt, pr.Start3Id, pr.Stop3Id);
        nettoSeconds += ShiftSeconds(4, pr.Start4StartedAt, pr.Stop4StoppedAt, pr.Start4Id, pr.Stop4Id);
        nettoSeconds += ShiftSeconds(5, pr.Start5StartedAt, pr.Stop5StoppedAt, pr.Start5Id, pr.Stop5Id);

        return Math.Max(0, nettoSeconds);
    }

    /// <summary>
    /// Canonical per-shift pause total in SECONDS — the single source of truth
    /// for every netto and display pause computation.
    ///
    /// Sums the contribution of EVERY populated pause slot that belongs to the
    /// shift (primary Pause{N} plus its sub-slots, see
    /// <see cref="EnumerateShiftPauseStampPairs"/>), where each slot contributes:
    ///   • <paramref name="useOneMinuteIntervals"/> == true  → the exact
    ///     (StoppedAt - StartedAt) delta in seconds (full precision).
    ///   • <paramref name="useOneMinuteIntervals"/> == false → the clock-tick
    ///     delta: floor BOTH endpoints to the absolute 5-minute grid and
    ///     difference them — floor(stop) - floor(start), a whole number of
    ///     5-minute units. A pause that stays inside one 5-min cell contributes
    ///     0; it adds 5 min for each 5-minute boundary it crosses.
    ///
    /// Fallback: when the shift has NO slot with both timestamps present (e.g.
    /// legacy admin-entered rows that only carry the integer field), falls back
    /// to the legacy 5-minute-tick value of the shift's primary slot only:
    /// (Pause{N}Id > 0 ? Pause{N}Id - 1 : 0) * 5 * 60 seconds.
    /// </summary>
    public static int ComputeShiftPauseSeconds(PlanRegistration r, int shift, bool useOneMinuteIntervals)
    {
        // Admin/manual pause override takes precedence: when set, it is the
        // authoritative total pause MINUTES for the shift. The recorded
        // Pause{N}StartedAt/StoppedAt sub-slots are preserved untouched in the DB
        // (documentation of what the worker actually did) but are not summed here.
        var overrideMinutes = GetShiftPauseOverrideMinutes(r, shift);
        if (overrideMinutes.HasValue)
        {
            return overrideMinutes.Value * 60;
        }

        long totalSeconds = 0;
        var hasTimestampedSlot = false;

        foreach (var (startedAt, stoppedAt) in EnumerateShiftPauseStampPairs(r, shift))
        {
            // A slot only counts as "measured" — and thus suppresses the
            // legacy-tick fallback — when BOTH endpoints are present, i.e. it is
            // a complete, measurable interval. A deliberately zero-duration
            // (start == stop) or invalid (stop < start) but COMPLETE pause still
            // counts: the worker stamped a real (if zero) pause, so the intended
            // contribution is 0 and the legacy field must not resurface.
            // An orphaned slot (only one endpoint — e.g. kiosk crash or partial
            // edit) is NOT a complete slot, so it does not suppress the fallback;
            // the row correctly falls back to the legacy Pause{N}Id tick value.
            if (startedAt.HasValue && stoppedAt.HasValue)
            {
                hasTimestampedSlot = true;
            }

            if (!startedAt.HasValue || !stoppedAt.HasValue || stoppedAt.Value <= startedAt.Value)
            {
                continue;
            }

            if (useOneMinuteIntervals)
            {
                totalSeconds += (long)(stoppedAt.Value - startedAt.Value).TotalSeconds;
            }
            else
            {
                var tickDelta = FloorTo5Min(stoppedAt.Value) - FloorTo5Min(startedAt.Value);
                totalSeconds += (long)tickDelta.TotalSeconds;
            }
        }

        if (!hasTimestampedSlot)
        {
            var pauseId = PrimaryPauseId(r, shift);
            return pauseId > 0 ? (pauseId - 1) * 5 * 60 : 0;
        }

        return (int)totalSeconds;
    }

    /// <summary>
    /// Read the per-shift admin/manual pause override (in minutes) from the
    /// registration. null = no override (compute pause from recorded slots);
    /// non-null = authoritative total pause minutes for that shift.
    /// </summary>
    public static int? GetShiftPauseOverrideMinutes(PlanRegistration r, int shift)
    {
        return shift switch
        {
            1 => r.Pause1OverrideMinutes,
            2 => r.Pause2OverrideMinutes,
            3 => r.Pause3OverrideMinutes,
            4 => r.Pause4OverrideMinutes,
            5 => r.Pause5OverrideMinutes,
            _ => null
        };
    }

    /// <summary>
    /// Enumerates the pause stamp pairs that belong to ONE shift.
    /// A shift can carry pauses in several slot columns:
    ///   shift 1 → Pause1 (primary), Pause10..Pause19, Pause100..Pause102
    ///   shift 2 → Pause2 (primary), Pause20..Pause29, Pause200..Pause202
    ///   shift 3 → Pause3 (single slot)
    ///   shift 4 → Pause4 (single slot)
    ///   shift 5 → Pause5 (single slot)
    /// The primary slot is always yielded first so callers that need the
    /// "primary only" semantics (e.g. legacy-fallback) can take the first pair.
    /// </summary>
    private static IEnumerable<(DateTime? StartedAt, DateTime? StoppedAt)> EnumerateShiftPauseStampPairs(PlanRegistration pr, int shift)
    {
        switch (shift)
        {
            case 1:
                yield return (pr.Pause1StartedAt, pr.Pause1StoppedAt);
                yield return (pr.Pause10StartedAt, pr.Pause10StoppedAt);
                yield return (pr.Pause11StartedAt, pr.Pause11StoppedAt);
                yield return (pr.Pause12StartedAt, pr.Pause12StoppedAt);
                yield return (pr.Pause13StartedAt, pr.Pause13StoppedAt);
                yield return (pr.Pause14StartedAt, pr.Pause14StoppedAt);
                yield return (pr.Pause15StartedAt, pr.Pause15StoppedAt);
                yield return (pr.Pause16StartedAt, pr.Pause16StoppedAt);
                yield return (pr.Pause17StartedAt, pr.Pause17StoppedAt);
                yield return (pr.Pause18StartedAt, pr.Pause18StoppedAt);
                yield return (pr.Pause19StartedAt, pr.Pause19StoppedAt);
                yield return (pr.Pause100StartedAt, pr.Pause100StoppedAt);
                yield return (pr.Pause101StartedAt, pr.Pause101StoppedAt);
                yield return (pr.Pause102StartedAt, pr.Pause102StoppedAt);
                break;
            case 2:
                yield return (pr.Pause2StartedAt, pr.Pause2StoppedAt);
                yield return (pr.Pause20StartedAt, pr.Pause20StoppedAt);
                yield return (pr.Pause21StartedAt, pr.Pause21StoppedAt);
                yield return (pr.Pause22StartedAt, pr.Pause22StoppedAt);
                yield return (pr.Pause23StartedAt, pr.Pause23StoppedAt);
                yield return (pr.Pause24StartedAt, pr.Pause24StoppedAt);
                yield return (pr.Pause25StartedAt, pr.Pause25StoppedAt);
                yield return (pr.Pause26StartedAt, pr.Pause26StoppedAt);
                yield return (pr.Pause27StartedAt, pr.Pause27StoppedAt);
                yield return (pr.Pause28StartedAt, pr.Pause28StoppedAt);
                yield return (pr.Pause29StartedAt, pr.Pause29StoppedAt);
                yield return (pr.Pause200StartedAt, pr.Pause200StoppedAt);
                yield return (pr.Pause201StartedAt, pr.Pause201StoppedAt);
                yield return (pr.Pause202StartedAt, pr.Pause202StoppedAt);
                break;
            case 3:
                yield return (pr.Pause3StartedAt, pr.Pause3StoppedAt);
                break;
            case 4:
                yield return (pr.Pause4StartedAt, pr.Pause4StoppedAt);
                break;
            case 5:
                yield return (pr.Pause5StartedAt, pr.Pause5StoppedAt);
                break;
        }
    }

    /// <summary>
    /// The legacy 5-minute-tick integer pause field for a shift's primary slot.
    /// Pause{N}Id stores break in 5-minute ticks plus a +1 sentinel
    /// (Pause1Id = 1 means 0 min, Pause1Id = 4 means 15 min, etc.).
    /// </summary>
    private static int PrimaryPauseId(PlanRegistration pr, int shift) => shift switch
    {
        1 => pr.Pause1Id,
        2 => pr.Pause2Id,
        3 => pr.Pause3Id,
        4 => pr.Pause4Id,
        5 => pr.Pause5Id,
        _ => 0
    };

    private static readonly long FiveMinuteTicks = TimeSpan.FromMinutes(5).Ticks;

    /// <summary>
    /// Floors a DateTime down to its absolute 5-minute grid boundary on the
    /// timeline (NOT relative to the day) so the result is over-midnight safe.
    /// </summary>
    private static DateTime FloorTo5Min(DateTime dt)
        => new DateTime(dt.Ticks - (dt.Ticks % FiveMinuteTicks), dt.Kind);
}
