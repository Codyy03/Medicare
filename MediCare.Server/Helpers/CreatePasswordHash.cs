using Microsoft.AspNetCore.Identity;

namespace MediCare.Server.Helpers
{
    public static class CreatePasswordHash
    {
        public static string CreateHash(string password)
        {
            var hasher = new PasswordHasher<object>();
            return hasher.HashPassword(null, password);
        }
    }
}
