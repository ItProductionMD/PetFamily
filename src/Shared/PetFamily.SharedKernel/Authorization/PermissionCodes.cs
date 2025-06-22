namespace PetFamily.SharedKernel.Authorization;

public static class PermissionCodes
{
    public static class UserManagement
    {
        public const string UserView = "User.View";
        public const string UserCreate = "User.Create";
        public const string UserEdit = "User.Edit";
        public const string UserDelete = "User.Delete";
        public const string UserRestore = "User.Restore";
        public static List<string> GetPermissions() =>
            [UserView, UserCreate, UserEdit, UserDelete, UserRestore];
    }

    public static class VolunteerManagement
    {
        public const string VolunteerView = "Volunteer.View";
        public const string VolunteerCreate = "Volunteer.Create";
        public const string VolunteerEdit = "Volunteer.Edit";
        public const string VolunteerDelete = "Volunteer.Delete";
        public const string VolunteerRestore = "Volunteer.Restore";

        public static List<string> GetPermissions() =>
            [VolunteerView, VolunteerCreate, VolunteerEdit, VolunteerDelete, VolunteerRestore];
    }

    public static class RoleManagement
    {
        public const string RoleView = "Role.View";
        public const string RoleCreate = "Role.Create";
        public const string RoleEdit = "Role.Edit";
        public const string RoleDelete = "Role.Delete";
        public const string RoleRestore = "Role.Restore";
        public static List<string> GetPermissions() =>
            [RoleView, RoleCreate, RoleEdit, RoleDelete, RoleRestore];
    }

    public static class PermissionManagement
    {
        public const string PermissionView = "Permission.View";
        public const string PermissionCreate = "Permission.Create";
        public const string PermissionEdit = "Permission.Edit";
        public const string PermissionDelete = "Permission.Delete";
        public static List<string> GetPermissions() => 
            [PermissionView, PermissionCreate, PermissionEdit, PermissionDelete];
    }

    public static class SpeciesManagement
    {
        public const string SpeciesView = "Species.View";
        public const string SpeciesCreate = "Species.Create";
        public const string SpeciesEdit = "Species.Edit";
        public const string SpeciesDelete = "Species.Delete";
        public const string SpeciesRestore = "Species.Restore";

        public static List<string> GetPermissions() =>
            [SpeciesView, SpeciesCreate, SpeciesEdit, SpeciesDelete, SpeciesRestore];
    }

    public static List<string> GetAllPermissionCodes()
    {
        var userManagementPermissions = UserManagement.GetPermissions();
        var volunteerManagementPermissions = VolunteerManagement.GetPermissions();
        var roleManagementPermissions = RoleManagement.GetPermissions();
        var permissionManagementPermissions = PermissionManagement.GetPermissions();
        var speciesManagementPermissions = SpeciesManagement.GetPermissions();

        List<string> list = [];

        list.AddRange(userManagementPermissions);
        list.AddRange(volunteerManagementPermissions);
        list.AddRange(roleManagementPermissions);
        list.AddRange(permissionManagementPermissions);
        list.AddRange(speciesManagementPermissions);

        return list;
    }
}
