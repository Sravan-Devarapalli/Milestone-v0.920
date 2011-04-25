CREATE PROCEDURE [dbo].[ProjectsListByClientWithSort]
(
	@ClientId INT,
	@Alias VARCHAR(100) = NULL ,
	@SortBy			NVARCHAR(225) = NULL
)
AS
	SET NOCOUNT ON
	DECLARE @SqlQuery NVARCHAR(4000)
	DECLARE @OrderBy NVARCHAR(1000)
	
	IF ISNULL(@SortBy,'') = ''
	BEGIN
		SET @OrderBy = 'ORDER BY  ProjectStatusName,Name'
	END
	ELSE
	BEGIN	
		IF (CHARINDEX(' desc', @SortBy) > 0)
		BEGIN
			SET @OrderBy = 'ORDER BY ' + REPLACE(@SortBy,' DESC',' DESC, Name DESC')
			SET @OrderBy = REPLACE(@OrderBy,'Name DESC, Name DESC','Name DESC')
		END
		ELSE
		BEGIN
			SET @OrderBy = 'ORDER BY ' + @SortBy + ', Name'
			SET @OrderBy = REPLACE(@OrderBy,'Name, Name','Name')
		END 	    
	END
	
	SELECT @SqlQuery = 'IF @Alias IS NULL   
		SELECT p.ClientId,
			   p.ProjectId,
			   p.Discount,
			   p.Terms,
			   p.Name,
			   p.PracticeManagerId,
			   p.PracticeId,
			   p.StartDate,
			   p.EndDate,
			   p.ClientName,
			   p.PracticeName,
			   p.ProjectStatusId,
			   p.ProjectStatusName,
			   p.ProjectNumber,
			   p.BuyerName,
			   p.OpportunityId,
			   p.GroupId,
			   p.ClientIsChargeable,
			   p.ProjectIsChargeable,
			   p.ProjectManagerId,
			   p.ProjectManagerFirstName,
			   p.ProjectManagerLastName,
			   p.DirectorId,
			   p.DirectorFirstName,
			   p.DirectorLastName,
			   pg.Name AS GroupName,
			   1 InUse 
		  FROM dbo.v_Project AS p
		  INNER JOIN dbo.ProjectGroup AS pg 
		  ON p.GroupId = pg.GroupId
		  WHERE p.ClientId = @ClientId
		' + @OrderBy +' 
	ELSE 
	BEGIN 
		DECLARE @PersonId INT 
		
		SELECT @PersonId = PersonId 
		FROM dbo.Person
		WHERE Alias = @Alias;
	
		WITH Perms AS (
			SELECT TargetId,
				   TargetType
			FROM dbo.Permission 
			WHERE PersonId = @PersonId
		)
		SELECT p.ClientId,
			   p.ProjectId,
			   p.Discount,
			   p.Terms,
			   p.Name,
			   p.PracticeManagerId,
			   p.PracticeId,
			   p.StartDate,
			   p.EndDate,
			   p.ClientName,
			   p.PracticeName,
			   p.ProjectStatusId,
			   p.ProjectStatusName,
			   p.ProjectNumber,
			   p.BuyerName,
			   p.OpportunityId,
			   p.GroupId,
			   p.ClientIsChargeable,
			   p.ProjectIsChargeable,
			   p.ProjectManagerId,
			   p.ProjectManagerFirstName,
			   p.ProjectManagerLastName,
			   p.DirectorId,
			   p.DirectorFirstName,
			   p.DirectorLastName,
			   pg.Name AS GroupName,
			   1 InUse 
		  FROM dbo.v_Project AS p
		  INNER JOIN dbo.ProjectGroup AS pg 
		  ON p.GroupId = pg.GroupId
		 WHERE 
			p.ClientId = @ClientId AND
			(
				p.GroupId IN (SELECT TargetId FROM Perms AS s WHERE s.TargetType = 2) OR
				p.PracticeId IN (SELECT TargetId FROM Perms AS s WHERE s.TargetType = 5) OR
				p.PracticeManagerId IN (SELECT TargetId FROM Perms AS s WHERE s.TargetType = 4) OR
				EXISTS ( SELECT	1
							  FROM	 dbo.v_PersonProjectCommission AS c
							  WHERE	 c.ProjectId = p.ProjectId
										AND c.PersonId IN (SELECT TargetId FROM Perms AS s WHERE s.TargetType = 3)
										AND c.CommissionType = 1)
			)'
		+ @OrderBy+'
	END'
	
	EXEC sp_executeSql 
				@SqlQuery,		
				N'@ClientId	INT, @Alias VARCHAR(100) = NULL',				
				@ClientId = @ClientId, @Alias = @Alias



