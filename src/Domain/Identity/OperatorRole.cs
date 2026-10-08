namespace GameNet.Domain.Identity;

public enum OperatorRole
{
    Owner,
    Manager,
    Operator
}

public enum Permission
{
    DashboardRead,
    StationRead,
    StationWrite,
    AgentRead,
    AgentManage
}

public static class RolePermissions
{
    public static bool Has(OperatorRole role, Permission permission) =>
        role switch
        {
            OperatorRole.Owner => true,
            OperatorRole.Manager => permission is not Permission.AgentManage,
            OperatorRole.Operator => permission is Permission.DashboardRead or Permission.StationRead or Permission.AgentRead,
            _ => false
        };
}
