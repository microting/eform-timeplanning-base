using Microsoft.EntityFrameworkCore.Migrations;
using Microting.TimePlanningBase.Infrastructure.Data.DataMigrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <summary>
    /// Data-only migration: rewrites existing "GLS-A / 3F - Udenlandske
    /// praktikanter Landbrug" (Staldarbejde / Andet arbejde) pay-rule-set rows to
    /// the corrected § 50 tiers of 2026-08-07. Presets are copy-at-create-time
    /// snapshots, so the catalogue correction never reached rule sets customers
    /// had already created. See <see cref="PraktikantSection50TierCorrection"/>
    /// for scope, idempotency and versioning details.
    /// </summary>
    public partial class CorrectPraktikantSection50Tiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var statement in PraktikantSection50TierCorrection.SqlStatements)
            {
                migrationBuilder.Sql(statement);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Deliberately irreversible: rolling back would reinstate defective
            // tiers under which long Saturdays, Sundays and holidays earn zero
            // overtime minutes, and the pre-correction shapes are not uniform
            // across customers, so there is no single prior state to restore.
            // The PayTierRuleVersions rows written by Up() preserve the full
            // history should forensics ever be needed.
        }
    }
}
