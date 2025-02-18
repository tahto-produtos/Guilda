/*
  Warnings:

  - Added the required column `GDA_IDGDA_ORDER` to the `GDA_COLLABORATOR_VOUCHER` table without a default value. This is not possible if the table is not empty.

*/
BEGIN TRY

BEGIN TRAN;

-- AlterTable
ALTER TABLE [dbo].[GDA_COLLABORATOR_VOUCHER] ADD [GDA_IDGDA_ORDER] INT NOT NULL;

-- AlterTable
ALTER TABLE [dbo].[GDA_PRODUCT] ADD [GDA_DETAIL_IDGDA_DETAIL] INT,
[GDA_SIZE_IDGDA_SIZE] INT,
[VALIDITY_DATE] DATETIME2;


-- CreateTable
CREATE TABLE [dbo].[GDA_SIZES] (
    [IDGDA_SIZE] INT NOT NULL IDENTITY(1,1),
    [SIZE_NAME] NVARCHAR(1000) NOT NULL,
    [CREATED_AT] DATETIME2 NOT NULL CONSTRAINT [GDA_SIZES_CREATED_AT_df] DEFAULT CURRENT_TIMESTAMP,
    [DELETED_AT] DATETIME2,
    [createdByCollaboratorId] INT NOT NULL,
    CONSTRAINT [GDA_SIZES_pkey] PRIMARY KEY CLUSTERED ([IDGDA_SIZE])
);

-- CreateTable
CREATE TABLE [dbo].[GDA_PRODUCT_DETAIL] (
    [IDGDA_PRODUCT_DETAIL] INT NOT NULL IDENTITY(1,1),
    [PRODUCT_DETAIL_NAME] NVARCHAR(1000) NOT NULL,
    [CREATED_AT] DATETIME2 NOT NULL CONSTRAINT [GDA_PRODUCT_DETAIL_CREATED_AT_df] DEFAULT CURRENT_TIMESTAMP,
    [DELETED_AT] DATETIME2,
    [createdByCollaboratorId] INT NOT NULL,
    CONSTRAINT [GDA_PRODUCT_DETAIL_pkey] PRIMARY KEY CLUSTERED ([IDGDA_PRODUCT_DETAIL])
);

-- AddForeignKey
ALTER TABLE [dbo].[GDA_PRODUCT] ADD CONSTRAINT [GDA_PRODUCT_GDA_SIZE_IDGDA_SIZE_fkey] FOREIGN KEY ([GDA_SIZE_IDGDA_SIZE]) REFERENCES [dbo].[GDA_SIZES]([IDGDA_SIZE]) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE [dbo].[GDA_PRODUCT] ADD CONSTRAINT [GDA_PRODUCT_GDA_DETAIL_IDGDA_DETAIL_fkey] FOREIGN KEY ([GDA_DETAIL_IDGDA_DETAIL]) REFERENCES [dbo].[GDA_PRODUCT_DETAIL]([IDGDA_PRODUCT_DETAIL]) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE [dbo].[GDA_SIZES] ADD CONSTRAINT [GDA_SIZES_createdByCollaboratorId_fkey] FOREIGN KEY ([createdByCollaboratorId]) REFERENCES [dbo].[GDA_COLLABORATORS]([IDGDA_COLLABORATORS]) ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE [dbo].[GDA_PRODUCT_DETAIL] ADD CONSTRAINT [GDA_PRODUCT_DETAIL_createdByCollaboratorId_fkey] FOREIGN KEY ([createdByCollaboratorId]) REFERENCES [dbo].[GDA_COLLABORATORS]([IDGDA_COLLABORATORS]) ON DELETE NO ACTION ON UPDATE NO ACTION;

COMMIT TRAN;

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0
BEGIN
    ROLLBACK TRAN;
END;
THROW

END CATCH
