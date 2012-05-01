CREATE PROCEDURE [dbo].[SetProjectTimeTypes]
    (
      @ProjectId INT ,
      @ProjectTimeTypes NVARCHAR(MAX)
    )
AS 
    BEGIN
	/*
		For DefaultTimetypes 
		
		insert the record that are not in the list but there in the default timetype list (with  isallowedtoshow as 0)

		delete the record that are there in the list and the default timetype list


		For CustomTimeTypes

		Delete the records that are not in the list and there in the project timetype table

		insert the records that are there in the list but not there in the table.

	*/
	
        DECLARE @DefaultTimeTypesList TABLE ( TimeTypeId INT )
        INSERT  INTO @DefaultTimeTypesList
                ( TimeTypeId
                )
                SELECT  TimeTypeId
                FROM    dbo.TimeType
                WHERE   IsDefault = 1
	-- Convert TimeType ids from string to TABLE
        DECLARE @ProjectTimeTypesList TABLE ( TimeTypeId INT )
        INSERT  INTO @ProjectTimeTypesList
                ( TimeTypeId
                )
                SELECT  *
                FROM    dbo.ConvertStringListIntoTable(@ProjectTimeTypes)


        DECLARE @ErrorMessage NVARCHAR(MAX) = '';
        WITH    CTE
                  AS ( SELECT   TT.Name
                       FROM     dbo.ProjectTimeType AS PTT
                                INNER JOIN dbo.ChargeCode AS CC ON CC.TimeTypeId = PTT.TimeTypeId
                                                              AND CC.ProjectId = @ProjectId
                                INNER JOIN dbo.TimeEntry AS TE ON TE.ChargeCodeId = CC.Id
                                INNER JOIN dbo.TimeType AS TT ON TT.TimeTypeId = PTT.TimeTypeId
                                                              AND CC.TimeTypeId = PTT.TimeTypeId
                       WHERE    PTT.TimeTypeId NOT IN (
                                SELECT  *
                                FROM    @ProjectTimeTypesList )
                       UNION
                       SELECT   tType.Name
                       FROM     ( SELECT    TimeTypeId
                                  FROM      @DefaultTimeTypesList
                                  EXCEPT
                                  SELECT    TimeTypeId
                                  FROM      @ProjectTimeTypesList
                                ) dttl
                                INNER JOIN dbo.ChargeCode AS CC ON CC.TimeTypeId = dttl.TimeTypeId
                                                              AND CC.ProjectId = @ProjectId
                                INNER JOIN dbo.TimeEntry AS TE ON TE.ChargeCodeId = CC.Id
                                INNER JOIN dbo.TimeType AS tType ON tType.TimeTypeId = dttl.TimeTypeId
                                                              AND CC.TimeTypeId = dttl.TimeTypeId
                     )
            SELECT  @ErrorMessage = @ErrorMessage + '-' + TT.Name + '<br />'
            FROM    CTE AS TT

        IF ( @ErrorMessage = '' ) 
            BEGIN
		--FOR CUSTOM TIMETYPES

                DELETE  FROM dbo.ProjectTimeType
                WHERE   projectId = @ProjectId
                        AND TimetypeId NOT IN ( SELECT  *
                                                FROM    @ProjectTimeTypesList )

	

                INSERT  INTO dbo.ProjectTimeType
                        ( projectId ,
                          TimetypeId ,
                          isallowedtoshow
                        )
                        SELECT  @ProjectId ,
                                customProjectTimeTypes.TimeTypeId ,
                                1
                        FROM    ( SELECT    TimeTypeId
                                  FROM      @ProjectTimeTypesList
                                  EXCEPT
                                  SELECT    TimeTypeId
                                  FROM      @DefaultTimeTypesList
                                ) customProjectTimeTypes
                                LEFT JOIN ( SELECT  *
                                            FROM    dbo.ProjectTimeType
                                            WHERE   ProjectId = @ProjectId
                                          ) ptt ON ptt.TimeTypeId = customProjectTimeTypes.TimeTypeId
                        WHERE   ptt.TimeTypeId IS NULL

		--For Default TimeTypes

                DELETE  FROM dbo.ProjectTimeType
                WHERE   projectId = @ProjectId
                        AND TimetypeId IN ( SELECT  *
                                            FROM    @ProjectTimeTypesList
                                            INTERSECT
                                            ( SELECT    TT.TimeTypeId
                                              FROM      dbo.TimeType AS TT
                                              WHERE     TT.IsDefault = 1
                                            ) )

                INSERT  INTO dbo.ProjectTimeType
                        ( projectId ,
                          TimetypeId ,
                          isallowedtoshow
                        )
                        SELECT  @ProjectId ,
                                TimeTypeId ,
                                0
                        FROM    ( SELECT    TimeTypeId
                                  FROM      @DefaultTimeTypesList
                                  EXCEPT
                                  SELECT    TimeTypeId
                                  FROM      @ProjectTimeTypesList
                                ) dttl
            END
        ELSE 
            BEGIN
                SELECT  @ErrorMessage = 'Time has already been entered for the following Work Type(s). The Work Type(s) cannot be unassigned from this project.'
                        + @ErrorMessage
                RAISERROR(@ErrorMessage,16,1)
            END

    END

