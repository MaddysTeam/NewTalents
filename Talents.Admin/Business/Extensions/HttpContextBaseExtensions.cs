using Business;
using Business.Security;
using System.Linq;
using System.Security.Claims;

namespace System.Web
{

   public static class HttpContextBaseExtensions
   {

      private readonly static object userProfileInHttpContext = new object();
      private readonly static string declarePeriodInHttpContext = string.Empty;
      private readonly static string evalPeriodInHttpContext = string.Empty;

      public static BzUserProfile GetUserProfile(this HttpContextBase httpContext)
      {
         BzUserProfile profile = httpContext.Items[userProfileInHttpContext] as BzUserProfile;


         if (profile == null)
         {
            profile = new BzUserProfile();

            var profileClaims = (httpContext.User.Identity as System.Security.Claims.ClaimsIdentity)
                .FindAll(m => m.Type.StartsWith(BzClaimTypes.UserProfile, StringComparison.InvariantCulture))
                .ToDictionary(m => m.Type.Substring(BzClaimTypes.UserProfile.Length));

            foreach (var prop in profile.GetType().GetProperties())
            {
               Claim claim;
               if (prop.CanWrite && profileClaims.TryGetValue(prop.Name, out claim))
               {
                  Type underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                  if (underlyingType != null && claim.Value == String.Empty)
                  {
                     prop.SetValue(profile, null);
                  }
                  else
                  {
                     prop.SetValue(profile, Convert.ChangeType(claim.Value, underlyingType ?? prop.PropertyType));
                  }
               }
            }

            httpContext.Items[userProfileInHttpContext] = profile;
         }


         return profile;
      }


      public static DeclarePeriod GetDeclarePeriod(this HttpContextBase httpContext)
      {
         DeclarePeriod period = httpContext.Items[declarePeriodInHttpContext] as DeclarePeriod;

         if (period == null)
         {
            period = DeclarePeriod.GetAll().FirstOrDefault(x => x.IsCurrent);

            httpContext.Items[declarePeriodInHttpContext] = period;
         }

         return period ?? new DeclarePeriod();
      }


      public static EvalPeriod GetEvalPeriod(this HttpContextBase httpContext)
      {
         EvalPeriod period = httpContext.Items[evalPeriodInHttpContext] as EvalPeriod;

         if (period == null)
         {
            period = EvalPeriod.GetAll().FirstOrDefault(x => x.IsCurrent);

            httpContext.Items[declarePeriodInHttpContext] = period;
         }

         return period ?? new EvalPeriod();
      }

      public static bool IsRole(this HttpContextBase httpContext, string roleName)
          => (httpContext.User.Identity as ClaimsIdentity)
                  .HasClaim(c => c.Type == ClaimTypes.Role && c.Value == roleName);


      public static bool IsInRole(this HttpContextBase httpContext, params string[] roleNames)
          => (httpContext.User.Identity as ClaimsIdentity)
                  .HasClaim(c => c.Type == ClaimTypes.Role && Array.IndexOf(roleNames, c.Value) != -1);


      public static bool IsRoleInScope(this HttpContextBase httpContext, string scopeType, long scopeId, string roleName)
          => (httpContext.User.Identity as ClaimsIdentity)
                  .HasClaim(c => c.Type == scopeType + scopeId && c.Value == roleName);


      public static bool IsRoleInScope(this HttpContextBase httpContext, string scopeType, long scopeId, params string[] roleNames)
          => (httpContext.User.Identity as ClaimsIdentity)
                  .HasClaim(c => c.Type == scopeType + scopeId && Array.IndexOf(roleNames, c.Type) != -1);


      public static bool HasPermission(this HttpContextBase httpContext, string permissionName)
      {
         var profile = httpContext.GetUserProfile();
         if (profile.IsSystemAdmin)
            return true;

         var role = BzRoleCache.FindRole(profile.UserType);
         var permission = BzPermissionCache.FindPermission(permissionName);
         if (role == null || permission == null)
            return false;

         var rolePermission = BzPermissionCache.FindRolePermission(role.Id, permission.Id);
         return rolePermission != null && rolePermission.IsGrant;
      }

   }

}