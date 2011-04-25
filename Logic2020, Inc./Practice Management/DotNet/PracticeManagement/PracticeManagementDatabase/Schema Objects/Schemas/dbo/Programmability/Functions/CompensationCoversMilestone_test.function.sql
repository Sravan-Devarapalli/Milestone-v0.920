-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-10-21
-- Description:	Checks if there's compensation record as in #886
-- =============================================
CREATE FUNCTION dbo.CompensationCoversMilestone_test
(
	@PersonId INT,
	@mile_start DATETIME = NULL,
	@mile_end DATETIME = NULL 
)
RETURNS BIT
AS BEGIN
    IF @PersonId IS NULL 
        RETURN 1
    
    --needed for debugging purposes
    --DECLARE @PersonId INT
    --SET @PersonId = 3587
        
    DECLARE @isActive BIT 
    SELECT TOP 1 @isActive = CASE WHEN PersonStatusId = 1 THEN 1 ELSE 0 END 
    FROM dbo.Person WHERE PersonId = @PersonId
    
    --needed for debugging purposes
    --SELECT * FROM dbo.Person WHERE PersonId = @PersonId
    --PRINT @isActive
    
    IF @isActive = 0 
    RETURN 1
		
    DECLARE @Result BIT
	
    DECLARE @pay_start DATETIME
    DECLARE @pay_end DATETIME

	-- finding the last copmpensation record
    DECLARE @Now DATETIME
    SET @Now = GETDATE() ;
	
    WITH    LastPay
              AS ( SELECT   p.StartDate,
                            p.EndDate
                   FROM     dbo.v_Pay AS p
                   WHERE    p.PersonId = @PersonId
                            AND @Now >= p.StartDate
                            AND @Now < p.EndDateOrig
                   UNION ALL
                   SELECT TOP 1
                            p.StartDate,
                            p.EndDate
                   FROM     dbo.v_Pay AS p
                   WHERE    p.PersonId = @PersonId
                            AND NOT EXISTS ( SELECT 1
                                             FROM   dbo.v_Pay AS p
                                             WHERE  p.PersonId = @PersonId
                                                    AND @Now >= p.StartDate
                                                    AND @Now < p.EndDateOrig )
                            AND @Now < p.StartDate
                   ORDER BY p.StartDate
                 )
        SELECT  @pay_start = StartDate,
                @pay_end = EndDate
        FROM    LastPay
        
    -- @pay_end = NULL means no end date, so set it to max datettime
    IF @pay_end IS NULL
		SET @pay_end = CAST('9999-01-01' AS DATETIME)
	        
	-- Finding the last milestone record if no dates were specified
    IF @mile_start IS NULL AND @mile_end IS NULL
    BEGIN 
		SELECT TOP 1
				@mile_start = mp.StartDate,
				@mile_end = mp.EndDate
		FROM    dbo.v_MilestonePerson AS mp
		WHERE   mp.PersonId = @PersonId
		ORDER BY mp.StartDate DESC
    END 
	
    --	If there's compensation, but no projects, return true
    IF @pay_start IS NOT NULL AND @pay_end IS NOT NULL 
		AND @mile_end IS NULL AND @mile_start IS NULL
	RETURN 1
		   
	-- check if pay milestone dates are inside milestone dates
    IF /* DATEDIFF(day, @mile_start, @pay_end) >= 0 AND */
		DATEDIFF(day, @mile_end, @pay_end) >= 0
        SET @Result = 1
    ELSE 
        SET @Result = 0
	
	-- needed for debugging purposes
    /*PRINT @pay_start
    PRINT @pay_end
    PRINT @mile_start
    PRINT @mile_end
    PRINT @Result
    PRINT DATEDIFF(day, @pay_start, @mile_start)
    PRINT DATEDIFF(day, @mile_end, @pay_start)*/
	
    RETURN @Result
   END

