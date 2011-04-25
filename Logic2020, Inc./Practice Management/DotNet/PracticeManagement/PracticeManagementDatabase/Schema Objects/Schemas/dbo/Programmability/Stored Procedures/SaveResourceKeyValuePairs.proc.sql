CREATE PROCEDURE dbo.SaveResourceKeyValuePairs
(
	@TypeId   INT,
	@ResourcesKey NVARCHAR(255),
	@Value      NVARCHAR(MAX)
)
AS
BEGIN
	SET NOCOUNT ON
	IF NOT EXISTS(SELECT TOP 1 1 
				  FROM [dbo].[StringResources]
				  WHERE @ResourcesKey = ResourcesKey 
				  AND TypeId = @TypeId 
				 )
	BEGIN
		INSERT INTO [dbo].[StringResources]
					([TypeId]
					,[ResourcesKey]
					,[Value])
		 VALUES
					(@TypeId
					,@ResourcesKey
					,@Value)
           
	END
	ELSE 
	BEGIN
		UPDATE [dbo].[StringResources]
		SET  Value = @Value
		WHERE ResourcesKey = @ResourcesKey  AND TypeId = @TypeId
	END
END
           
