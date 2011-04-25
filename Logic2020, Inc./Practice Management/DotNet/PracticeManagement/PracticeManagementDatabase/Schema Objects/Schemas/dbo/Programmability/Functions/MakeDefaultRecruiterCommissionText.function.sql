-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-02-2008
-- Updated by:	
-- Update date:	
-- Description:	Converts the Default Recruiter commission items to the text line.
-- =============================================
CREATE FUNCTION [dbo].[MakeDefaultRecruiterCommissionText]
(
	@DefaultRecruiterCommissionHeaderId   INT
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	DECLARE @Result NVARCHAR(4000)

	SELECT @Result = ISNULL(@Result + ', $', '$') +
	       CAST(i.Amount AS NVARCHAR) + ' after ' +
	       CAST(ROUND(i.HoursToCollect / 8, 0) AS NVARCHAR) + ' days'
	  FROM dbo.DefaultRecruiterCommissionItem AS i
	 WHERE i.DefaultRecruiterCommissionHeaderId = @DefaultRecruiterCommissionHeaderId
	ORDER BY i.HoursToCollect

	RETURN @Result
END

