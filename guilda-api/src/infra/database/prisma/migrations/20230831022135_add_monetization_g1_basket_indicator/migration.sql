/*
  Warnings:

  - Added the required column `MONETIZATION_G1` to the `GDA_BASKET_INDICATOR` table without a default value. This is not possible if the table is not empty.

*/
BEGIN TRY

BEGIN TRAN;

-- AlterTable
ALTER TABLE [dbo].[GDA_BASKET_INDICATOR] ADD [MONETIZATION_G1] INT NOT NULL;

COMMIT TRAN;

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0
BEGIN
    ROLLBACK TRAN;
END;
THROW

END CATCH
