CREATE FUNCTION dbo.GetAvgUtilization
(
	@PersonId INT,
	@StartDate DATETIME,
	@DaysForward INT = 184,
	@ActiveProjects BIT = 1,
	@ProjectedProjects BIT = 1,
	@ExperimentalProjects BIT = 1,
	@InternalProjects	BIT = 1  
)
RETURNS INT
AS
BEGIN
	  
    
    DECLARE @wUtil INT
    DECLARE @av INT 
	DECLARE @EndRange DATETIME
	
	SET @EndRange = DATEADD(dd , @DaysForward, @StartDate) - 1
    
    -- Iterate through all days

		BEGIN
			SET @av = dbo.GetNumberAvaliableHours(@PersonId, @StartDate, @EndRange, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects ,@InternalProjects)
			
			IF (@av = 0 OR @av IS NULL)
				SET @wUtil = 0
			ELSE 		
				SET @wUtil = CEILING(
					100*ISNULL(dbo.GetNumberProjectedHours(@PersonId, @StartDate, @EndRange, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects, @InternalProjects), 0) / 
						dbo.GetNumberAvaliableHours(@PersonId, @StartDate, @EndRange, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects, @InternalProjects))
		END 
 
   return @wUtil

END

