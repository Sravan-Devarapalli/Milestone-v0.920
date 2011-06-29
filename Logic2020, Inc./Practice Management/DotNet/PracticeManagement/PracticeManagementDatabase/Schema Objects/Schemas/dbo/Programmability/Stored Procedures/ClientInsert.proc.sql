-- =============================================
-- Author:		Skip Sailors
-- Create date: 4-22-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 8-22-2008
-- Description:	Insert a new Client, output and select ID of new client record.
-- =============================================
CREATE PROCEDURE dbo.ClientInsert
(
	@Name                 NVARCHAR(50),
	@DefaultDiscount      DECIMAL(18,2),
	@DefaultSalespersonId INT, 
	@DefaultDirectorId	  INT,
	@Inactive             BIT,
	@IsChargeable         BIT,
	@DefaultTerms         INT,
	@ClientId             INT OUTPUT,
	@IsMarginColorInfoEnabled BIT
)
AS
	SET NOCOUNT ON

	IF EXISTS (SELECT 1 FROM dbo.Client AS c WHERE c.Name = @Name)
	BEGIN
		DECLARE @ErrorMessage NVARCHAR(2048)
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70004)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE
	BEGIN
		INSERT INTO Client
					(DefaultDiscount, DefaultTerms, DefaultSalespersonId, DefaultDirectorID, Name, Inactive, IsChargeable,IsMarginColorInfoEnabled)
			 VALUES (@DefaultDiscount, @DefaultTerms, @DefaultSalespersonId, @DefaultDirectorId, @Name, @Inactive, @IsChargeable,@IsMarginColorInfoEnabled)

		SELECT @clientId = SCOPE_IDENTITY()

		SELECT @clientId
	END
GO

