CREATE TABLE [dbo].[VendorAttachment]
(
	Id							INT IDENTITY(1,1) NOT NULL,
	VendorId					INT				  NOT NULL,
	[FileName]					[nvarchar](256)       NULL,	
	[AttachmentData]			[varbinary](max)      NULL,
	UploadedDate				DATETIME			  NULL,
	ModifiedBy					INT					  NULL,
   CONSTRAINT PK_VendorAttachment_Id PRIMARY KEY CLUSTERED(Id),
);
