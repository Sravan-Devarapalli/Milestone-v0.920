CREATE PROCEDURE [dbo].[PersonListAllByStatusList] 
(
	@PersonStatusIdsList NVARCHAR(225) = NULL,
	@PersonId	INT = NULL
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @Query NVARCHAR(4000),
			@Where NVARCHAR(4000)
			
	DECLARE @PersonSeniorityId INT
		
	SET @Where = ''

	SET @Query='SELECT PersonId,
					   FirstName,
					   LastName,
					   IsDefaultManager
				FROM dbo.Person'
	  
	  IF(ISNULL(@PersonStatusIdsList,'')<>'')
	  BEGIN
		SET @Where = @Where + ' WHERE PersonStatusId IN (' + @PersonStatusIdsList + ')'
	  END
	  IF (ISNULL(@PersonId,'') <> '')
	  BEGIN
		SELECT @PersonSeniorityId = SeniorityId 
		FROM Person
		WHERE PersonId = @PersonId
				
		SET @Where = (CASE WHEN @Where='' THEN ' WHERE ' ELSE @Where+ ' AND ' END )
						+ 'PersonId = ' + CONVERT(NVARCHAR,@PersonId) 
						+ (CASE WHEN @PersonSeniorityId <= 65 -- According to 2656, Managers and up should be able to see their subordinates, but not equals.
								THEN ' OR SeniorityId > '+ CONVERT(NVARCHAR,@PersonSeniorityId)
								ELSE '' END
							)
	  END
	 SET @Query = @Query + @Where
	 EXEC(@Query)

END
