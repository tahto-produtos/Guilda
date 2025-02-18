BEGIN TRY

BEGIN TRAN;

-- AlterTable
ALTER TABLE [dbo].[GDA_ORDER_PRODUCT] ADD [GDA_STOCK_IDGDA_STOCK] INT;

-- AddForeignKey
ALTER TABLE [dbo].[GDA_ORDER_PRODUCT] ADD CONSTRAINT [GDA_ORDER_PRODUCT_GDA_STOCK_IDGDA_STOCK_fkey] FOREIGN KEY ([GDA_STOCK_IDGDA_STOCK]) REFERENCES [dbo].[GDA_STOCK]([IDGDA_STOCK]) ON DELETE SET NULL ON UPDATE NO ACTION;

COMMIT TRAN;

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0
BEGIN
    ROLLBACK TRAN;
END;
THROW

END CATCH
