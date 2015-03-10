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
	
	DECLARE @NewProjectStartDate DATETIME,
			@NewProjectEndDate	DATETIME,
			@ProjectId	INT,
			@OldProjectStartDate	DATETIME,
			@OldProjectEndDate		DATETIME,
			@W2SalaryTimescaleId INT,
			@W2HourlyTimescaleId INT

	SELECT	@W2SalaryTimescaleId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
	SELECT  @W2HourlyTimescaleId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'

	SELECT @ProjectId = P.ProjectId,
			@NewProjectStartDate = (SELECT MIN(StartDate) FROM dbo.Milestone AS m WHERE m.ProjectId = P.ProjectId),
			@NewProjectEndDate = (SELECT MAX(ProjectedDeliveryDate) FROM dbo.Milestone AS m WHERE m.ProjectId = P.ProjectId),
			@OldProjectStartDate = P.StartDate,
			@OldProjectEndDate = P.EndDate
	FROM dbo.Project P
	 WHERE EXISTS (SELECT 1 FROM inserted AS i WHERE i.ProjectId = P.ProjectId)
	    OR EXISTS (SELECT 1 FROM deleted AS i WHERE i.ProjectId = P.ProjectId)

	UPDATE dbo.Project
	   SET StartDate = @NewProjectStartDate,
	       EndDate = @NewProjectEndDate
	FROM Project P
	WHERE P.ProjectId = @ProjectId
	
	IF  ( SELECT UserLogin FROM SessionLogData WHERE SessionID = @@SPID) IS NULL
	BEGIN
		EXEC SessionLogPrepare @UserLogin = @UserLogin
	END

	UPDATE PTRS
		SET EndDate = @NewProjectEndDate + (7 - DATEPART(dw,@NewProjectEndDate))
	FROM [dbo].[PersonTimeEntryRecursiveSelection] PTRS
	WHERE PTRS.ProjectId = @ProjectId AND PTRS.IsRecursive = 1

	 --Updates commission attribution records as per the new project range for the overlapping records.
	UPDATE A																				
		SET A.StartDate = CASE WHEN A.StartDate < @NewProjectStartDate THEN @NewProjectStartDate ELSE A.StartDate END,--max startdate
			A.EndDate = CASE WHEN A.EndDate > @NewProjectEndDate THEN @NewProjectEndDate ELSE A.EndDate END--min enddate
	FROM [dbo].Attribution A
	INNER JOIN AttributionRecordTypes ART ON A.AttributionRecordTypeId = ART.AttributionRecordId AND ART.IsRangeType = 1
	WHERE A.ProjectId = @ProjectId AND A.StartDate <= @NewProjectEndDate AND @NewProjectStartDate <= A.EndDate AND 
			(
			 A.StartDate <> CASE WHEN A.StartDate < @NewProjectStartDate THEN @NewProjectStartDate ELSE A.StartDate END
				OR A.EndDate <> CASE WHEN A.EndDate > @NewProjectEndDate THEN @NewProjectEndDate ELSE A.EndDate END
			)
 
	DECLARE @Attributions TABLE(Id INT,AType INT)
	--Extending dates If there is any overlapping of new and old project dates.
	IF @NewProjectStartDate <= @OldProjectEndDate AND @OldProjectStartDate <= @NewProjectEndDate    
	BEGIN
			--Extend the attribution Start date if the project start date is extended as per the person pay/division/employment history
			UPDATE A 
			SET A.StartDate = CASE WHEN @NewProjectStartDate > pay.StartDate THEN @NewProjectStartDate ELSE pay.StartDate END
			FROM dbo.Attribution A
			INNER JOIN dbo.[v_PersonValidAttributionRange] pay ON pay.PersonId = A.TargetId 
			WHERE A.ProjectId = @ProjectId 
				AND A.AttributionRecordTypeId = 1 
				AND A.StartDate <= pay.EndDate 
				AND pay.StartDate <= A.EndDate
				AND @NewProjectStartDate < @OldProjectStartDate 
				AND A.StartDate = @OldProjectStartDate

			--Extend the attribution end date if the project end date is extended as per the person pay/division/employment  history
			UPDATE A
			SET A.EndDate = CASE WHEN @NewProjectEndDate < pay.EndDate  THEN @NewProjectEndDate ELSE pay.EndDate END
			FROM dbo.Attribution A
			INNER JOIN dbo.[v_PersonValidAttributionRange] pay ON pay.PersonId = A.TargetId 
			WHERE A.ProjectId = @ProjectId 
				AND A.AttributionRecordTypeId = 1 
				AND (A.StartDate <= pay.EndDate) 
				AND (pay.StartDate <= A.EndDate) 
				AND @NewProjectEndDate > @OldProjectEndDate 
				AND A.EndDate = @OldProjectEndDate  



			DELETE A
			FROM [dbo].Attribution A
			INNER JOIN AttributionRecordTypes ART ON A.AttributionRecordTypeId = ART.AttributionRecordId AND ART.IsRangeType = 1
			WHERE A.ProjectId = @ProjectId AND (A.StartDate > @NewProjectEndDate OR @NewProjectStartDate > A.EndDate)

	END
	ELSE 
	--If new project range is completly out of old project range. 
	BEGIN   
		  	INSERT INTO @Attributions
			SELECT A.TargetId,A.AttributionTypeId
			FROM dbo.Attribution A
			WHERE A.ProjectId = @ProjectId AND A.AttributionRecordTypeId = 1
			GROUP BY A.TargetId,A.AttributionTypeId

			DELETE A
			FROM dbo.Attribution A
			WHERE A.ProjectId = @ProjectId AND A.AttributionRecordTypeId = 1

			INSERT INTO Attribution(ProjectId,AttributionRecordTypeId,AttributionTypeId,TargetId,StartDate,EndDate,Percentage)
			SELECT @ProjectId,AR.AttributionRecordId,A.AType,A.Id,
					 CASE WHEN @NewProjectStartDate > pay.StartDate THEN @NewProjectStartDate ELSE pay.StartDate END,
					 CASE WHEN @NewProjectEndDate < pay.EndDate  THEN @NewProjectEndDate ELSE pay.EndDate END,100 AS Percentage
			FROM @Attributions A
			INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsRangeType = 1
			INNER JOIN dbo.[v_PersonValidAttributionRange] pay ON pay.PersonId = A.Id  
			WHERE (@NewProjectStartDate < pay.EndDate) AND (pay.StartDate <= @NewProjectEndDate)

	END
	
	IF  ( SELECT UserLogin FROM SessionLogData WHERE SessionID = @@SPID) IS NULL
	BEGIN
		EXEC SessionLogPrepare @UserLogin = @UserLogin
	END

