-- =============================================
-- Author:		Nikita G.
-- Create date: 2009-12-24
-- Description:	Returns all time entries
-- Modified By & Date: Thulasiram P & 11/2/2010
-- =============================================
CREATE PROCEDURE [dbo].[TimeEntriesAll]
    @PersonIds VARCHAR(MAX) = NULL,
    @MilestoneDateFrom DATETIME = NULL,
    @MilestoneDateTo DATETIME = NULL,
    @ForecastedHoursFrom REAL = NULL,
    @ForecastedHoursTo REAL = NULL,
    @ActualHoursFrom REAL = NULL,
    @ActualHoursTo REAL = NULL,
    @ProjectIds VARCHAR(MAX) = NULL,
    @MilestonePersonId INT = NULL,
    @MilestoneId INT = NULL,
    @TimeTypeId INT = NULL,
    @Notes VARCHAR(50) = NULL,
    @IsReviewed INT = NULL,
    @IsCorrect BIT = NULL,
    @EntryDateFrom DATETIME = NULL,
    @EntryDateTo DATETIME = NULL,
    @ModifiedDateFrom DATETIME = NULL,
    @ModifiedDateTo DATETIME = NULL,
    @ModifiedBy INT = NULL,
    @SortExpression VARCHAR(500) = NULL,
	@IsChargeable BIT = NULL,
	@RequesterId INT,
	@IsProjectChargeable BIT = NULL,
	@PageSize      INT = NULL,
	@PageNo        INT = NULL
