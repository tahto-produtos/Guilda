/*
  Warnings:

  - Added the required column `GDA_IDGDA_ORDER` to the `GDA_COLLABORATOR_VOUCHER` table without a default value. This is not possible if the table is not empty.

*/
BEGIN TRY

BEGIN TRAN;

-- AlterTable
ALTER TABLE [dbo].[GDA_HISTORY_INDICATOR_GROUP] ADD [ENDED_AT] DATETIME2,
[STARTED_AT] DATETIME2;

COMMIT TRAN;

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0
BEGIN
    ROLLBACK TRAN;
END;
THROW

END CATCH
