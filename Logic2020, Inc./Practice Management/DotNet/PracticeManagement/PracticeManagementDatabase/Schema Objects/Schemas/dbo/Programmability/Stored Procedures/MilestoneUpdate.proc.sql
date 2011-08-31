-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-22-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	11-27-2008
-- Description:	Updates a Milestone
-- =============================================
CREATE PROCEDURE dbo.MilestoneUpdate
(
	@MilestoneId              INT,
	@ProjectId                INT,
	@Description              NVARCHAR(255),
	@Amount                   DECIMAL(18,2),
	@StartDate                DATETIME,
	@ProjectedDeliveryDate    DATETIME,
	@ActualDeliveryDate       DATETIME,
	@IsHourlyAmount           BIT,
	@UserLogin                NVARCHAR(255),
	@ConsultantsCanAdjust	  BIT,
	@IsChargeable			  BIT,
	@IsStartDateChangeReflectedForMilestoneAndPersons BIT,
	@IsEndDateChangeReflectedForMilestoneAndPersons   BIT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsStartOrEndDateChanged BIT  = 0

	IF NOT EXISTS(SELECT 1 FROM dbo.Milestone as m WHERE m.MilestoneId = @MilestoneId AND m.StartDate = @StartDate AND m.ProjectedDeliveryDate = @ProjectedDeliveryDate)
	BEGIN
	SET @IsStartOrEndDateChanged = 1
	END

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	-- Change the milestone
	UPDATE dbo.Milestone
	   SET ProjectId = @ProjectId,
	       Description = @Description,
	       Amount = @Amount,
	       StartDate = @StartDate,
	       ProjectedDeliveryDate = @ProjectedDeliveryDate,
	       ActualDeliveryDate = @ActualDeliveryDate,
	       IsHourlyAmount = @IsHourlyAmount,
	       IsChargeable = @IsChargeable,
	       ConsultantsCanAdjust = @ConsultantsCanAdjust
	 WHERE MilestoneId = @MilestoneId


	 IF(@IsStartOrEndDateChanged = 1)
	 BEGIN
	 
		DECLARE @TempTable TABLE(MilestonePersonId INT)
		
		INSERT INTO @TempTable
		SELECT   mp.MilestonePersonId
		FROM [dbo].[MilestonePerson] AS mp
		INNER JOIN dbo.Person AS p ON mp.PersonId = p.PersonId
		WHERE  (mp.MilestoneId = @MilestoneId) 
		  AND  (
			    (p.TerminationDate < @StartDate) 
			    OR (p.HireDate > @ProjectedDeliveryDate)
			   )
	
		 
		DELETE MPE FROM dbo.MilestonePersonEntry MPE
		JOIN @TempTable Temp ON Temp.MilestonePersonId = MPE.MilestonePersonId

		 
		DELETE MP FROM dbo.MilestonePerson MP
		JOIN @TempTable Temp ON Temp.MilestonePersonId = MP.MilestonePersonId


	    UPDATE mpe
			   SET StartDate =  CASE WHEN @IsStartDateChangeReflectedForMilestoneAndPersons =1 THEN 
								( CASE
									 WHEN ( P.HireDate > @StartDate) THEN  P.HireDate
									 ELSE @StartDate
								   END
								) ELSE StartDate END,
				  EndDate = CASE WHEN  @IsEndDateChangeReflectedForMilestoneAndPersons = 1 THEN
								( CASE
									WHEN ( @ProjectedDeliveryDate > p.TerminationDate ) THEN  p.TerminationDate
									ELSE (@ProjectedDeliveryDate)
								  END
								) 
								ELSE EndDate END
			  FROM dbo.MilestonePersonEntry AS mpe
				   INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
				   INNER JOIN dbo.Person AS p ON p.PersonId = mp.PersonId
			 WHERE mp.MilestoneId = @MilestoneId AND (@IsStartDateChangeReflectedForMilestoneAndPersons = 1 OR @IsEndDateChangeReflectedForMilestoneAndPersons = 1 )

	    UPDATE mpe
			   SET EndDate =  CASE WHEN @IsStartDateChangeReflectedForMilestoneAndPersons =1 THEN 
								( CASE
									 WHEN ( mpe.StartDate > mpe.EndDate) THEN  mpe.StartDate
									 ELSE mpe.EndDate
								   END
								) ELSE mpe.EndDate END,
				  StartDate = CASE WHEN @IsEndDateChangeReflectedForMilestoneAndPersons =1 THEN 
								( CASE
									 WHEN ( mpe.StartDate > mpe.EndDate) THEN  mpe.EndDate
									 ELSE mpe.StartDate
								   END
								) ELSE mpe.StartDate END
			  FROM dbo.MilestonePersonEntry AS mpe
				   INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
				   INNER JOIN dbo.Person AS p ON p.PersonId = mp.PersonId
			 WHERE mp.MilestoneId = @MilestoneId AND (@IsStartDateChangeReflectedForMilestoneAndPersons = 1 OR @IsEndDateChangeReflectedForMilestoneAndPersons = 1 )

	  END


	-- End logging session
	EXEC dbo.SessionLogUnprepare
 END
