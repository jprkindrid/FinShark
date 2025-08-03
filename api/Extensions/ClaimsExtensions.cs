using System.Security.Claims;

namespace api.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.GivenName);
            return value ?? throw new InvalidOperationException("GivenName claim is missing.");
        }
    }
}
