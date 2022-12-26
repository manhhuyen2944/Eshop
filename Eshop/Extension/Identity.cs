using System.Security.Claims;
using System.Security.Principal;

namespace Eshop.Extension
{
    public static class Identity
    {
        public static string GetAccountId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("AccountId");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetIsAdmin(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IsAdmin");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetUsername(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Username");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetSpecClaim(this ClaimsPrincipal claimsPrincipal, string claimytype)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimytype);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}

