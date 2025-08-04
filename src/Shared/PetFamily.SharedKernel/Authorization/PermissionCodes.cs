namespace PetFamily.SharedKernel.Authorization;

public static class PermissionCodes
{
    public static class UserManagement
    {
        public static readonly string UserView = "User.View";
        public static readonly string UserCreate = "User.Create";
        public const string UserEdit = "User.Edit";
        public const string UserDelete = "User.Delete";
        public const string UserRestore = "User.Restore";
        public static List<string> GetAllPermissions() =>
            [UserView, UserCreate, UserEdit, UserDelete, UserRestore];
    }

    public static class VolunteerManagement
    {
        public const string VolunteerView = "Volunteer.View";
        public const string VolunteerCreate = "Volunteer.Create";
        public const string VolunteerEdit = "Volunteer.Edit";
        public const string VolunteerDelete = "Volunteer.Delete";
        public const string VolunteerRestore = "Volunteer.Restore";

        public static List<string> GetAllPermissions() =>
            [VolunteerView, VolunteerCreate, VolunteerEdit, VolunteerDelete, VolunteerRestore];
    }

    public static class RoleManagement
    {
        public const string RoleView = "Role.View";
        public const string RoleCreate = "Role.Create";
        public const string RoleEdit = "Role.Edit";
        public const string RoleDelete = "Role.Delete";
        public const string RoleRestore = "Role.Restore";
        public static List<string> GetAllPermissions() =>
            [RoleView, RoleCreate, RoleEdit, RoleDelete, RoleRestore];
    }

    public static class PermissionManagement
    {
        public const string PermissionView = "Permission.View";
        public const string PermissionCreate = "Permission.Create";
        public const string PermissionEdit = "Permission.Edit";
        public const string PermissionDelete = "Permission.Delete";
        public static List<string> GetAllPermissions() =>
            [PermissionView, PermissionCreate, PermissionEdit, PermissionDelete];
    }

    public static class SpeciesManagement
    {
        public const string SpeciesView = "Species.View";
        public const string SpeciesCreate = "Species.Create";
        public const string SpeciesEdit = "Species.Edit";
        public const string SpeciesDelete = "Species.Delete";
        public const string SpeciesRestore = "Species.Restore";

        public static List<string> GetAllPermissions() =>
            [SpeciesView, SpeciesCreate, SpeciesEdit, SpeciesDelete, SpeciesRestore];
    }

    public static class VolunteerRequestManagement
    {
        public const string VolunteerRequestsGetUnreviewed = "VolunteerRequests.GetUnreviewed";
        public const string VolunteerRequestsGetOnReview = "VolunteerRequests.GetOnReview";
        public const string VolunteerRequestView = "VolunteerRequest.View";
        public const string VolunteerRequestCreate = "VolunteerRequest.Create";
        public const string VolunteerRequestUpdate = "VolunteerRequest.Update";
        public const string VolunteerRequestDelete = "VolunteerRequest.Delete";
        public const string VolunteerRequestApprove = "VolunteerRequest.Approve";
        public const string VolunteerRequestReject = "VolunteerRequest.Reject";
        public const string VolunteerRequestTakeForReview = "VolunteerRequest.TakeForReview";
        public const string VolunteerRequestSendToRevision = "VolunteerRequest.SendToRevision";

        public static List<string> GetAllPermissions() =>
            [
            VolunteerRequestView,
            VolunteerRequestCreate,
            VolunteerRequestUpdate,
            VolunteerRequestDelete,
            VolunteerRequestApprove,
            VolunteerRequestReject,
            VolunteerRequestTakeForReview,
            VolunteerRequestSendToRevision
            ];
    }

    public static List<string> GetAllPermissionCodes()
    {
        var userManagementPermissions = UserManagement.GetAllPermissions();
        var volunteerManagementPermissions = VolunteerManagement.GetAllPermissions();
        var roleManagementPermissions = RoleManagement.GetAllPermissions();
        var permissionManagementPermissions = PermissionManagement.GetAllPermissions();
        var speciesManagementPermissions = SpeciesManagement.GetAllPermissions();

        List<string> list = [];

        list.AddRange(userManagementPermissions);
        list.AddRange(volunteerManagementPermissions);
        list.AddRange(roleManagementPermissions);
        list.AddRange(permissionManagementPermissions);
        list.AddRange(speciesManagementPermissions);

        return list;
    }

    public static List<string> GetPermissionsForAdmin() =>
        [
            UserManagement.UserDelete,
            UserManagement.UserView,
            UserManagement.UserRestore,
            VolunteerManagement.VolunteerView,
            VolunteerManagement.VolunteerCreate,
            VolunteerManagement.VolunteerDelete,
            VolunteerManagement.VolunteerRestore,
            RoleManagement.RoleView,
            RoleManagement.RoleCreate,
            RoleManagement.RoleEdit,
            RoleManagement.RoleDelete,
            RoleManagement.RoleRestore,
            PermissionManagement.PermissionView,
            PermissionManagement.PermissionCreate,
            PermissionManagement.PermissionEdit,
            PermissionManagement.PermissionDelete,
            SpeciesManagement.SpeciesView,
            SpeciesManagement.SpeciesCreate,
            SpeciesManagement.SpeciesEdit,
            SpeciesManagement.SpeciesDelete,
            SpeciesManagement.SpeciesRestore,
            VolunteerRequestManagement.VolunteerRequestView,
            VolunteerRequestManagement.VolunteerRequestDelete,
            VolunteerRequestManagement.VolunteerRequestApprove,
            VolunteerRequestManagement.VolunteerRequestReject,
            VolunteerRequestManagement.VolunteerRequestTakeForReview,
            VolunteerRequestManagement.VolunteerRequestSendToRevision,
            VolunteerRequestManagement.VolunteerRequestsGetUnreviewed,
            VolunteerRequestManagement.VolunteerRequestsGetOnReview 
        ];

    public static List<string> GetPermissionsForUser() =>
        [
            UserManagement.UserView,
            UserManagement.UserEdit,
            VolunteerManagement.VolunteerView,
            VolunteerRequestManagement.VolunteerRequestView,
            VolunteerRequestManagement.VolunteerRequestCreate,
            VolunteerRequestManagement.VolunteerRequestUpdate,
            VolunteerRequestManagement.VolunteerRequestDelete
        ];

    public static List<string> GetPermissionsForVolunteer() =>
        [
            UserManagement.UserView,
            UserManagement.UserEdit,
            VolunteerManagement.VolunteerView,
            VolunteerManagement.VolunteerEdit
        ];
}
