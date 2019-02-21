using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Symber.Web.Data;
using Symber.Web.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Business.Identity
{

	public class ApplicationDbContext : IdentityDbContext<BzUser, BzRole, long>
	{

		public ApplicationDbContext(APDBDef db)
		{
			var user = new DbUser() { DB = db };
			UserDal = user;
			UserLoginDal = user;
			UserClaimDal = user;
			UserRoleDal = user;

			var role = new DBRole() { DB = db };
			RoleDal = role;
		}


		public static ApplicationDbContext Create(IdentityFactoryOptions<ApplicationDbContext> options, IOwinContext context)
		{
			return new ApplicationDbContext(context.Get<APDBDef>());
		}


		public void Dispose()
		{
		}


		public IdentityDbUser<BzUser, long> UserDal { get; private set; }
		public IdentityDBUserLogin<BzUser, long> UserLoginDal { get; private set; }
		public IdentityDBUserClaim<long> UserClaimDal { get; private set; }
		public IdentityDBUserRole<BzUser, long> UserRoleDal { get; private set; }
		public IdentityDBRole<BzRole, long> RoleDal { get; private set; }


		public class DbUser :
		  IdentityDbUser<BzUser, long>,
		  IdentityDBUserLogin<BzUser, long>,
		  IdentityDBUserRole<BzUser, long>,
		  IdentityDBUserClaim<long>
		{

			public APDBDef DB { get; set; }


			#region [ IdentityDbUser ]


			public void Create(BzUser user)
			{
				DB.BzUserDal.Insert(user);
			}


			public void Delete(long userId)
			{
				var ur = APDBDef.BzUserRole;

				DB.BzUserRoleDal.ConditionDelete(ur.UserId == userId);
				DB.BzUserDal.PrimaryDelete(userId);
			}


			public BzUser FindByEmail(string email)
			{
				var u = APDBDef.BzUser;

				return DB.BzUserDal.ConditionQuery(u.Email == email, null, null, null).FirstOrDefault();
			}


			public BzUser FindById(long userId)
			{
				return DB.BzUserDal.PrimaryGet(userId);
			}


			public BzUser FindByName(string userName)
			{
				var u = APDBDef.BzUser;

				return DB.BzUserDal.ConditionQuery(u.UserName == userName, null, null, null).FirstOrDefault();
			}


			public IQueryable<BzUser> GetAll()
			{
				return DB.BzUserDal.ConditionQuery(null, null, null, null).AsQueryable();
			}


			public void Update(BzUser user)
			{
				DB.BzUserDal.Update(user);
			}


			#endregion


			#region [ IdentityDBUserRole ]


			public void AddToRole(long userId, string roleName)
			{
				var ur = APDBDef.BzUserRole;
				var r = APDBDef.BzRole;

				var roleId = DB.BzRoleDal.ConditionQuery(r.Name == roleName, null, null, null).FirstOrDefault().Id;

				DB.BzUserRoleDal.Insert(new BzUserRole()
				{
					UserId = userId,
					RoleId = roleId,
					ScopeId = 0,
					ScopeType = ""
				});
			}


			public void RemoveFromRole(long userId, string roleName)
			{
				var ur = APDBDef.BzUserRole;
				var r = APDBDef.BzRole;

				var roleId = DB.BzRoleDal.ConditionQuery(r.Name == roleName, null, null, null).FirstOrDefault().Id;

				DB.BzUserRoleDal.ConditionDelete(ur.RoleId == roleId);
			}


			public IList<string> GetRoles(long userId)
			{
				var ur = APDBDef.BzUserRole;
				var r = APDBDef.BzRole;

				return APQuery.select(r.Name).distinct()
					.from(r, ur.JoinInner(r.Id == ur.RoleId))
					.where(ur.UserId == userId)
					.query(DB, reader => reader.GetString(0))
					.ToList();
			}


			public bool IsInRole(long userId, string roleName)
			{
				var ur = APDBDef.BzUserRole;
				var r = APDBDef.BzRole;

				return (int)APQuery.select(ur.Asterisk.Count())
					.from(ur, r.JoinInner(ur.RoleId == r.Id))
					.where(ur.UserId == userId & r.Name == roleName)
					.executeScale(DB) > 0;
			}


			#endregion


			#region [ IdentityDBUserClaim ]


			public void AddClaims(long userId, IEnumerable<IdentityUserClaim<long>> claims)
			{
				foreach (var claim in claims)
				{
					DB.BzUserClaimDal.Insert(
						new BzUserClaim { UserId = userId, ClaimType = claim.ClaimType, ClaimValue = claim.ClaimValue }
						);
				}
			}


			public void RemoveClaim(long userId, IEnumerable<IdentityUserClaim<long>> claims)
			{
				var uc = APDBDef.BzUserClaim;
				foreach (var claim in claims)
				{
					DB.BzUserClaimDal.ConditionDelete(
						uc.UserId == userId & uc.ClaimType == claim.ClaimType & uc.ClaimValue == claim.ClaimValue
						);
				}
			}


			public IList<Claim> GetClaims(long userId)
			{
				var uc = APDBDef.BzUserClaim;

				return DB.BzUserClaimDal.ConditionQuery(uc.UserId == userId, null, null, null)
					 .Select(c => new Claim(c.ClaimType, c.ClaimValue))
					 .ToList();
			}


			#endregion


			#region [ IdentityDBUserLogin ]


			public void AddLogin(IdentityUserLogin<long> userLogin)
			{
				throw new NotImplementedException();
			}


			public IList<UserLoginInfo> GetLogins(long userId)
			{
				throw new NotImplementedException();
			}


			public void RemoveLogin(IdentityUserLogin<long> userLogin)
			{
				throw new NotImplementedException();
			}


			public BzUser Find(UserLoginInfo userloginInfo)
			{
				// 为 Caslogin 增加 IsExtLogined
				var user = FindByName(userloginInfo.ProviderKey);
				user.IsExtLogined = true;
				return user;
			}


			#endregion

		}


		public class DBRole :
			IdentityDBRole<BzRole, long>
		{

			public APDBDef DB { get; set; }


			#region [ IdentityDBRole ]


			public void Create(BzRole role)
			{
				DB.BzRoleDal.Insert(role);
			}


			public void Delete(long roleId)
			{
				var ur = APDBDef.BzUserRole;

				DB.BzUserRoleDal.ConditionDelete(ur.RoleId == roleId);
				DB.BzRoleDal.PrimaryDelete(roleId);
			}


			public BzRole FindById(long roleId)
			{
				return DB.BzRoleDal.PrimaryGet(roleId);
			}


			public BzRole FindByName(string roleName)
			{
				var r = APDBDef.BzRole;

				return DB.BzRoleDal.ConditionQuery(r.Name == roleName, null, null, null).FirstOrDefault();
			}


			public IQueryable<BzRole> GetAll()
			{
				return DB.BzRoleDal.ConditionQuery(null, null, null, null).AsQueryable();
			}


			public void Update(BzRole role)
			{
				DB.BzRoleDal.Update(role);
			}


			#endregion

		}


	}

}
