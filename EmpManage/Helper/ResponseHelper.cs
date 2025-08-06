using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmpManage.Helper
{
    public static class ResponseHelper
    {
        public static string Success(string action, string entity)
            => $"{entity} {action} successfully.";

        public static string LoggedIn(string enitity)
            => $"{enitity} logged in Successfully";
        public static string NotFound(string entity)
            => $"{entity} not found.";

        public static string Invalid(string entity)
            => $"Invalid {entity}.";

        public static string Mismatch(string entity)
            => $"{entity} do not match.";

        public static string BadRequest(string field)
            => $"Please provide a valid {field}.";

        public static string Exists(string entity)
            => $"{entity} already exists.";

        public static string Removed(string entity, string from, string to)
            => $"{entity} {to} removed from {from}";

        public static string PermissionAssigned(int menu, int role)
            => $"Pemission of {menu} given to {role}";

        public static string InternalError(string entity)
            => $"Something went wrong while processing {entity}.";

        public static string ValidationError(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return "Validation failed: " + string.Join(", ", errors);
        }

        public static string Unauthorized()
            => $"Invalid Credentials";
    }

}