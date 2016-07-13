CREATE PROCEDURE [dbo].[InsertInternalProject]
(
	@Name NVARCHAR(100),
	@ProjectNumberSeries NVARCHAR(10),
	@DivisionId INT,
	@PracticeId INT
)
AS
BEGIN
	
	-- sereies P91, P92, P93, P94, P95, P97
	

	--Create a project Number with the given series
	DECLARE  @ProjectNumber NVARCHAR (12), @ErrorMessage NVARCHAR(MAX)
	DECLARE @LowerLimitRange INT ,@HigherLimitRange INT ,@NextProjectNumber INT
	SET @LowerLimitRange = 1
		SET @HigherLimitRange = 9999
		SET @ErrorMessage = 'Internal project code not avaliable'

	DECLARE @ProjectRanksList TABLE (projectNumber INT,projectNumberRank INT)
	INSERT INTO @ProjectRanksList 
	SELECT Convert(INT,SUBSTRING(p.ProjectNumber,4,4)) as projectNumber ,
		   RANK() OVER (ORDER BY Convert(INT,SUBSTRING(p.ProjectNumber,4,4))) AS  projectNumberRank
		FROM dbo.Project p 
		WHERE LEN(p.ProjectNumber) = 7
		AND	 ISNUMERIC( SUBSTRING(ProjectNumber,4,4))  = 1 
		AND SUBSTRING(p.ProjectNumber,4,4)!='0000'
		AND SUBSTRING(ProjectNumber,2,2)= @ProjectNumberSeries
		
	INSERT INTO @ProjectRanksList 
	SELECT -1,MAX(projectNumberRank)+1 FROM @ProjectRanksList 

		SELECT TOP 1 @NextProjectNumber = projectNumberRank 
		FROM @ProjectRanksList  
		WHERE projectNumber != projectNumberRank
		ORDER BY projectNumberRank

	IF (@NextProjectNumber IS NULL AND NOT EXISTS(SELECT 1 FROM @ProjectRanksList WHERE projectNumber != -1))
	BEGIN 
		SET  @NextProjectNumber = @LowerLimitRange
	END 
	ELSE IF (@NextProjectNumber > @HigherLimitRange )
	BEGIN
		RAISERROR (@ErrorMessage, 16, 1)
	END

	SET @ProjectNumber = 'P'+@ProjectNumberSeries+ REPLICATE('0',4-LEN(@NextProjectNumber)) + CONVERT(NVARCHAR,@NextProjectNumber)

	--Insert Project

	DECLARE @ClientId INT,
			
			@GroupId INT

	SELECT @ClientId=ClientId 
		   FROM dbo.Client 
		   WHERE Name='Logic20/20'

	SELECT @GroupId=GroupId 
		   FROM ProjectGroup 
		   WHERE Name='Operations' AND ClientId = @ClientId

	INSERT INTO dbo.Project(ClientId, Discount, Terms, ProjectNumber, PracticeId, ProjectStatusId, Name, GroupId, IsChargeable, IsAdministrative, IsAllowedtoShow, IsInternal, CanCreateCustomWorkTypes, IsNoteRequired, IsSeniorManagerUnassigned,IsBusinessDevelopment,DivisionId)
	VALUES (@ClientId,0.0,0,@ProjectNumber,@PracticeId,3,@Name,@GroupId,1,0,0,1,1,1,0,0,@DivisionId)
END

