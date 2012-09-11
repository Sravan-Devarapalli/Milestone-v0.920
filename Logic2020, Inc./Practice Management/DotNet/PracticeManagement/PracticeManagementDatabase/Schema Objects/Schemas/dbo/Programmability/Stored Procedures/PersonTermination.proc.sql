-- =============================================
-- Author:		Sainath C
-- Create date: 24/05/2012
-- Description:	Terminates the Person with given termination date.
-- =============================================
CREATE PROCEDURE [dbo].[PersonTermination]
(
	@PersonId			INT,
	@TerminationDate    DATETIME,
	@PersonStatusId     INT
)
AS
BEGIN
	/*
		1.Delete time entries after termination date.
		2.Adjust the person milestone end date with the given termination date.
			if milestone person start date is greater than person termination date deleted the milestone person record
		3.Set client director as project owner to the adjusted projects.(Note If person status is terminated perform this action else if person status termination pending/contigent(with termination date) dont perform it)
		4.Close a current compensation for the terminated persons
		5.Delete all the Compensation records later @TerminationDate
		6.Delete all the Recursive records later @TerminationDate
	*/

	DECLARE @ErrorMessage NVARCHAR(2048),@FutureDate DATETIME 

	SELECT @FutureDate = dbo.GetFutureDate()

	BEGIN TRY
	--1.Delete time entries after the termination date

	DELETE TEH
	FROM dbo.TimeEntryHours TEH
		INNER JOIN dbo.TimeEntry TE ON TEH.TimeEntryId = TE.TimeEntryId 
										AND TE.PersonId = @PersonId
										AND TE.ChargeCodeDate > @TerminationDate
	DELETE TE
	FROM dbo.TimeEntry TE 
	WHERE  TE.PersonId = @PersonId
			AND TE.ChargeCodeDate > @TerminationDate

	--2.Adjust the person milestone end date with the given termination date.
	--	if milestone person start date is greater than person termination date deleted the milestone person record		
	
	UPDATE MPE
	SET MPE.EndDate = @TerminationDate
	FROM dbo.MilestonePersonEntry MPE
		 INNER JOIN dbo.MilestonePerson MP ON MPE.MilestonePersonId = MP.MilestonePersonId
		 INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
		 INNEr JOIN dbo.Project P ON M.ProjectId = P.ProjectId AND P.IsAdministrative = 0
	WHERE MP.PersonId = @PersonId
		  AND @TerminationDate BETWEEN MPE.StartDate and MPE.EndDate

	DELETE  MPE
	FROM dbo.MilestonePersonEntry MPE
		 INNER JOIN dbo.MilestonePerson MP ON MPE.MilestonePersonId = MP.MilestonePersonId
		 INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
		 INNEr JOIN dbo.Project P ON M.ProjectId = P.ProjectId AND P.IsAdministrative = 0
	WHERE MP.PersonId = @PersonId
		  AND @TerminationDate < MPE.StartDate 
	  
	--3.Set client director as project owner to the projects for which the person is project owner.
	
	UPDATE Pro
	SET Pro.ProjectOwnerId = Pro.DirectorId
	FROM dbo.Project Pro 
	INNER JOIN dbo.Person P ON P.PersonId = @PersonId
								AND Pro.ProjectOwnerId = P.PersonId
								AND Pro.DirectorId IS NOT NULL
								AND Pro.ProjectOwnerId != Pro.DirectorId
	WHERE @PersonStatusId = 2 -- termination status


	--4.Close a current compensation for the terminated persons
	UPDATE dbo.Pay
		SET EndDate = @TerminationDate + 1
		WHERE Person = @PersonId AND EndDate > @TerminationDate + 1
			AND StartDate < @TerminationDate + 1

	--5.Delete all the Compensation records later @TerminationDate
	DELETE FROM dbo.Pay
	WHERE Person = @PersonId AND StartDate >= @TerminationDate + 1
						

	--6.Delete all the Recursive records later @TerminationDate
	DECLARE @WeekDayOfTerminationDate DATETIME
	SELECT @WeekDayOfTerminationDate = @TerminationDate +(7-DATEPART(weekday,@TerminationDate))

	DELETE PTRS
	FROM dbo.PersonTimeEntryRecursiveSelection AS PTRS
	WHERE PTRS.StartDate > @WeekDayOfTerminationDate AND PTRS.PersonId = @PersonId

	UPDATE PTRS
	SET PTRS.EndDate = @WeekDayOfTerminationDate
	FROM dbo.PersonTimeEntryRecursiveSelection AS PTRS
	WHERE @WeekDayOfTerminationDate BETWEEN PTRS.StartDate AND ISNULL(PTRS.EndDate,@FutureDate) AND PTRS.PersonId = @PersonId

	END TRY
	BEGIN CATCH
		DECLARE	 @ERROR_STATE			TINYINT
		,@ERROR_SEVERITY		TINYINT
		,@ERROR_MESSAGE		    NVARCHAR(2000)
		,@InitialTranCount		TINYINT

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)

	END CATCH
END

