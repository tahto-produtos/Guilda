BEGIN TRY

BEGIN TRAN;

-- CreateTable
CREATE TABLE [dbo].[GDA_LOG_INDICATOR_IMPORT] (
    [INDICATOR_ID] INT NOT NULL IDENTITY(1,1),
    [COLABORATOR_ID] INT,
    [CODE_IMPORT] INT NOT NULL,
    [CODE] INT,
    [NAME] NVARCHAR(1000),
    [DESCRIPTION] NVARCHAR(1000),
    [TYPE] NVARCHAR(1000),
    [STATUS] BIT,
    [WEIGHT] INT,
    [CREATED_AT] DATETIME2 NOT NULL CONSTRAINT [GDA_LOG_INDICATOR_IMPORT_CREATED_AT_df] DEFAULT CURRENT_TIMESTAMP,
    [DELETED] DATETIME2,
    CONSTRAINT [GDA_LOG_INDICATOR_IMPORT_pkey] PRIMARY KEY CLUSTERED ([INDICATOR_ID])
);

-- AddForeignKey
ALTER TABLE [dbo].[GDA_LOG_INDICATOR_IMPORT] ADD CONSTRAINT [GDA_LOG_INDICATOR_IMPORT_COLABORATOR_ID_fkey] FOREIGN KEY ([COLABORATOR_ID]) REFERENCES [dbo].[GDA_COLLABORATORS]([IDGDA_COLLABORATORS]) ON DELETE NO ACTION ON UPDATE NO ACTION;

COMMIT TRAN;

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0
BEGIN
    ROLLBACK TRAN;
END;
THROW

END CATCH
