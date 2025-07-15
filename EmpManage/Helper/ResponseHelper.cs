namespace EmpManage.Helper
{
    public static class ResponseHelper
    {
        public static string LoggedIn(string entity) =>
            $"{entity} logged in successfully.";

        public static string Success(string action, string entity) =>
            $"{entity} {action} sucessfully";

        public static string Fetched(string entity, object id) =>
            $"{entity} with ID {id} was created successfully.";

        public static string Updated(string entity, object id) =>
            $"{entity} with ID {id} was updated successfully.";

        public static string Deleted(string entity, object id) =>
            $"{entity} with ID {id} was deleted successfully.";

        public static string Assigned(string item, object from, object to) =>
            $"{item} '{from}' was assigned to '{to}'.";

        public static string Removed(string item, object from, object to) =>
            $"{item} '{from}' was removed from '{to}'.";

        public static string NotFound(string entity, object id) =>
            $"{entity} with ID {id} was not found.";

        public static string AlreadyExists(string entity) =>
            $"{entity} already exists.";

        public static string Retrieved(string entity, object id) =>
            $"{entity} with ID {id} retrieved successfully.";

        public static string Fetched(string entity) =>
            $"{entity} fetched successfully.";

        public static string Unauthorized() =>
            "You are not authorized to access this resource.";

        public static string Forbidden() =>
            "You do not have permission to perform this action.";
    }
}