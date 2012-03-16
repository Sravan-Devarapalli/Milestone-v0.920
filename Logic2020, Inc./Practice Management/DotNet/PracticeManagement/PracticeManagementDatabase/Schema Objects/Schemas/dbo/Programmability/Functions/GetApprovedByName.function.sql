-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 03-15-2012
-- Description:  Time Entries grouped by workType and Resource for a Project.
-- =========================================================================
CREATE FUNCTION [dbo].[GetApprovedByName](@Date DATETIME,@ORTTimeTypeId INT,@PersonId INT)
RETURNS NVARCHAR(1000)
AS
BEGIN
	DECLARE  @ApprovedBy NVARCHAR(1000)
	
	SELECT @ApprovedBy = ' Approved by ' + P.FirstName + ', '+p.LastName + '.'
	FROM dbo.PersonCalendar AS PC
	INNER JOIN  dbo.Person AS P ON P.PersonId = PC.ApprovedBy
	WHERE PC.Date= @Date AND PC.TimeTypeId = @ORTTimeTypeId AND Pc.PersonId = @PersonId

	RETURN @ApprovedBy
END
