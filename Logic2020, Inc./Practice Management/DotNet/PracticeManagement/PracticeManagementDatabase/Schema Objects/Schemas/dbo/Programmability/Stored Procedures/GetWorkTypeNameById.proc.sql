CREATE PROCEDURE [dbo].[GetWorkTypeNameById]
(
	@TimeTypeId INT
)
AS
BEGIN
	SELECT Name from dbo.TimeType WHERE TimeTypeId = @TimeTypeId
END

