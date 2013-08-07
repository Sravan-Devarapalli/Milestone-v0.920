-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 03-15-2012
-- Description:  Time Entries grouped by workType and Resource for a Project.
-- Updated By : ThulasiRam.P
-- Modified Date : 03-21-2012
-- =========================================================================
CREATE FUNCTION [dbo].[GetApprovedByName]
    (
      @Date DATETIME ,
      @TimeTypeId INT ,
      @PersonId INT
    )
RETURNS NVARCHAR(1000)
AS 
    BEGIN
        DECLARE @ApprovedBy NVARCHAR(1000)
        SET @ApprovedBy = ''

        SELECT  @ApprovedBy = CASE WHEN  PC.ApprovedBy = Pc.PersonId THEN ' Entered by ' ELSE  ' Approved by ' END + P.FirstName + ' ' + p.LastName + '.'
        FROM    dbo.PersonCalendar AS PC
                INNER JOIN dbo.Person AS P ON P.PersonId = PC.ApprovedBy
        WHERE   PC.Date = @Date
                AND PC.TimeTypeId = @TimeTypeId
                AND Pc.PersonId = @PersonId

        RETURN @ApprovedBy
    END

