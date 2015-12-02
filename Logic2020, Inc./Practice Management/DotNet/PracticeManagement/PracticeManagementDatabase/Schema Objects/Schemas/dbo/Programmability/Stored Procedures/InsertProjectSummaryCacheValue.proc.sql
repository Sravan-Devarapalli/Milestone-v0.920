CREATE PROCEDURE [dbo].[InsertProjectSummaryCacheValue]
(
	@CurrentDate  DATETIME
)
AS
BEGIN 
	--SET NOCOUNT ON
	--SET ANSI_WARNINGS OFF
	
		DECLARE @StartDate DATETIME,
				@EndDate DATETIME ,
				@InsertingTime DATETIME,
				@ProjectId   NVARCHAR(MAX),
				@BackupDays INT = 3,
				@ErrorMessage NVARCHAR(MAX),
				@LogData NVARCHAR(200)
		SELECT @BackupDays = CONVERT(INT,Value) FROM dbo.Settings s WHERE s.SettingsKey = 'ProjectSummaryCacheValuesBackUpDays'
		SELECT @InsertingTime = dbo.InsertingTime()
		SELECT	@BackupDays = ISNULL(@BackupDays ,3),
				@StartDate = DATEADD(yy,-1,DATEADD(yy,DATEDIFF(yy,0,@InsertingTime),0)),
				@EndDate = DATEADD(ms,-3,DATEADD(yy,0,DATEADD(yy,DATEDIFF(yy,0,@InsertingTime)+1,0)))

		SELECT  @ProjectId = ISNULL(@ProjectId,'') + ',' + CONVERT(VARCHAR,P.ProjectId)
		FROM	dbo.Project AS P
		WHERE	P.StartDate IS NOT NULL AND P.EndDate IS NOT NULL
				AND P.ProjectId NOT IN (SELECT ProjectId FROM dbo.DefaultMilestoneSetting)
				AND P.IsAllowedToShow = 1
		
		
			BEGIN TRY
				BEGIN TRAN  ProjSummaryCacheValue_Tran

				DELETE [dbo].[ProjectSummaryCache] 
				WHERE CacheDate = CONVERT(DATE,@InsertingTime - @BackupDays)
				
				--SET @LogData = 'Delete Sucessfully FROM [InsertProjectSummaryCacheValue]'
				--EXEC [dbo].[UserActivityLogInsert]  @ActivityTypeID	= 6, @LogData = @LogData

				COMMIT TRAN  ProjSummaryCacheValue_Tran
			END TRY
			BEGIN CATCH
				ROLLBACK TRAN ProjSummaryCacheValue_Tran
				SET @ErrorMessage = 'From Sproc' +ERROR_MESSAGE()
				RAISERROR( @ErrorMessage, 16, 1)
			END CATCH

			BEGIN TRY
				BEGIN TRAN  ProjSummaryCacheValue_Tran2
					IF NOT EXISTS (SELECT 1 FROM [dbo].[ProjectSummaryCache]  WHERE CacheDate = CONVERT(DATE,@InsertingTime) and ismonthlyrecorD = 0)
						BEGIN
							INSERT  [dbo].[ProjectSummaryCache] 
							([ProjectId],[MonthStartDate],[MonthEndDate],RangeType,ProjectRevenue,ProjectRevenueNet,Cogs,GrossMargin,ProjectedhoursperMonth,Expense,ReimbursedExpense,ActualRevenue,ActualGrossMargin,PreviousMonthActualRevenue,PreviousMonthActualGrossMargin,IsMonthlyRecord,CreatedDate,CacheDate) 
							EXEC dbo.FinancialsListByProjectPeriodTotal @UseActuals=1,@ProjectId = @ProjectId 
							
							--SET @LogData = 'Inserted Sucessfully FROM [InsertProjectSummaryCacheValue] for Total'
							--EXEC [dbo].[UserActivityLogInsert]  @ActivityTypeID	= 6, @LogData = @LogData
						END	
				COMMIT TRAN  ProjSummaryCacheValue_Tran2
			END TRY
			BEGIN CATCH
				ROLLBACK TRAN ProjSummaryCacheValue_Tran2
				SET @ErrorMessage = 'From Sproc' +ERROR_MESSAGE()
				RAISERROR( @ErrorMessage, 16, 1)
			END CATCH

			SET @LogData = '<Cache><NEW_VALUES Status="Cached the Projects Data" CacheTime = "'+ CONVERT(NVARCHAR,@CurrentDate) +'"></NEW_VALUES><OLD_VALUES /></Cache>'
			EXEC [dbo].[UserActivityLogInsert]  @ActivityTypeID	= 6, @LogData = @LogData
END

