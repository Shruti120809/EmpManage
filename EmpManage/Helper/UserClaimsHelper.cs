using System.Security.Claims;

namespace EmpManage.Helper
{
    public class UserClaimsHelper
    {
        public static string GetCurrentUserName (ClaimsPrincipal user)
        {
            return user?.Identity?.Name ?? "Employee";
        }
    }
}
