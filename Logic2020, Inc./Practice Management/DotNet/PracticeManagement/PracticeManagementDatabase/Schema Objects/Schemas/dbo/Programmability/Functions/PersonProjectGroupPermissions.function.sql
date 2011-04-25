CREATE FUNCTION dbo.PersonProjectGroupPermissions
(
	@PersonId	INT
)
RETURNS 
@Result table
(
	TargetId int
)
AS
BEGIN
	-- Insert permissions from permissions table
	insert into @result
		select p.TargetId 
		from v_Permissions as p 
		where p.PermissionTypeId = 2 -- Project Group 
				and p.PersonId = @PersonId

	-- Select top record from result set
	delete 
	from @result
	where TargetId IS NULL

	-- If it's null, then person has all permissions
	IF (@@RowCount > 0)
		-- Insert all targets
		insert into @result
			select pg.GroupId
			from dbo.ProjectGroup as pg

	RETURN
END

