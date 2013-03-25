/* Views in Consultant Demand Pages:
1.Summary View- @StartDate,@EndDate,@IsSummary
2.Details View -@StartDate,@EndDate,@IsDetail
	a.Group by Month-@GroupByMonth,@ViewByTitleSkill
	b.Group by Title/Skill-@GroupByTitleSkill,@ViewByTitleSkill
	c.Group by Title :@GroupByTitle,@ViewBySkill
	d.Group by Skill :@GroupBySkill,@ViewByTitle
3.Graphs View  @StartDate,@EndDate,@IsGraph,
	a.Group By Month: @GroupByMonth
		(i).View By Title  : @ViewByTitle
		(ii).View By Skill : @ViewBySkill
	b.Group By Title :@GroupByTitle,@ViewByTitle
	c.Group By Skill :@GroupBySkill,@ViewBySkill
*/
CREATE PROCEDURE dbo.GetConsultantDemandForPeriod
 (
	@StartDate DATETIME,
	@EndDate DATETIME,
	@Titles VARCHAR(MAX) = NULL,
	@Skills VARCHAR(MAX) = NULL,
	--Report Type
	@IsSummary BIT=0,
	@IsDetail BIT=0,
	@IsGraph BIT=0,
	--Group By
	@GroupByMonth BIT=0,
	@GroupByTitle BIT=0,
	@GroupBySkill BIT=0,
	@GroupByTitleSkill BIT=0,
	--View By
	@ViewByTitle BIT = 0,
	@ViewBySkill BIT = 0,
	@ViewByTitleSkill BIT = 0
)
AS
BEGIN

	DECLARE @Query NVARCHAR(4000) = ' FROM [dbo].[v_ConsultingDemand] CD ',
			@GroupBy NVARCHAR(500) = ' GROUP BY ',
			@OrderBy NVARCHAR(500) = ' ORDER BY ',
			@Select NVARCHAR(500) = 'SELECT ',
			@Where  NVARCHAR(500) = ' WHERE CD.MonthStartDate BETWEEN DATEADD(D,-DAY(@StartDate)+1,@StartDate) AND @EndDate'+
									CASE WHEN @Titles IS NOT NULL THEN ' AND CD.Title IN (''' + REPLACE(@Titles,',',''',''') + ''')'  ELSE '' END +
									CASE WHEN @Skills IS NOT NULL THEN ' AND CD.Skill IN (''' + REPLACE(@Skills,',',''',''') + ''')' ELSE '' END ,
			@Columns NVARCHAR(500) = ''

	--1.Summary View
	IF @IsSummary=1
	BEGIN

		SELECT @GroupBy = @GroupBy + 'CD.MonthStartDate,CD.Title,CD.Skill',
				@OrderBy = @OrderBy + 'CD.MonthStartDate,CD.Title,CD.Skill',
				@Select = @Select+ 'CD.MonthStartDate,CD.Title,CD.Skill,SUM(COUNT) AS [COUNT]'

	END
	--2.Details View 
	ELSE IF @IsDetail=1 
	BEGIN
		
		SELECT @Columns = 'CD.ClientId,CD.AccountName,CD.ProjectId,CD.ProjectNumber,CD.ProjectName,CD.OpportunityId,CD.OpportunityNumber,CD.ResourceStartDate' + 
			CASE 
				WHEN @GroupByMonth = 1 THEN ',CD.MonthStartDate'
				WHEN @GroupByTitleSkill = 1 THEN ',CD.Title,CD.Skill'
				WHEN @GroupByTitle = 1 THEN ',CD.Title'
				WHEN @GroupBySkill = 1 THEN ',CD.Skill'
				ELSE ''
			END + 
			CASE 
				WHEN @GroupByTitleSkill = 0 AND @ViewByTitleSkill = 1 THEN ',CD.Title,CD.Skill'
				WHEN @GroupByTitleSkill = 0 AND @GroupByTitle = 0 AND @ViewByTitle = 1 THEN ',CD.Title'
				WHEN @GroupByTitleSkill = 0 AND @GroupBySkill = 0 AND @ViewBySkill = 1 THEN ',CD.Skill'
				ELSE ''
			END,
			@OrderBy = @OrderBy + ' 1 '
			SELECT	@Select = @Select + @Columns + ',SUM(COUNT) AS [COUNT]',
					@GroupBy = @GroupBy + @Columns

	END
	--2.Graph View 
	ELSE IF @IsGraph=1 
	BEGIN

	SELECT @Columns =
			CASE 
				WHEN @GroupByMonth = 1 THEN 'CD.MonthStartDate'
				WHEN @GroupByTitleSkill = 1 THEN 'CD.Title,CD.Skill'
				WHEN @GroupByTitle = 1 THEN 'CD.Title'
				WHEN @GroupBySkill = 1 THEN 'CD.Skill'
				ELSE ''
			END + 
			CASE 
				WHEN @GroupByTitleSkill = 0 AND @ViewByTitleSkill = 1 THEN ',CD.Title,CD.Skill'
				WHEN @GroupByTitleSkill = 0 AND @GroupByTitle = 0 AND @ViewByTitle = 1 THEN ',CD.Title'
				WHEN @GroupByTitleSkill = 0 AND @GroupBySkill = 0 AND @ViewBySkill = 1 THEN ',CD.Skill'
				ELSE ''
			END,
			@OrderBy = @OrderBy + ' 1 '

			SELECT	@Select = @Select + @Columns + ',SUM(COUNT) AS [COUNT]',
					@GroupBy = @GroupBy + @Columns

	END
	
	SELECT @Query = @Select + @Query + @Where + @GroupBy + @OrderBy

	--Print @Query
	EXEC sp_executeSql  @Query,N'@StartDate DATETIME,@EndDate DATETIME',@StartDate=@StartDate,@EndDate=@EndDate

END

