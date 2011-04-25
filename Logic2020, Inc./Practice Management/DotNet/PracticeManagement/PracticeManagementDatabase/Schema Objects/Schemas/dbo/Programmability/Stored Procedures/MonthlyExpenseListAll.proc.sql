-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-28-2008
-- Updated by:  Anatoliy Lokshin
-- Update date: 11-26-2008
-- Description:	List all monthly expenses
-- =============================================
CREATE PROCEDURE [dbo].[MonthlyExpenseListAll]
(
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON

	DECLARE @PivotDefinition VARCHAR(2000)
	DECLARE @PivotColumns VARCHAR(2000)
	DECLARE @Delimiter CHAR

	-- Generate PIVOT definitions
	SELECT @PivotDefinition =
	          ISNULL(@PivotDefinition + @Delimiter, '') +
	          '[' + CAST(YEAR(cal.Date) AS VARCHAR) + '-' +
	          CASE WHEN MONTH(cal.Date) < 10 THEN '0' ELSE '' END + CAST(MONTH(cal.Date) AS VARCHAR) + '-01]',
	       @PivotColumns =
	          ISNULL(@PivotColumns + @Delimiter, '') +
	          'ISNULL([' + CAST(YEAR(cal.Date) AS VARCHAR) + '-' +
	          CASE WHEN MONTH(cal.Date) < 10 THEN '0' ELSE '' END + CAST(MONTH(cal.Date) AS VARCHAR) + '-01], 0) AS ' +
	          '[' + CAST(YEAR(cal.Date) AS VARCHAR) + CASE WHEN MONTH(cal.Date) < 10 THEN '0' ELSE '' END + CAST(MONTH(cal.Date) AS VARCHAR) + ']',
	       @Delimiter = ','
	  FROM dbo.Calendar AS cal
	 WHERE cal.Date BETWEEN @StartDate AND @EndDate
	GROUP BY YEAR(cal.Date), MONTH(cal.Date)
	ORDER BY YEAR(cal.Date), MONTH(cal.Date)

	DECLARE @Expression VARCHAR(8000)

	-- Generate DSQL
	SET @Expression =
		' SELECT Name AS ItemName,ExpenseBasisName,ExpenseBasisId,ExpenseCategoryName,WeekPaidOptionName,' +
		@PivotColumns +
		' FROM (SELECT MIN(cal.Date) AS Date,' +
               'me.Name,' +
               'me.ExpenseBasisId,' +
               'b.Name AS ExpenseBasisName,' +
               'me.ExpenseCategoryId,' +
               'c.Name AS ExpenseCategoryName,' +
               'me.WeekPaidOptionId,' +
               'po.Name AS WeekPaidOptionName,' +
		    ' CASE me.ExpenseBasisId' +
                 ' WHEN 2' +
                 ' THEN SUM(ISNULL(f.PersonMilestoneDailyAmount * me.Amount / 100, 0))' +
                 ' WHEN 3' +
                 ' THEN SUM(ISNULL((ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)) * f.PersonHoursPerDay * me.Amount / 100, 0))' +
                 ' WHEN 4' +
                 ' THEN SUM(ISNULL(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount - ISNULL((ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)) * f.PersonHoursPerDay, 0), 0) * me.Amount / 100)' +
                 ' ELSE CAST(0 AS DECIMAL)' +
             ' END AS Amount' +
		' FROM dbo.Calendar AS cal' +
             ' LEFT JOIN dbo.MonthlyExpense AS me' +
             '     ON YEAR(cal.Date) = me.Year AND MONTH(cal.Date) = me.Month' +
             ' LEFT JOIN dbo.v_FinancialsRetrospective AS f' +
             '     ON cal.Date = f.Date' +
             ' LEFT JOIN dbo.ExpenseBasis AS b' +
             '     ON me.ExpenseBasisId = b.ExpenseBasisId' +
             ' LEFT JOIN dbo.ExpenseCategory AS c' +
             '     ON me.ExpenseCategoryId = c.ExpenseCategoryId' +
             ' LEFT JOIN dbo.WeekPaidOption AS po' +
             '     ON me.WeekPaidOptionId = po.WeekPaidOptionId' +
		' WHERE cal.Date BETWEEN ''' +
		CAST(YEAR(@StartDate) AS VARCHAR) + '-' + CAST(MONTH(@StartDate) AS VARCHAR) + '-01' + ''' AND ''' +
		CAST(YEAR(@EndDate) AS VARCHAR) + '-' + CAST(MONTH(@EndDate) AS VARCHAR) + '-' + CAST(dbo.GetDaysInMonth(@EndDate) AS VARCHAR) + ''' AND me.ExpenseBasisId <> 1' +
		' GROUP BY me.Name, me.Amount, me.Year, me.Month, me.ExpenseBasisId, b.Name, me.ExpenseCategoryId, c.Name, me.WeekPaidOptionId, po.Name ' +
     ' ) AS f' +
     ' PIVOT ' +
     ' (SUM(f.Amount) FOR [Date] IN (' + @PivotDefinition + ')) AS pvt' +
	' UNION ALL' +
	' SELECT Name AS ItemName,ExpenseBasisName,ExpenseBasisId,ExpenseCategoryName,WeekPaidOptionName,' +
	    @PivotColumns +
		' FROM (SELECT MIN(cal.Date) AS Date,' +
               'me.Name,' +
               'me.ExpenseBasisId,' +
               'b.Name AS ExpenseBasisName,' +
               'me.ExpenseCategoryId,' +
               'c.Name AS ExpenseCategoryName,' +
               'me.WeekPaidOptionId,' +
               'po.Name AS WeekPaidOptionName,' +
	           'ISNULL(me.Amount, 0) AS Amount' +
		' FROM dbo.Calendar AS cal' +
             ' LEFT JOIN dbo.MonthlyExpense AS me' +
             '     ON YEAR(cal.Date) = me.Year AND MONTH(cal.Date) = me.Month' +
             ' LEFT JOIN dbo.ExpenseBasis AS b' +
             '     ON me.ExpenseBasisId = b.ExpenseBasisId' +
             ' LEFT JOIN dbo.ExpenseCategory AS c' +
             '     ON me.ExpenseCategoryId = c.ExpenseCategoryId' +
             ' LEFT JOIN dbo.WeekPaidOption AS po' +
             '     ON me.WeekPaidOptionId = po.WeekPaidOptionId' +
		' WHERE cal.Date BETWEEN ''' +
		CAST(YEAR(@StartDate) AS VARCHAR) + '-' + CAST(MONTH(@StartDate) AS VARCHAR) + '-01' + ''' AND ''' +
		CAST(YEAR(@EndDate) AS VARCHAR) + '-' + CAST(MONTH(@EndDate) AS VARCHAR) + '-' + CAST(dbo.GetDaysInMonth(@EndDate) AS VARCHAR) + ''' AND me.ExpenseBasisId = 1' +
		' GROUP BY me.Name, me.Amount, me.Year, me.Month, me.ExpenseBasisId, b.Name, me.ExpenseCategoryId, c.Name, me.WeekPaidOptionId, po.Name ' +
     ' ) AS f' +
     ' PIVOT ' +
     ' (SUM(f.Amount) FOR [Date] IN (' + @PivotDefinition + ')) AS pvt' +
	 ' ORDER BY Name'

	EXEC (@Expression)

