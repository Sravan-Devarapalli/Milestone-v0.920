-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 2012-02-10
-- Description:	Gets Administrative ChargeCodeValues for AdministrativeTimeType 
-- =============================================
CREATE PROCEDURE dbo.GetAdministrativeChargeCodeValues
(
  @TimeTypeId INT
)
AS
BEGIN

	SELECT TOP(1)	CC.ClientId AS 'ClientId', 
			CC.ProjectGroupId AS 'GroupId', 
			CC.ProjectId AS 'ProjectId' 
	FROM dbo.ChargeCode CC
	WHERE CC.TimeTypeId = @TimeTypeId

	
END

