CREATE view dbo.v_Permissions
as
select perm.PersonID, perm.TargetId, targ.* 
from dbo.permission as perm
left join dbo.permissiontarget as targ on perm.targettype = targ.PermissionTypeID

