CREATE PROCEDURE [dbo].[SetProjectTimeTypes]
(
	@ProjectId INT,
	@ProjectTimeTypes NVARCHAR(MAX) 
)
AS
BEGIN
	/*
		For DefaultTimetypes 
		
		insert the record that are not in the list but there in the default timetype list (with  isallowedtoshow as 0)

		delete the record that are there in the list and the default timetype list


		For CustomTimeTypes

		Delete the records that are not in the list and there in the projecttimetype table

		insert the records that are there in the list but not there in the table.

	*/
	
	DECLARE @DefaultTimeTypesList TABLE(TimeTypeId INT)
	INSERT INTO @DefaultTimeTypesList (TimeTypeId)
	SELECT TimeTypeId FROM dbo.TimeType WHERE IsDefault = 1

	-- Convert TimeType ids from string to TABLE
	DECLARE @ProjectTimeTypesList TABLE (TimeTypeId INT)
	INSERT INTO @ProjectTimeTypesList (TimeTypeId)
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectTimeTypes)

	--FOR CUSTOM TIMETYPES
	
	DELETE FROM dbo.ProjectTimeType 
	WHERE projectId = @ProjectId AND TimetypeId NOT IN (SELECT * FROM @ProjectTimeTypesList)

	INSERT INTO dbo.ProjectTimeType (projectId,TimetypeId,isallowedtoshow)
	SELECT @ProjectId ,customProjectTimeTypes.TimeTypeId ,1 
	FROM  (SELECT TimeTypeId FROM @ProjectTimeTypesList EXCEPT SELECT TimeTypeId FROM @DefaultTimeTypesList) customProjectTimeTypes 
	LEFT JOIN (SELECT * FROM dbo.ProjectTimeType WHERE ProjectId = @ProjectId ) ptt ON ptt.TimeTypeId = customProjectTimeTypes.TimeTypeId  
	WHERE ptt.TimeTypeId IS NULL

	--For Defalult TimeTypes

	DELETE FROM dbo.ProjectTimeType 
	WHERE projectId = @ProjectId AND TimetypeId IN (SELECT * FROM @ProjectTimeTypesList INTERSECT SELECT TimeTypeId FROM @DefaultTimeTypesList)

	INSERT INTO dbo.ProjectTimeType (projectId,TimetypeId,isallowedtoshow)
	SELECT @ProjectId,TimeTypeId,0 FROM
	(SELECT TimeTypeId FROM @DefaultTimeTypesList  EXCEPT SELECT TimeTypeId FROM @ProjectTimeTypesList) dttl

END