AS 
    BEGIN
        SET NOCOUNT ON ;
        
    DECLARE @SameProjectCondition VARCHAR(1000)    
    
    -- Get users roles
    --DECLARE @RequesterId INT 
    --SET @RequesterId = 3526
    
    DECLARE @IsAdminOrPM BIT,
			 @FirstRecord INT,
			 @LastRecord INT
			 
	SET @FirstRecord = @PageSize * @PageNo
	SET @LastRecord = @FirstRecord + @PageSize
	
    
    ;WITH RequestersRoles AS (
		SELECT r.RoleName 
		FROM dbo.Person AS p
		INNER JOIN dbo.aspnet_Users AS u ON u.UserName = p.Alias
		INNER JOIN dbo.aspnet_UsersInRoles AS ur ON u.UserId = ur.UserId
		INNER JOIN dbo.aspnet_Roles AS r ON ur.RoleId = r.RoleId
		WHERE p.PersonId = @RequesterId
    )
    SELECT @IsAdminOrPM = 1
    FROM RequestersRoles AS roles 
    WHERE roles.RoleName = 'Administrator' OR roles.RoleName = 'Practice Manager'
    
    IF ISNULL(@IsAdminOrPM, 0) = 0
    BEGIN    
		SET @SameProjectCondition = ' ' + 
			'AND PersonID IN (SELECT persons.PersonId ' + 
			'FROM dbo.MilestonePerson AS persons ' + 
			'WHERE persons.MilestoneId IN( ' + 
			'	SELECT mp.MilestoneId  ' + 
			'	FROM dbo.MilestonePerson AS mp WHERE mp.PersonId = ' + CAST(@RequesterId AS VARCHAR) + 
			'))' 
    END
    ELSE
		SET @SameProjectCondition = '';
	
	
	/*
		---	Describe filtering conditions
	*/
	DECLARE @PersonIdCondition VARCHAR(max)
		
	IF(ISNULL(@PersonIds, '') <> '')
	BEGIN
		SET @PersonIdCondition = ' AND ( PersonId IN (' + ISNULL(@PersonIds, ''' ''') + ') )' 
	END
	ELSE
	BEGIN
		SET @PersonIdCondition = ' '
	END

	DECLARE @MilestoneDateCondition VARCHAR(100)
	SET @MilestoneDateCondition = 
		CASE 
			WHEN (@MilestoneDateFrom IS NULL) OR (@MilestoneDateTo IS NULL) THEN ' '
			ELSE ' AND ( [MilestoneDate] BETWEEN ''' + CONVERT(VARCHAR(8), @MilestoneDateFrom, 10) + '''
                                         AND ''' + CONVERT(VARCHAR(8), @MilestoneDateTo, 10) + ''' )'
		END 
		
	DECLARE @ForecastedHoursCondition VARCHAR(100)
	SET @ForecastedHoursCondition = 
		CASE 
			WHEN @ForecastedHoursFrom IS NULL
                      OR @ForecastedHoursTo IS NULL THEN ' '
			ELSE ' AND ( ForecastedHours >= ' + CAST(@ForecastedHoursFrom AS VARCHAR) + '
                        AND ForecastedHours <= ' + CAST(@ForecastedHoursTo AS VARCHAR) + ' )'
		END 
		
	DECLARE @ActualHoursCondition VARCHAR(100)
	SET @ActualHoursCondition = 
		CASE 
			WHEN @ActualHoursFrom IS NULL OR @ActualHoursTo IS NULL THEN ' '
			ELSE ' AND ( ActualHours >= ' + CAST(@ActualHoursFrom AS VARCHAR) + '
                        AND ActualHours <= ' + CAST(@ActualHoursTo AS VARCHAR) + ' )'
		END 
		
	DECLARE @ProjectIdsCondition VARCHAR(MAX)	
	IF(ISNULL(@ProjectIds, '') <> '')
	BEGIN
		SET @ProjectIdsCondition = ' AND ( ProjectId IN (' + ISNULL(@ProjectIds, ''' ''') + ') )'
	END
	ELSE
	BEGIN
		SET @ProjectIdsCondition = ' '
	END
		
	IF @IsProjectChargeable IS NOT NULL
		SET @ProjectIdsCondition = @ProjectIdsCondition + ' AND (IsProjectChargeable = ' + CAST(@IsProjectChargeable AS VARCHAR) + ')'
		
	DECLARE @MilestoneIdCondition VARCHAR(100)
	SET @MilestoneIdCondition = 
		CASE 
			WHEN @MilestoneId IS NULL THEN ' '
			ELSE ' AND ( MilestoneId = ' + CAST(@MilestoneId AS VARCHAR) + ' )'
		END 
		
	DECLARE @MilestonePersonIdCondition VARCHAR(100)
	SET @MilestonePersonIdCondition = 
		CASE 
			WHEN @MilestonePersonId IS NULL THEN ' '
			ELSE ' AND ( MilestonePersonId = ' + CAST(@MilestonePersonId AS VARCHAR) + ' )'
		END 
		
	DECLARE @TimeTypeIdCondition VARCHAR(100)
	SET @TimeTypeIdCondition = 
		CASE 
			WHEN @TimeTypeId IS NULL THEN ' '
			ELSE ' AND ( TimeTypeId = ' + CAST(@TimeTypeId AS VARCHAR) + ' )'
		END 
		
	DECLARE @NotesCondition VARCHAR(100)
	SET @NotesCondition = 
		CASE 
			WHEN @Notes IS NULL THEN ' '
			ELSE ' AND (Note LIKE ''%' + @Notes + '%'')'
		END 
		
	DECLARE @IsReviewedCondition VARCHAR(100)
	SET @IsReviewedCondition = 
		CASE 
			WHEN @IsReviewed IS NULL THEN ' '
			WHEN @IsReviewed = 2 THEN ' AND ( IsReviewed IS NULL )'
			ELSE ' AND ( IsReviewed = ' + CAST(@IsReviewed AS VARCHAR) + ' )'
		END 
		
	DECLARE @IsCorrectCondition VARCHAR(100)
	SET @IsCorrectCondition = 
		CASE 
			WHEN @IsCorrect IS NULL THEN ' '
			ELSE ' AND ( IsCorrect = ' + CAST(@IsCorrect AS VARCHAR) + ' )'
		END 
		
	DECLARE @IsChargeableCondition VARCHAR(100)
	SET @IsChargeableCondition = 
		CASE 
			WHEN @IsChargeable IS NULL THEN ' '
			ELSE ' AND ( IsChargeable = ' + CAST(@IsChargeable AS VARCHAR) + ' )'
		END 
		
	DECLARE @ModifiedByCondition VARCHAR(100)
	SET @ModifiedByCondition = 
		CASE 
			WHEN @ModifiedBy IS NULL THEN ' '
			ELSE ' AND ( ModifiedBy = ' + CAST(@ModifiedBy AS VARCHAR) + ' )'
		END 
		
	DECLARE @EntryDateCondition VARCHAR(100)
	SET @EntryDateCondition = 
		CASE 
			WHEN @EntryDateFrom IS NULL OR @EntryDateTo IS NULL THEN ' '
			ELSE ' AND ( [EntryDate] BETWEEN ''' + CONVERT(VARCHAR(8), @EntryDateFrom, 10) + ''' AND '''+ 
						CONVERT(VARCHAR(8), @EntryDateTo, 10) + ''')'
		END 
		
	DECLARE @ModifiedDateCondition VARCHAR(100)
	SET @ModifiedDateCondition = 
		CASE 
			WHEN @ModifiedDateFrom IS NULL OR @ModifiedDateTo IS NULL THEN ' '
			ELSE ' AND ( [ModifiedDate] BETWEEN ''' + CONVERT(VARCHAR(8), @ModifiedDateFrom, 10) + ''' AND ''' +     
						CONVERT(VARCHAR(8), @ModifiedDateTo, 10) + ''' )'
		END 
	DECLARE @query VARCHAR(max)
	IF @FirstRecord IS NULL
	BEGIN
		SET @query = '
			SELECT  *
			FROM    v_TimeEntries
			WHERE   1=1 ' 
				+ @PersonIdCondition 
				+ @MilestoneDateCondition 
				+ @ForecastedHoursCondition 
				+ @ActualHoursCondition 
				+ @ProjectIdsCondition 
				+ @MilestoneIdCondition 
				+ @MilestonePersonIdCondition
				+ @TimeTypeIdCondition 
				+ @NotesCondition 
				+ @IsReviewedCondition 
				+ @ModifiedByCondition 
				+ @IsChargeableCondition 
				+ @IsCorrectCondition
				+ @EntryDateCondition 
				+ @ModifiedDateCondition
				+ @SameProjectCondition
		 + ISNULL(@SortExpression, ' ')
	END
	ELSE
	BEGIN
		IF ISNULL(@SortExpression,'') = ''
		SET @SortExpression =' ORDER BY ModifiedDate '

		SET @query = '
			SELECT *
			FROM (
			SELECT  TOP ('+CONVERT(VARCHAR(10),@LastRecord)+') 
				*,
				ROW_NUMBER() OVER(' + @SortExpression + ') - 1 AS rownum
			FROM    v_TimeEntries
			WHERE   1=1 ' 
				+ @PersonIdCondition 
				+ @MilestoneDateCondition 
				+ @ForecastedHoursCondition 
				+ @ActualHoursCondition 
				+ @ProjectIdsCondition 
				+ @MilestoneIdCondition 
				+ @MilestonePersonIdCondition
				+ @TimeTypeIdCondition 
				+ @NotesCondition 
				+ @IsReviewedCondition 
				+ @ModifiedByCondition 
				+ @IsChargeableCondition 
				+ @IsCorrectCondition
				+ @EntryDateCondition 
				+ @ModifiedDateCondition
				+ @SameProjectCondition
				+ @SortExpression
				+ ') Temp WHERE rownum BETWEEN '
				+ CONVERT(VARCHAR(10),@FirstRecord)+ ' AND ' 
				+ CONVERT(VARCHAR(10),@LastRecord - 1)
	   --PRINT @query 
	END
	
	EXEC(@query)    
    
    END

