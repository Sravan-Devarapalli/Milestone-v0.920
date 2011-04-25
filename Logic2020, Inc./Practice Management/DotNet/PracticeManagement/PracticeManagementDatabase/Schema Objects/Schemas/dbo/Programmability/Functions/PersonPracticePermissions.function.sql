CREATE FUNCTION dbo.PersonPracticePermissions
(
	@PersonId	INT
)
RETURNS 
@result table
(
	TargetId int
)
AS
BEGIN
	-- Insert permissions from permissions table
	insert into @result
		select p.TargetId 
		from v_Permissions as p 
		where p.PermissionTypeId = 5 -- Practice 
				and p.PersonId = @PersonId

	-- Select top record from result set
	delete 
	from @result
	where TargetId IS NULL

	-- If it's null, then person has all permissions
	IF (@@RowCount > 0)
		-- Insert all targets
		insert into @result
			select pr.PracticeId
			from dbo.Practice as pr

	RETURN
END

