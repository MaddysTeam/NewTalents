using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Symber.Web.Identity
{

	public interface IdentityDbContext<TUser, TRole, TKey> : IDisposable
      where TUser : IdentityUser<TKey>
      where TRole : IdentityRole<TKey>
      where TKey : IEquatable<TKey>
   {
      IdentityDbUser<TUser, TKey> UserDal { get; }
      IdentityDBRole<TRole, TKey> RoleDal { get; }
      IdentityDBUserClaim<TKey> UserClaimDal { get; }
      IdentityDBUserLogin<TUser, TKey> UserLoginDal { get; }
      IdentityDBUserRole<TUser, TKey> UserRoleDal { get; }
   }


   public interface IdentityDbUser<TUser, TKey>
      where TUser : IdentityUser<TKey>
      where TKey : IEquatable<TKey>
   {
      void Create(TUser user);
      void Update(TUser user);
      void Delete(TKey userId);
      TUser FindById(TKey userId);
      TUser FindByName(string userName);
      TUser FindByEmail(string email);
      IQueryable<TUser> GetAll();
   }


   public interface IdentityDBUserClaim<TKey>
      where TKey : IEquatable<TKey>
   {
      void AddClaims(TKey userId, IEnumerable<IdentityUserClaim<TKey>> claims);
      void RemoveClaim(TKey userId, IEnumerable<IdentityUserClaim<TKey>> claims);
      IList<Claim> GetClaims(TKey userId);
   }


   public interface IdentityDBUserLogin<TUser, TKey>
      where TUser : IdentityUser<TKey>
      where TKey : IEquatable<TKey>
   {
      void AddLogin(IdentityUserLogin<TKey> userLogin);
      void RemoveLogin(IdentityUserLogin<TKey> userLogin);
      IList<UserLoginInfo> GetLogins(TKey userId);
      TUser Find(UserLoginInfo userloginInfo);
   }


   public interface IdentityDBUserRole<TUser, TKey>
      where TUser : IdentityUser<TKey>
      where TKey : IEquatable<TKey>
   {
      void AddToRole(TKey userId, string roleName);
      void RemoveFromRole(TKey userId, string roleName);
      IList<string> GetRoles(TKey userId);
      bool IsInRole(TKey userId, string roleName);
   }


   public interface IdentityDBRole<TRole, TKey>
      where TRole : IdentityRole<TKey>
      where TKey : IEquatable<TKey>
   {
      void Create(TRole role);
      void Update(TRole role);
      void Delete(TKey roleId);
      TRole FindById(TKey roleId);
      TRole FindByName(string roleName);
      IQueryable<TRole> GetAll();
   }

}
