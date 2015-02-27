CREATE PROCEDURE [dbo].[SaveBadgeDetailsByPersonId]
(
	@PersonId			INT,
	@IsBlocked			BIT,
	@BlockStartDate		DATETIME,
	@BlockEndDate		DATETIME,
	@IsPreviousBadge	BIT,
	@PreviousBadgeAlias	NVARCHAR(20),
	@LastBadgeStartDate	DATETIME,
	@LastBadgeEndDate	DATETIME,
	@IsException		BIT,
	@ExceptionStartDate	DATETIME,
	@ExceptionEndDate	DATETIME,
	@UpdatedBy			INT
)
AS
BEGIN

  IF EXISTS(SELECT 1 FROM dbo.MSBadge WHERE PersonId = @PersonId)
  BEGIN
		UPDATE dbo.MSBadge 
		SET IsBlocked = @IsBlocked,
			BlockStartDate = @BlockStartDate,
			BlockEndDate = @BlockEndDate,
			IsPreviousBadge = @IsPreviousBadge,
			PreviousBadgeAlias = @PreviousBadgeAlias,
			LastBadgeStartDate = @LastBadgeStartDate,
			LastBadgeEndDate = @LastBadgeEndDate,
			IsException = @IsException,
			ExceptionStartDate = @ExceptionStartDate,
			ExceptionEndDate = @ExceptionEndDate
		WHERE PersonId = @PersonId
  END
  ELSE
  BEGIN
	    INSERT INTO dbo.MSBadge(PersonId,IsBlocked,BlockStartDate,BlockEndDate,IsPreviousBadge,PreviousBadgeAlias,LastBadgeStartDate,LastBadgeEndDate,IsException,ExceptionStartDate,ExceptionEndDate)
		SELECT	@PersonId,@IsBlocked,@BlockStartDate,@BlockEndDate,@IsPreviousBadge,@PreviousBadgeAlias,@LastBadgeStartDate,@LastBadgeEndDate,@IsException,@ExceptionStartDate,@ExceptionEndDate
  END 
  
  EXEC [dbo].[UpdateMSBadgeDetailsByPersonId] @PersonId = @PersonId,@UpdatedBy = @UpdatedBy

END
