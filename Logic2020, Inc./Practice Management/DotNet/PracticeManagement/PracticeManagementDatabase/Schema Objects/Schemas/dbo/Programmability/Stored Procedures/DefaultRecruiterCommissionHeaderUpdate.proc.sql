-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-02-2008
-- Update by:	
-- Update date:	
-- Description:	Updates a person's default actual recruiting commisions
-- =============================================
CREATE PROCEDURE [dbo].[DefaultRecruiterCommissionHeaderUpdate]
(
	@PersonId                           INT,
	@StartDate                          DATETIME,
	@EndDate                            DATETIME,
	@DefaultRecruiterCommissionHeaderId INT
)
AS
	SET NOCOUNT ON

	SET @EndDate = ISNULL(@EndDate, dbo.GetFutureDate())

	DECLARE @OLD_StartDate DATETIME
	DECLARE @OLD_EndDate DATETIME
	DECLARE @ErrorMessage NVARCHAR(2048)

	IF NOT EXISTS (SELECT 1 FROM dbo.DefaultRecruiterCommissionHeader
					WHERE DefaultRecruiterCommissionHeaderId  = @DefaultRecruiterCommissionHeaderId)
	BEGIN
		SET IDENTITY_INSERT dbo.DefaultRecruiterCommissionHeader ON
		
		INSERT INTO [dbo].[DefaultRecruiterCommissionHeader]
           ([DefaultRecruiterCommissionHeaderId]
           ,[PersonId]
           ,[StartDate]
           ,[EndDate])
     VALUES
           (
           @DefaultRecruiterCommissionHeaderId
           ,@PersonId
           ,@StartDate
           ,@EndDate)
		
		SET IDENTITY_INSERT dbo.DefaultRecruiterCommissionHeader OFF
	END
	ELSE					
	BEGIN
		SELECT @OLD_StartDate = h.StartDate, @OLD_EndDate = h.EndDate
		  FROM dbo.DefaultRecruiterCommissionHeader AS h
		 WHERE h.DefaultRecruiterCommissionHeaderId = @DefaultRecruiterCommissionHeaderId

		IF EXISTS(SELECT 1
					FROM dbo.DefaultRecruiterCommissionHeader AS h
				   WHERE h.PersonId = @PersonId 
					 AND h.StartDate >= @StartDate
					 AND h.StartDate <> @OLD_StartDate
					 AND h.EndDate <= @OLD_StartDate)
		BEGIN
			-- The record overlaps from begining
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70013)
			RAISERROR (@ErrorMessage, 16, 1)
			RETURN
		END
		ELSE IF EXISTS(SELECT 1
						 FROM dbo.DefaultRecruiterCommissionHeader AS h
						WHERE h.PersonId = @PersonId 
						  AND h.EndDate <= @EndDate
						  AND h.EndDate <> @OLD_EndDate
						  AND h.StartDate >= @OLD_EndDate)
		BEGIN
			-- The records overlaps from ending
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70014)
			RAISERROR (@ErrorMessage, 16, 1)
			RETURN
		END
		ELSE IF EXISTS(SELECT 1
						 FROM dbo.DefaultRecruiterCommissionHeader AS h
						WHERE h.PersonId = @PersonId
						  AND h.StartDate <= @StartDate AND h.EndDate >= @EndDate
						  AND h.StartDate <> @OLD_StartDate AND h.EndDate <> @OLD_EndDate)
		BEGIN
			-- The record overlaps within the period
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70015)
			RAISERROR (@ErrorMessage, 16, 1)
			RETURN
		END
		BEGIN TRAN

		UPDATE dbo.DefaultRecruiterCommissionHeader
		   SET EndDate = @StartDate
		 WHERE PersonId = @PersonId AND EndDate = @OLD_StartDate

		UPDATE dbo.DefaultRecruiterCommissionHeader
		   SET StartDate = @EndDate
		 WHERE PersonId = @PersonId AND StartDate = @OLD_EndDate

		UPDATE dbo.DefaultRecruiterCommissionHeader
		   SET StartDate = @StartDate,
			   EndDate = @EndDate
		 WHERE DefaultRecruiterCommissionHeaderId = @DefaultRecruiterCommissionHeaderId

		COMMIT TRAN
	END
