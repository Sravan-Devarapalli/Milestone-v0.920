CREATE FUNCTION ISDayLightSavingTime
(
	@dateTime DATETIME
)
RETURNS BIT
AS
BEGIN

	DECLARE @MarchFirst DATETIME ,
			@DayOfTheWeekMarchFirst INT,
			@DayOfTheWeekNovFirst INT,
			@NovemberFirst DATETIME,
			@MarchSecondSunday DATETIME,
			@NovFirstSunday DATETIME
		
	SELECT @MarchFirst = CONVERT(NVARCHAR,YEAR(@dateTime)) +'-03-01' ,
			@NovemberFirst =CONVERT(NVARCHAR,YEAR(@dateTime))+'-11-01'
	SELECT @DayOfTheWeekMarchFirst =DATEPART(DW, @MarchFirst),
			@DayOfTheWeekNovFirst = DATEPART(DW,@NovemberFirst)

	SELECT   @MarchSecondSunday = CASE WHEN @DayOfTheWeekMarchFirst >1 THEN  @MarchFirst+15-@DayOfTheWeekMarchFirst ELSE @MarchFirst + 7 END,
			@NovFirstSunday = CASE WHEN @DayOfTheWeekNovFirst >1 THEN @NovemberFirst +8 -@DayOfTheWeekNovFirst ELSE @DayOfTheWeekNovFirst END

	IF @dateTime BETWEEN @MarchSecondSunday AND @NovFirstSunday - 1
	BEGIN
		RETURN 1
	END

	RETURN 0	
	
END	
