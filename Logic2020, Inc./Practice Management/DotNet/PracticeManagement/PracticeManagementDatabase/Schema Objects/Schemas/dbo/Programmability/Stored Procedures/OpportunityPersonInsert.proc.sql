CREATE PROCEDURE [dbo].[OpportunityPersonInsert]
(
	@OpportunityId INT, 
	@PersonIdList NVARCHAR(MAX),
	@OutSideResources NVARCHAR(4000),
	@RelationTypeId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @PersonIdListLocal NVARCHAR(MAX)
	SELECT  @PersonIdListLocal = @PersonIdList
	
	DECLARE @OpportunityPersonIdsWithTypeTable TABLE
	(
	PersonId INT,
	PersonType INT,
	Quantity INT,
	NeedBy  DATETIME
	)

	IF(@RelationTypeId = 1) --Proposed Resources
	BEGIN
		IF(@PersonIdList IS NOT NULL)
		BEGIN
			INSERT INTO @OpportunityPersonIdsWithTypeTable(PersonId,PersonType)
			SELECT ResultId ,ResultType
			FROM [dbo].[ConvertStringListIntoTableWithTwoColoumns] (@PersonIdListLocal)

			DELETE op
			FROM dbo.OpportunityPersons  op
			LEFT JOIN @OpportunityPersonIdsWithTypeTable AS p 
			ON op.OpportunityId = @OpportunityId AND op.PersonId = p.PersonId AND op.OpportunityPersonTypeId=p.PersonType
			WHERE p.PersonId IS NULL and OP.OpportunityId = @OpportunityId AND op.RelationTypeId = @RelationTypeId

			INSERT INTO OpportunityPersons(OpportunityId,PersonId,OpportunityPersonTypeId,RelationTypeId)
			SELECT @OpportunityId ,p.PersonId,p.PersonType,@RelationTypeId 
			FROM @OpportunityPersonIdsWithTypeTable AS p 
			LEFT JOIN dbo.OpportunityPersons op
			ON p.PersonId = op.PersonId AND op.OpportunityId=@OpportunityId AND op.OpportunityPersonTypeId=p.PersonType
			WHERE op.PersonId IS NULL
		END
	END
	ELSE
	BEGIN
		IF(ISNULL(@PersonIdList,'')<>'')
		BEGIN
			DECLARE @PersonIdListLocalXML XML
			IF(SUBSTRING(@PersonIdListLocal,LEN(@PersonIdListLocal),1)=',')
			SET @PersonIdListLocal = SUBSTRING(@PersonIdListLocal,1,LEN(@PersonIdListLocal)-1)
			SET @PersonIdListLocal = '<root><item><personid>'+@PersonIdListLocal+'</needby></item></root>'

			SET @PersonIdListLocal = REPLACE(@PersonIdListLocal,':','</personid><persontypeid>')
			SET @PersonIdListLocal = REPLACE(@PersonIdListLocal,'|','</persontypeid><qty>')
			SET @PersonIdListLocal = REPLACE(@PersonIdListLocal,'?','</qty><needby>')
			SET @PersonIdListLocal = REPLACE(@PersonIdListLocal,',','</needby></item><item><personid>')
			
			SELECT @PersonIdListLocalXML  = CONVERT(XML,@PersonIdListLocal)

			INSERT INTO @OpportunityPersonIdsWithTypeTable
			(PersonId ,
			PersonType ,
			Quantity,
			NeedBy)
			SELECT C.value('personid[1]','int') personid,
					C.value('persontypeid[1]','int') persontypeid,
					C.value('qty[1]','int') qty,
					C.value('needby[1]','DATETIME') needby
			FROM @PersonIdListLocalXML.nodes('/root/item') as T(C)
		END

		DELETE FROM dbo.OpportunityPersons
		WHERE opportunityId = @OpportunityId AND RelationTypeId = @RelationTypeId
			
		INSERT INTO OpportunityPersons(OpportunityId,PersonId,OpportunityPersonTypeId,RelationTypeId,Quantity,NeedBy)
		SELECT @OpportunityId ,p.PersonId,p.PersonType,@RelationTypeId,p.Quantity,p.NeedBy
		FROM @OpportunityPersonIdsWithTypeTable AS p 
   END
END
