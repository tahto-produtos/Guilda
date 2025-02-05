/*
  Warnings:

  - You are about to drop the column `metric_max` on the `GDA_HISTORY_INDICATOR_GROUP` table. All the data in the column will be lost.

*/
BEGIN TRY

BEGIN TRAN;

-- AlterTable
ALTER TABLE [dbo].[GDA_GROUPS] ADD [ORDER] INT;

-- AlterTable
ALTER TABLE [dbo].[GDA_HISTORY_INDICATOR_GROUP] DROP COLUMN [metric_max];

COMMIT TRAN;

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0
BEGIN
    ROLLBACK TRAN;
END;
THROW

END CATCH
