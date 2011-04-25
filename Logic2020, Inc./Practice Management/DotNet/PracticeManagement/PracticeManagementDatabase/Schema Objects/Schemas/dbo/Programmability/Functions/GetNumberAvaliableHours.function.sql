-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-10-29
-- Description:	Gets number of avaliable hours for the person
-- =============================================
CREATE FUNCTION dbo.GetNumberAvaliableHours
    (
      @PersonId INT,
      @startDate DATETIME,
      @endDate DATETIME,
   	  @ActiveProjects BIT = 1,
	  @ProjectedProjects BIT = 1,
	  @ExperimentalProjects BIT = 1	,
	  @InternalProjects		BIT = 1
    )
RETURNS INT
AS BEGIN
    DECLARE @res INT ;

   IF dbo.GetCurrentPayType(@PersonId) = 2 
   BEGIN	
		WITH    DefaultHours
				  AS ( SELECT   Date,
								dbo.GetDefaultHoursPerDay(PersonId, Date) AS defhrs
					   FROM     dbo.v_PersonCalendar
					   WHERE    Date BETWEEN @startDate AND @endDate
								AND @PersonId = PersonId
								AND DayOff = 0
					 )
			SELECT  @res = SUM(defhrs)
			FROM    DefaultHours
	END 
    ELSE
		SET @res = ISNULL(dbo.GetNumberProjectedHours(@PersonId, @startDate, @endDate, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects,@InternalProjects), 0)
    
    RETURN @res
   END

