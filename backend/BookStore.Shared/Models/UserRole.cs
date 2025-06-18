namespace BookStore.Shared.Models
{
    public enum UserRole
    {
        Customer = 0,
        Admin = 1,
        Manager = 2
    }
    
    public static class UserRoles
    {
        public const string Customer = "Customer";
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        
        public static string[] All => new[] { Customer, Admin, Manager };
        
        public static string GetRoleName(UserRole role)
        {
            return role.ToString();
        }
        
        public static UserRole ParseRole(string role)
        {
            return role switch
            {
                Customer => UserRole.Customer,
                Admin => UserRole.Admin,
                Manager => UserRole.Manager,
                _ => UserRole.Customer
            };
        }
    }
}
