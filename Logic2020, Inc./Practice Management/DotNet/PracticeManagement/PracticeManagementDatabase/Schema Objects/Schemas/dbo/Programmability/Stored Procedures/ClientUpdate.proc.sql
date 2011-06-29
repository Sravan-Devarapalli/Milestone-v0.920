CREATE PROCEDURE dbo.ClientUpdate
(
	@ClientId             INT,
	@Name                 NVARCHAR(50),
	@DefaultDiscount      DECIMAL(18,2),
	@DefaultSalespersonId INT,
	@DefaultDirectorId	  INT,
	@IsChargeable         BIT,
	@Inactive             BIT,
	@DefaultTerms         INT,
	@IsMarginColorInfoEnabled BIT
)
AS
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM dbo.Client AS c WHERE c.Name = @Name AND c.ClientId <> @ClientId)
	BEGIN
		DECLARE @ErrorMessage NVARCHAR(2048)
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70004)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE
	BEGIN
		UPDATE dbo.Client
		   SET DefaultDiscount = @DefaultDiscount,
			   DefaultTerms = @DefaultTerms,
			   DefaultSalespersonID = @DefaultSalespersonID,
			   DefaultDirectorID	= @DefaultDirectorId,
			   Name = @Name,
			   Inactive = @Inactive,
			   IsChargeable = @IsChargeable,
			   IsMarginColorInfoEnabled = @IsMarginColorInfoEnabled
		 WHERE ClientId = @ClientId
	END

GO

