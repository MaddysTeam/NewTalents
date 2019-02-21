using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Symber.Web.Identity
{

	public class UserStore<TUser, TRole, TContext, TKey> :
		 IUserLoginStore<TUser, TKey>,
		 IUserRoleStore<TUser, TKey>,
		 IUserClaimStore<TUser, TKey>,
		 IUserPasswordStore<TUser, TKey>,
		 IUserSecurityStampStore<TUser, TKey>,
		 IUserEmailStore<TUser, TKey>,
		 IUserLockoutStore<TUser, TKey>,
		 IUserPhoneNumberStore<TUser, TKey>,
		 IQueryableUserStore<TUser, TKey>,
		 IUserTwoFactorStore<TUser, TKey>
		 where TUser : IdentityUser<TKey>
		 where TRole : IdentityRole<TKey>
		 where TContext : IdentityDbContext<TUser, TRole, TKey>
		 where TKey : IEquatable<TKey>
	{

		public UserStore(TContext context)
		{
			Context = context;
		}


		public virtual TContext Context { get; private set; }


		#region [ IQueryableUserStore ]


		public virtual IQueryable<TUser> Users
			=> Context.UserDal.GetAll();


		public async virtual Task CreateAsync(TUser user)
			=> await Task.Run(() => Context.UserDal.Create(user));


		public async virtual Task UpdateAsync(TUser user)
			=> await Task.Run(() => Context.UserDal.Update(user));


		public async virtual Task DeleteAsync(TUser user)
			=> await Task.Run(() => Context.UserDal.Delete(user.Id));


		public async virtual Task<TUser> FindByIdAsync(TKey userId)
			=> await Task.Run(() => Context.UserDal.FindById(userId));


		public async virtual Task<TUser> FindByNameAsync(string userName)
			=> await Task.Run(() => Context.UserDal.FindByName(userName));


		#endregion


		#region [ IUserEmailStore ]


		public virtual Task SetEmailAsync(TUser user, string email)
		{
			user.Email = email;
			return Task.FromResult(0);
		}


		public virtual Task<string> GetEmailAsync(TUser user)
			=> Task.FromResult(user.Email);


		public virtual Task<bool> GetEmailConfirmedAsync(TUser user)
			=> Task.FromResult(user.EmailConfirmed);


		public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed)
		{
			user.EmailConfirmed = confirmed;
			return Task.FromResult(0);
		}


		public virtual Task<TUser> FindByEmailAsync(string email)
			=> Task.FromResult(Context.UserDal.FindByEmail(email));


		#endregion


		#region [ IUserPhoneNumberStore ]


		public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber)
		{
			user.PhoneNumber = phoneNumber;
			return Task.FromResult(0);
		}


		public virtual Task<string> GetPhoneNumberAsync(TUser user)
			=> Task.FromResult(user.PhoneNumber);


		public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
		{
			user.PhoneNumberConfirmed = confirmed;
			return Task.FromResult(0);
		}


		public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
			=> Task.FromResult(user.PhoneNumberConfirmed);


		#endregion


		#region [ IUserLockoutStore ]


		public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
			=> Task.FromResult(user.LockoutEnd ?? DateTimeOffset.MinValue);


		public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
		{
			user.LockoutEnd = lockoutEnd;
			return Task.FromResult(0);
		}


		public virtual Task<int> IncrementAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount++;
			return Task.FromResult(user.AccessFailedCount);
		}


		public virtual Task ResetAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount = 0;
			return Task.FromResult(0);
		}


		public virtual Task<int> GetAccessFailedCountAsync(TUser user)
			=> Task.FromResult(user.AccessFailedCount);


		public virtual Task<bool> GetLockoutEnabledAsync(TUser user)
			=> Task.FromResult(user.LockoutEnabled);


		public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled)
		{
			user.LockoutEnabled = enabled;
			return Task.FromResult(0);
		}


		#endregion


		#region [ IUserSecurityStampStore ]


		public virtual Task SetSecurityStampAsync(TUser user, string stamp)
		{
			user.SecurityStamp = stamp;
			return Task.FromResult(0);
		}


		public virtual Task<string> GetSecurityStampAsync(TUser user)
			=> Task.FromResult(user.SecurityStamp);


		#endregion


		#region [ IUserPasswordStore ]


		public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
		{
			user.PasswordHash = passwordHash;
			return Task.FromResult(0);
		}


		public virtual Task<string> GetPasswordHashAsync(TUser user)
			=> Task.FromResult(user.PasswordHash);


		public virtual Task<bool> HasPasswordAsync(TUser user)
			=> Task.FromResult(user.PasswordHash != null);


		#endregion


		#region [ IUserClaimStore ]


		public async virtual Task<IList<Claim>> GetClaimsAsync(TUser user)
			=> await Task.Run(() => Context.UserClaimDal.GetClaims(user.Id));


		public virtual Task AddClaimAsync(TUser user, Claim claim)
		{
			Context.UserClaimDal.AddClaims(user.Id,
				new IdentityUserClaim<TKey>[]
				{
					new IdentityUserClaim<TKey>{ UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value }
				});
			return Task.FromResult(0);
		}

		public async virtual Task RemoveClaimAsync(TUser user, Claim claim)
			=> await Task.Run(() => Context.UserClaimDal.RemoveClaim(user.Id,
				new IdentityUserClaim<TKey>[]
				{
					new IdentityUserClaim<TKey>{ UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value }
				}));


		#endregion


		#region [ IUserLoginStore ]


		public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
		{
			Context.UserLoginDal.AddLogin(new IdentityUserLogin<TKey>
			{
				UserId = user.Id,
				ProviderKey = login.ProviderKey,
				LoginProvider = login.LoginProvider
			});
			return Task.FromResult(false);
		}


		public virtual async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
			=> await Task.Run(()
				=> Context.UserLoginDal.RemoveLogin(new IdentityUserLogin<TKey>
				{
					UserId = user.Id,
					ProviderKey = login.ProviderKey,
					LoginProvider = login.LoginProvider
				})
			);


		public async virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
			=> await Task.Run(() => Context.UserLoginDal.GetLogins(user.Id));


		public async virtual Task<TUser> FindAsync(UserLoginInfo login)
			=> await Task.Run(() => Context.UserLoginDal.Find(login));


		#endregion


		#region [ IUserRoleStore ]


		public async virtual Task AddToRoleAsync(TUser user, string roleName)
			=> await Task.Run(() => Context.UserRoleDal.AddToRole(user.Id, roleName));


		public async virtual Task RemoveFromRoleAsync(TUser user, string roleName)
			=> await Task.Run(() => Context.UserRoleDal.RemoveFromRole(user.Id, roleName));


		public virtual async Task<IList<string>> GetRolesAsync(TUser user)
			=> await Task.Run(() => Context.UserRoleDal.GetRoles(user.Id));


		public virtual async Task<bool> IsInRoleAsync(TUser user, string roleName)
			=> await Task.Run(() => Context.UserRoleDal.IsInRole(user.Id, roleName));


		#endregion


		#region [ IUserTwoFactorStore ]


		public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
		{
			user.TwoFactorEnabled = enabled;
			return Task.FromResult(0);
		}


		public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user)
			=> Task.FromResult(user.TwoFactorEnabled);


		#endregion


		#region [ IDisposable ]


		public void Dispose() { }


		#endregion

	}

}
