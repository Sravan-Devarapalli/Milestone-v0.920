CREATE PROCEDURE [dbo].[SetProjectAttributionValues]
	(
	@ProjectId			INT,
	@AttributionXML		XML,
	@UserLogin			NVARCHAR(255)
	)
AS
BEGIN
	SET ANSI_NULLS ON;
	SET NOCOUNT ON;
	/*
	<Attributions>
		<AttributionType AttributionTypeId="">
			<AttributionRecordType AttributionRecordTypeId="">
				<Attribution AttributionId="" TargetId="" TargetName="" StartDate="" EndDate="" Percentage="" TitleId="" Title="" IsEditMode="" IsNewEntry="" IsCheckboxChecked="" IsRepeaterEntry="">
				</Attribution>
			</AttributionRecordType>
        <AttributionType>
	</Attributions>
	*/
	BEGIN TRY
		 BEGIN TRAN ProjectAttribution

		 EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
		 	DECLARE @AttributionValues TABLE(AttributionId				INT			NULL,
											 AttributionTypeId			INT			NOT NULL,
											 AttributionRecordTypeId	INT			NOT NULL,
											 TargetId					INT			NOT NULL,
											 StartDate					DATETIME	NULL,
											 EndDate					DATETIME	NULL,
											 Percentage					DECIMAL(5,2) NULL) 

            INSERT INTO @AttributionValues
			SELECT	NEW.c.value('@AttributionId', 'INT'),
					NEW.c.value('..[1]/..[1]/@AttributionTypeId', 'INT'),
					NEW.c.value('..[1]/@AttributionRecordTypeId', 'INT'),
					NEW.c.value('@TargetId', 'INT'),
					NEW.c.value('@StartDate', 'DATETIME'),
					NEW.c.value('@EndDate', 'DATETIME'),
					NEW.c.value('@Percentage', 'DECIMAL(5,2)')
				
			FROM @AttributionXML.nodes('Attributions/AttributionType/AttributionRecordType/Attribution') NEW(c)
			
			UPDATE @AttributionValues
			SET StartDate = NULL,
				EndDate = NULL
			WHERE AttributionRecordTypeId = 2

			--Delete values in Attribution table if not exists in XML
			DELETE	A
			FROM	dbo.Attribution  A
					LEFT JOIN @AttributionValues AV ON A.AttributionId = AV.AttributionId
			WHERE	AV.AttributionId IS NULL AND A.ProjectId = @ProjectId

			--Update values in Attribution table if AttributionId exists in both the table and XML
			UPDATE	A 
			SET		A.TargetId = AV.TargetId,
					A.StartDate = AV.StartDate,
					A.EndDate = AV.EndDate,
					A.Percentage = AV.Percentage
			FROM	dbo.Attribution  A
					INNER JOIN @AttributionValues AV ON A.AttributionId = AV.AttributionId 
			WHERE A.TargetId <> AV.TargetId OR
					A.StartDate <> AV.StartDate OR
					A.EndDate <> AV.EndDate OR
					A.Percentage <> AV.Percentage

			--Insert values into Attribution table if not exists in Attribution table(i.e. If it has no AttributionId)
			INSERT INTO dbo.Attribution(AttributionTypeId,AttributionRecordTypeId,ProjectId,TargetId,StartDate,EndDate,Percentage)
			SELECT	AV.AttributionTypeId,
					AV.AttributionRecordTypeId,
					@ProjectId,
					AV.TargetId,
					AV.StartDate,
					AV.EndDate,
					AV.Percentage
			FROM @AttributionValues AS AV
			WHERE AV.AttributionId < 0

			

		 EXEC dbo.SessionLogUnprepare

		 COMMIT TRAN ProjectAttribution
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN ProjectAttribution

		DECLARE @ErrorMessage NVARCHAR(2000)
		SET @ErrorMessage = ERROR_MESSAGE()

		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH
END

