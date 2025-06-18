using Microsoft.AspNetCore.Authorization;

namespace BookStore.Shared.Authorization
{
    public class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
    
    public static class AuthorizationPolicies
    {
        public const string AdminOnly = "AdminOnly";
        public const string AdminOrManager = "AdminOrManager";
        public const string AuthenticatedUser = "AuthenticatedUser";
        
        public static void ConfigurePolicies(AuthorizationOptions options)
        {
            options.AddPolicy(AdminOnly, policy => 
                policy.RequireRole(Models.UserRoles.Admin));
                
            options.AddPolicy(AdminOrManager, policy => 
                policy.RequireRole(Models.UserRoles.Admin, Models.UserRoles.Manager));
                
            options.AddPolicy(AuthenticatedUser, policy => 
                policy.RequireAuthenticatedUser());
        }
    }
}
