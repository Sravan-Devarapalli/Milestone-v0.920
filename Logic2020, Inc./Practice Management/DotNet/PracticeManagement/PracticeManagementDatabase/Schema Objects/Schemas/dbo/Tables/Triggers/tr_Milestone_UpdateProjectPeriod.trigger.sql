
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-12-2008
-- Updated by:	Srinivas.M
-- Update Date:	05-21-2012
-- Description:	Updates a Project's Start and End Dates according to the data in the Milestone table
-- =============================================
CREATE TRIGGER tr_Milestone_UpdateProjectPeriod
ON dbo.Milestone
AFTER INSERT, UPDATE, DELETE
AS
	SET NOCOUNT ON
		
	DECLARE @UserLogin NVARCHAR(50)
	
	SELECT @UserLogin = UserLogin
	FROM SessionLogData
	WHERE SessionID = @@SPID
	
	DECLARE @StartDate DATETIME,
			@EndDate	DATETIME,
			@ProjectId	INT

	SELECT @ProjectId = P.ProjectId,
			@StartDate = (SELECT MIN(StartDate) FROM dbo.Milestone AS m WHERE m.ProjectId = P.ProjectId),
			@EndDate = (SELECT MAX(ProjectedDeliveryDate) FROM dbo.Milestone AS m WHERE m.ProjectId = P.ProjectId)
	FROM dbo.Project P
	 WHERE EXISTS (SELECT 1 FROM inserted AS i WHERE i.ProjectId = P.ProjectId)
	    OR EXISTS (SELECT 1 FROM deleted AS i WHERE i.ProjectId = P.ProjectId)

	UPDATE dbo.Project
	   SET StartDate = @StartDate,
	       EndDate = @EndDate
	FROM Project P
	WHERE P.ProjectId = @ProjectId

	UPDATE PTRS
		SET EndDate = @EndDate + (7 - DATEPART(dw,@EndDate))
	FROM [dbo].[PersonTimeEntryRecursiveSelection] PTRS
	WHERE PTRS.ProjectId = @ProjectId AND PTRS.IsRecursive = 1

	UPDATE A
		SET A.StartDate = CASE WHEN A.StartDate < @StartDate THEN @StartDate ELSE A.StartDate END,--max startdate
			A.EndDate = CASE WHEN A.EndDate > @EndDate THEN @EndDate ELSE A.EndDate END--min enddate
	FROM [dbo].Attribution A
	INNER JOIN AttributionRecordTypes ART ON A.AttributionRecordTypeId = ART.AttributionRecordId AND ART.IsRangeType = 1
	WHERE A.ProjectId = @ProjectId AND A.StartDate <= @EndDate AND @StartDate <= A.EndDate AND 
			(
			 A.StartDate <> CASE WHEN A.StartDate < @StartDate THEN @StartDate ELSE A.StartDate END
				OR A.EndDate <> CASE WHEN A.EndDate > @EndDate THEN @EndDate ELSE A.EndDate END
			)

	DELETE A
	FROM [dbo].Attribution A
	INNER JOIN AttributionRecordTypes ART ON A.AttributionRecordTypeId = ART.AttributionRecordId AND ART.IsRangeType = 1
	WHERE A.ProjectId = @ProjectId AND (A.StartDate > @EndDate OR @StartDate > A.EndDate)

	
	IF  ( SELECT UserLogin FROM SessionLogData WHERE SessionID = @@SPID) IS NULL
	BEGIN
		EXEC SessionLogPrepare @UserLogin = @UserLogin
	END

