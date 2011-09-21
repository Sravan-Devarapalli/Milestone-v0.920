CREATE PROCEDURE [dbo].[SaveQuickLinksForDashBoard]
(
	@linkNameList NVARCHAR(MAX), 
	@virtualPathList NVARCHAR(MAX),
	@dashBoardType INT 
)
AS
BEGIN

		DECLARE @linkNameListLocal TABLE
		(
			linkName NVARCHAR(255),
			RowNumber INT IDENTITY(1,1) NOT NULL
		)

		DECLARE @ResultId NVARCHAR(255), @Pos INT

		SET @linkNameList = LTRIM(RTRIM(@linkNameList))+ ','
		SET @Pos = CHARINDEX(',', @linkNameList, 1)

		IF REPLACE(@linkNameList, ',', '') <> ''
		BEGIN
			WHILE @Pos > 0
			BEGIN
				SET @ResultId = LTRIM(RTRIM(LEFT(@linkNameList, @Pos - 1)))
				IF @ResultId <> ''
				BEGIN
					INSERT INTO @linkNameListLocal (linkName) 
					VALUES (@ResultId)
				END
				SET @linkNameList = RIGHT(@linkNameList, LEN(@linkNameList) - @Pos)
				SET @Pos = CHARINDEX(',', @linkNameList, 1)

			END
		END	

		---
		DECLARE @virtualPathListLocal TABLE
		(
			virtualPath NVARCHAR(255),
			RowNumber INT IDENTITY(1,1) NOT NULL
		)

		DECLARE @Result NVARCHAR(255), @Position INT

		SET @virtualPathList = LTRIM(RTRIM(@virtualPathList))+ ','
		SET @Position = CHARINDEX(',', @virtualPathList, 1)

		IF REPLACE(@virtualPathList, ',', '') <> ''
		BEGIN
			WHILE @Position > 0
			BEGIN
				SET @Result = LTRIM(RTRIM(LEFT(@virtualPathList, @Position - 1)))
				IF @Result <> ''
				BEGIN
					INSERT INTO @virtualPathListLocal(virtualPath) 
					VALUES (@Result)
				END
				SET @virtualPathList = RIGHT(@virtualPathList, LEN(@virtualPathList) - @Position)
				SET @Position = CHARINDEX(',', @virtualPathList, 1)

			END
		END	


		---

		BEGIN TRAN T1;

		DELETE FROM  QuickLinks
		WHERE DashBoardTypeId = @dashBoardType

		INSERT INTO QuickLinks(LinkName,VirtualPath,DashBoardTypeId)
		SELECT ln.linkName,vpl.virtualPath,@dashBoardType
		FROM @linkNameListLocal AS ln
		JOIN @virtualPathListLocal AS vpl ON ln.RowNumber = vpl.RowNumber

		COMMIT TRAN T1;



END
	
