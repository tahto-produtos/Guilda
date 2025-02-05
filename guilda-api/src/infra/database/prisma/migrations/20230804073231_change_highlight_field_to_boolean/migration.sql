/*
  Warnings:

  - You are about to alter the column `HIGHLIGHT` on the `GDA_PRODUCT` table. The data in that column could be lost. The data in that column will be cast from `Int` to `Bit`.

*/
BEGIN TRY

BEGIN TRAN;

-- AlterTable
ALTER TABLE [dbo].[GDA_PRODUCT] DROP CONSTRAINT [GDA_PRODUCT_HIGHLIGHT_df];
ALTER TABLE [dbo].[GDA_PRODUCT] ALTER COLUMN [HIGHLIGHT] BIT NOT NULL;
ALTER TABLE [dbo].[GDA_PRODUCT] ADD CONSTRAINT [GDA_PRODUCT_HIGHLIGHT_df] DEFAULT 0 FOR [HIGHLIGHT];

COMMIT TRAN;

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0
BEGIN
    ROLLBACK TRAN;
END;
THROW

END CATCH
