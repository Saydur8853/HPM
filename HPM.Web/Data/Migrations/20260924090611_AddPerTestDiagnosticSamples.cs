using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPerTestDiagnosticSamples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lab_Sample_Collection_Memo_ID",
                table: "Lab_Sample_Collection");

            migrationBuilder.AddColumn<int>(
                name: "Service_ID",
                table: "Lab_Sample_Collection",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE sample
                SET [Service_ID] = first_item.[Test_ID]
                FROM [Lab_Sample_Collection] sample
                CROSS APPLY (
                    SELECT TOP (1) item.[Test_ID]
                    FROM [Cash_Memo_Sub_DInvestigation] item
                    WHERE item.[Memo_ID] = sample.[Memo_ID]
                    ORDER BY item.[SLNo], item.[ID]
                ) first_item;

                ;WITH ranked AS (
                    SELECT [ID], ROW_NUMBER() OVER (
                        PARTITION BY [Memo_ID], [Service_ID]
                        ORDER BY CASE [Status] WHEN 'Completed' THEN 0 WHEN 'InProcess' THEN 1 ELSE 2 END, [ID]
                    ) AS rn
                    FROM [Lab_Sample_Collection]
                    WHERE [Service_ID] IS NOT NULL
                )
                DELETE sample
                FROM [Lab_Sample_Collection] sample
                INNER JOIN ranked ON ranked.[ID] = sample.[ID]
                WHERE ranked.rn > 1;

                INSERT INTO [Lab_Sample_Collection]
                    ([Memo_ID], [Service_ID], [Specimen_Type], [Collected_At], [Collected_By], [Status], [Remarks])
                SELECT ordered.[Memo_ID], ordered.[Test_ID],
                    COALESCE(template.[Specimen_Type], 'Blood / Specimen'), template.[Collected_At], template.[Collected_By],
                    COALESCE(template.[Status], 'Pending'), template.[Remarks]
                FROM (
                    SELECT DISTINCT item.[Memo_ID], item.[Test_ID]
                    FROM [Cash_Memo_Sub_DInvestigation] item
                    INNER JOIN [Cash_Memo_DInvestigation] memo ON memo.[ID] = item.[Memo_ID]
                    WHERE memo.[Module_ID] = 2
                ) ordered
                OUTER APPLY (
                    SELECT TOP (1) sample.[Specimen_Type], sample.[Collected_At], sample.[Collected_By], sample.[Status], sample.[Remarks]
                    FROM [Lab_Sample_Collection] sample
                    WHERE sample.[Memo_ID] = ordered.[Memo_ID]
                    ORDER BY CASE sample.[Status] WHEN 'Completed' THEN 0 WHEN 'InProcess' THEN 1 ELSE 2 END, sample.[ID]
                ) template
                WHERE NOT EXISTS (
                    SELECT 1 FROM [Lab_Sample_Collection] existing
                    WHERE existing.[Memo_ID] = ordered.[Memo_ID] AND existing.[Service_ID] = ordered.[Test_ID]
                );

                DELETE FROM [Lab_Sample_Collection] WHERE [Service_ID] IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "Service_ID",
                table: "Lab_Sample_Collection",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lab_Sample_Collection_Memo_ID_Service_ID",
                table: "Lab_Sample_Collection",
                columns: new[] { "Memo_ID", "Service_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lab_Sample_Collection_Service_ID",
                table: "Lab_Sample_Collection",
                column: "Service_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lab_Sample_Collection_Service_Service_ID",
                table: "Lab_Sample_Collection",
                column: "Service_ID",
                principalTable: "Service",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lab_Sample_Collection_Service_Service_ID",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropIndex(
                name: "IX_Lab_Sample_Collection_Memo_ID_Service_ID",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropIndex(
                name: "IX_Lab_Sample_Collection_Service_ID",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropColumn(
                name: "Service_ID",
                table: "Lab_Sample_Collection");

            migrationBuilder.CreateIndex(
                name: "IX_Lab_Sample_Collection_Memo_ID",
                table: "Lab_Sample_Collection",
                column: "Memo_ID");
        }
    }
}
